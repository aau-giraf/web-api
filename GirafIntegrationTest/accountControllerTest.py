from testLib import *
import time
import json


def testAccountController():
    test = controllerTest("Account Controller")

    ####
    test.newTest('GETting role without authorization yields error')
    response = test.request('GET', '/role')
    test.ensure(response['success'] is False)
    test.ensure('data' not in response or response['data'] is None)

    ####
    test.newTest('GETting username without authorization yields error')
    response = test.request('GET', '/username')
    test.ensure(response['success'] is False)
    test.ensure('data' not in response or response['data'] is None)

    ####
    test.newTest('Login with valid credentials returns with "success"=True and "data"')
    response = test.request('POST', 'account/login', '{"username": "Graatand", "password": "password"}')
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))
    test.ensure('data' in response)
    graatandToken = response['data']

    ####
    test.newTest('GETting role with authorization')
    response = test.request('GET', 'role', auth=graatandToken)
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))
    test.ensure(response['data'] == "Guardian")

    ####
    test.newTest('GETting username with authorization')
    response = test.request('GET', 'user/username', auth=graatandToken)
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))
    test.ensure(response['data'] == "Graatand")

    ####
    test.newTest('Login with invalid password returns with "success"=False and no "data"')
    response = test.request('POST', 'account/login', '{"username": "Graatand", "password": "wrongPassword"}')
    test.ensure(response['success'] is False)
    test.ensure('data' not in response or response['data'] is None)

    ####
    test.newTest('Login with invalid username returns with "success"=False and no "data"')
    response = test.request('POST', 'account/login', '{"username": "WrongGraatand", "password": "password"}')
    test.ensure(response['success'] is False)
    test.ensure('data' not in response or response['data'] is None)

    # User story `Guardian would like to log in`
    ####
    test.newTest('Register new user')
    # Register Gunnar, without logging in
    # Will generate a unique enough number, so the user isn't already created
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))

    ####
    test.newTest('Login as new user')
    response = test.request('POST', 'account/login', '{"username": "' + gunnarUsername + '", "password": "password"}')
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))
    gunnarToken = response['data']

    ####
    test.newTest('Check if token is valid')
    response = test.request('GET', 'user/username', auth=gunnarToken)
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))
    test.ensure(response['data'] == gunnarUsername)

    ####
    test.newTest('Check that Gunnar is a citizen')
    response = test.request('GET', 'user', auth=gunnarToken)
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))
    test.ensure(response['data']['roleName'] == 'Citizen')

    ####
    test.newTest('Login as department')
    response = test.request('POST', 'account/login', '{"username": "Tobias", "password": "password"}')
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))
    tobiasToken = response['data']

    ####
    test.newTest('Add Gunnar to guardians')
    response = test.request('POST', 'role/guardian/' + gunnarUsername, auth=tobiasToken)
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))

    ####
    test.newTest('Check that Gunnar is a guardian')
    response = test.request('GET', 'user', auth=gunnarToken)
    test.ensure(response['success'] is True, 'Error: {0}'.format(response['errorKey']))
    test.ensure(response['data']['roleName'] == 'Guardian')
