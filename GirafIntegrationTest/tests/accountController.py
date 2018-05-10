from integrate import TestCase, test
from testLib import *
import requests
import time

class AccountController(TestCase):
    "Account Controller"
    graatandToken = None
    graatandId = None
    gunnarToken = None
    tobiasToken = None
    gunnarUsername = None
    grundenbergerUsername = None
    grundenberger = None

    @test()
    def getUsernameNoAuth(self, check):
        "GETting username without authorization yields error"
        response = requests.get(Test.url + 'user/username').json()
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")

    @test()
    def loginAsGraatand(self, check):
        "Login as Graatand"
        response = requests.post(Test.url + 'account/login', json = {"username": "Graatand", "password": "password"}).json()
        check.is_true(response['success'])
        check.is_not_none(response['data'])
        AccountController.graatandToken = response['data']

    @test(skip_if_failed=["loginAsGraatand"])
    def getGraatandId(self, check):
        "Get Graatands ID"
        response = requests.get(Test.url + 'User', headers=auth(self.graatandToken)).json()
        ensureSuccess(response, check)
        self.graatandId = response['data']['id'];

    # create new user - add weekschdueles - add pictos - delete user - ensure user is deleted, 
    @test(skip_if_failed=['loginAsGraatand'])
    def registerGrundenberger(self, check):
        'Register grundenberger'
        self.grundenbergerUsername = 'grundenberger{0}'.format(str(time.time()))

        response = requests.post(Test.url + 'account/register', headers=auth(self.graatandToken), json={
            "username": self.grundenbergerUsername,
            "password": "password",
            "role": "Citizen",
            "departmentId": 1
        }).json()
        ensureSuccess(response, check)

    @test(skip_if_failed=['registerGrundenberger'])
    def loginGrundenberger(self, check):
        "Login grundenberger"
        self.grundenberger = login(self.grundenbergerUsername, check)

    @test(skip_if_failed=['loginGrundenberger'])
    def getGrundenbergerInfo(self, check):
        response = requests.get(Test.url + 'User', headers=auth(self.grundenberger)).json()
        ensureSuccess(response, check)

    @test(skip_if_failed=["loginAsGraatand"])
    def getUsernameWithAuth(self, check):
        "GETting username with authorization"
        response = requests.get(Test.url + 'user', headers = {"Authorization":"Bearer {0}".format(AccountController.graatandToken)}).json()
        check.is_true(response['success'])
        check.is_not_none(response['data'])
        check.equal(response['data']['username'], "Graatand")

    @test()
    def loginInvalidPassword(self, check):
        "Login with invalid password"
        response = requests.post(Test.url + 'account/login', json = {"username": "Graatand", "password": "wrongPassword"}).json()
        check.is_false(response['success'])
        check.equal(response['errorKey'], "InvalidCredentials")


    @test()
    def loginInvalidUsername(self, check):
        "Login with invalid username"
        response = requests.post(Test.url + 'account/login', json = {"username": "WrongGraatand", "password": "password"}).json()
        check.is_false(response['success'])
        check.equal(response['errorKey'], "InvalidCredentials")

    # User story `Guardian would like to log in`
    @test()
    def registerUserGunnarNoAuth(self, check):
        "Register Gunnar, without logging in"
        # Will generate a unique enough number, so the user isn't already created
        response = requests.post(Test.url + 'account/register', json = {"username": 'Gunnar{0}'.format(str(time.time())) ,"password": "password", "role": "Citizen", "departmentId": 1}).json()
        check.is_false(response['success'])

    @test(skip_if_failed=['loginAsGraatand', 'registerUserGunnarNoAuth'])
    def registerUserGunnarWithAuth(self, check):
        "Register Gunnar, with graatand"
        # Will generate a unique enough number, so the user isn't already created
        AccountController.gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
        response = requests.post(Test.url + 'account/register', headers = {"Authorization":"Bearer {0}".format(AccountController.graatandToken)}, json = {"username": AccountController.gunnarUsername ,"password": "password", "role": "Citizen", "departmentId": 1}).json()
        check.is_true(response['success'])

    @test(skip_if_failed=["registerUserGunnarWithAuth"])
    def loginAsGunnar(self, check):
        "Login as new user"
        response = requests.post(Test.url + 'account/login', json = {"username": AccountController.gunnarUsername, "password": "password"}).json()
        check.is_true(response['success'])
        check.is_not_none(response['data'])
        AccountController.gunnarToken = response['data']

    @test(skip_if_failed=["loginAsGunnar"])
    def testGunnarsToken(self, check):
        "Check if gunnars token is valid"
        response = requests.get(Test.url + 'user', headers = {"Authorization":"Bearer {0}".format(AccountController.gunnarToken)}).json()
        check.is_true(response['success'])
        check.equal(response['data']['username'], AccountController.gunnarUsername)

    @test(skip_if_failed=["loginAsGunnar"])
    def testGunnarRole(self, check):
        "Check that Gunnar is a citizen"
        response = requests.get(Test.url + 'user', headers = {"Authorization":"Bearer {0}".format(AccountController.gunnarToken)}).json()
        check.is_true(response['success'])
        check.equal(response['data']['roleName'], 'Citizen')

    @test()
    def loginAsTobias(self, check):
        "Login as department"
        response = requests.post(Test.url + 'account/login', json = {"username": "Tobias", "password": "password"}).json()
        check.is_true(response['success'])
        check.is_not_none(response['data'])
        AccountController.tobiasToken = response['data']

    @test(skip_if_failed=['loginGrundenberger'], depends=["getGrundenbergerInfo"], expect_fail=True)
    def deleteGrundenberger(self, check):
        "Remove Grundenberger"
        check.fail(); # TODO

    @test(skip_if_failed=['deleteGrundenberger'])
    def loginAsDeletedGrundenberger(self, check):
        "Try loggin in as deleted user"
        login(self.grundenbergerUsername, check, fail=True)

    @test(skip_if_failed=['deleteGrundenberger', 'getGraatandId'])
    def useDeletedGrundenbergerToken(self, check):
        "Try using deleted users token"
        response = requests.get(Test.url + 'User/{0}'.format(self.graatandId), headers=auth(self.grundenberger)).json()
        check.is_false(response['success'])

    # TODO: Change password