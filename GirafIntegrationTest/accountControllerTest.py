from testLib import *
import time
import json
import requests

def AccountController():
    "Account Controller"
    graatandToken = None;

    @test()
    def getUsernameNoAuth():
        "GETting username without authorization yields error"
        response = requests.get(Test.url + 'user/username')
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")

    @test()
    def loginAsGraatand():
        "Login as gunnar"
        response = requests.post(Test.url + 'account/login', json = {"username": "Graatand", "password": "password"}).json()
        check.is_true(response['success'])
        check.is_not_None(response['data'])
        AccountController.graatandToken = response['data']

    @test(depends="loginAsGraatand")
    def getUsernameWithAuth():
        "GETting username with authorization"
        response = requests.get(Test.url + 'user/username', headers = {"Authorization":"Bearer {0}".format(graatandToken)}).json()
        check.is_true(response['success'])
        check.is_not_None(response['data'])
        check.equal(response['data'], "Graatand")

    @test()
    def loginInvalidPassword():
        "Login with invalid password"
        response = requests.post(Test.url + 'account/login', json = {"username": "Graatand", "password": "wrongPassword"}).json()
        check.ensureError(response)
        check.ensure('data' not in response or response['data'] is None)

    @test()
    def test():
        "Login with invalid username returns with "success"=False and no "data""
        response = requests.post(Test.url + 'account/login', json = {"username": "WrongGraatand", "password": "password"}).json()
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")

        # User story `Guardian would like to log in`
    @test()
    def test():
        "Register new user"
        # Register Gunnar, without logging in
        # Will generate a unique enough number, so the user isn't already created
        gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
        response = requests.post(Test.url + 'account/register', json = {"username": gunnarUsername ,"password": "password","departmentId": 1}).json()
        test.ensureSuccess(response)

    @test()
    def test():
        "Login as new user"
        response = requests.post(Test.url + 'account/login', json = {"username": gunnarUsername, "password": "password"}).json()
        test.ensureSuccess(response)
        gunnarToken = response['data']

    @test()
    def test():
        "Check if token is valid"
        response = requests.get(Test.url + 'user/username', headers = {"Authorization":"Bearer {0}".format(gunnarToken)}).json()
        test.ensureSuccess(response)
        test.ensure(response['data'] == gunnarUsername)

    @test()
    def test():
        "Check that Gunnar is a citizen"
        response = requests.get(Test.url + 'user', headers = {"Authorization":"Bearer {0}".format(gunnarToken)}).json()
        test.ensureSuccess(response)
        test.ensure(response['data']['roleName'] == 'Citizen')

    @test()
    def test():
        "Login as department"
        response = requests.post(Test.url + 'account/login', json = '{"username": "Tobias", "password": "password"}').json()
        test.ensureSuccess(response)
        tobiasToken = response['data']

        # TODO: Change password