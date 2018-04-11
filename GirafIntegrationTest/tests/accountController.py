from integrate import TestCase, test
from testLib import *
import requests

class AccountController(TestCase):
    "Account Controller"
    graatandToken = None
    gunnarToken = None
    tobiasToken = None
    gunnarUsername = None



    @test()
    def getUsernameNoAuth(self, check):
        "GETting username without authorization yields error"
        response = requests.get(Test.url + 'user/username').json()
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")

    @test()
    def loginAsGraatand(self, check):
        "Login as gunnar"
        response = requests.post(Test.url + 'account/login', json = {"username": "Graatand", "password": "password"}).json()
        check.is_true(response['success'])
        check.is_not_none(response['data'])
        AccountController.graatandToken = response['data']

    @test(depends=["loginAsGraatand"])
    def getUsernameWithAuth(self, check):
        "GETting username with authorization"
        response = requests.get(Test.url + 'user/username', headers = {"Authorization":"Bearer {0}".format(graatandToken)}).json()
        check.is_true(response['success'])
        check.is_not_none(response['data'])
        check.equal(response['data'], "Graatand")

    @test()
    def loginInvalidPassword(self, check):
        "Login with invalid password"
        response = requests.post(Test.url + 'account/login', json = {"username": "Graatand", "password": "wrongPassword"}).json()
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")


    @test()
    def loginInvalidUsername(self, check):
        "Login with invalid username"
        response = requests.post(Test.url + 'account/login', json = {"username": "WrongGraatand", "password": "password"}).json()
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")

    # User story `Guardian would like to log in`
    @test()
    def registerUserGunnarNoAuth(self, check):
        "Register Gunnar, without logging in"
        # Will generate a unique enough number, so the user isn't already created
        AccountController.gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
        response = requests.post(Test.url + 'account/register', json = {"username": AccountController.gunnarUsername ,"password": "password","departmentId": 1}).json()
        check.is_true(response['success'])

    @test(depends=["registerUserGunnarNoAuth"])
    def loginAsGunnar(self, check):
        "Login as new user"
        response = requests.post(Test.url + 'account/login', json = {"username": AccountController.gunnarUsername, "password": "password"}).json()
        check.is_true(response['success'])
        check.is_not_none(response['data'])
        AccountController.gunnarToken = response['data']

    @test(depends=["loginAsGunnar"])
    def testGunnarsToken(self, check):
        "Check if gunnars token is valid"
        response = requests.get(Test.url + 'user/username', headers = {"Authorization":"Bearer {0}".format(AccountController.gunnarToken)}).json()
        check.is_true(response['success'])
        check.equal(response['data'], AccountController.gunnarUsername)

    @test(depends=["loginAsGunnar"])
    def testGunnarRole(self, check):
        "Check that Gunnar is a citizen"
        response = requests.get(Test.url + 'user', headers = {"Authorization":"Bearer {0}".format(AccountController.gunnarToken)}).json()
        check.is_true(response['success'])
        check.equal(response['data']['roleName'], 'Citizen')

    @test()
    def loginAsTobias(self, check):
        "Login as department"
        response = requests.post(Test.url + 'account/login', json = '{"username": "Tobias", "password": "password"}').json()
        print(response)
        check.is_true(response['success'])
        check.is_not_none(response['data'])
        AccountController.tobiasToken = response['data']

    # TODO: Change password