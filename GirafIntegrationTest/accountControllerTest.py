from testLib import *
import time
import json
import requests

def AccountControllerTest():
    test = Test("Account Controller")
    url = Test.url()
    ####
    test.new('GETting username without authorization yields error')
    response = requests.get(url + 'user/username')
    test.ensureError(response)
    test.ensure('data' not in response or response['data'] is None)

    ####
    test.new('Login with valid credentials returns with "success"=True and "data"')
    response = requests.post(url + 'account/login', json = {"username": "Graatand", "password": "password"}).json()
    test.ensureSuccess(response)
    test.ensure('data' in response)
    graatandToken = response['data']

    ####
    test.new('GETting username with authorization')
    
    response = requests.get(url + 'user/username', headers = {"Authorization":"Bearer {0}".format(graatandToken)}).json()
    
    test.ensureSuccess(response)
    test.ensure(response['data'] == "Graatand")

    ####
    test.new('Login with invalid password returns with "success"=False and no "data"')
    response = requests.post(url + 'account/login', json = {"username": "Graatand", "password": "wrongPassword"}).json()
    test.ensureError(response)
    test.ensure('data' not in response or response['data'] is None)

    ####
    test.new('Login with invalid username returns with "success"=False and no "data"')
    response = requests.post(url + 'account/login', json = {"username": "WrongGraatand", "password": "password"}).json()
    test.ensureError(response)
    test.ensureNoData(response)

    # User story `Guardian would like to log in`
    ####
    test.new('Register new user')
    # Register Gunnar, without logging in
    # Will generate a unique enough number, so the user isn't already created
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = requests.post(url + 'account/register', json = {"username": gunnarUsername ,"password": "password","departmentId": 1}).json()
    test.ensureSuccess(response)

    ####
    test.new('Login as new user')
    response = requests.post(url + 'account/login', json = {"username": gunnarUsername, "password": "password"}).json()
    test.ensureSuccess(response)
    gunnarToken = response['data']
    print(response)

    ####
    test.new('Check if token is valid')
    response = requests.get(url + 'user/username', headers = {"Authorization":"Bearer {0}".format(gunnarToken)}).json()
    print (response)
    test.ensureSuccess(response)
    test.ensure(response['data'] == gunnarUsername)

    ####
    test.new('Check that Gunnar is a citizen')
    response = requests.get(url + 'user', headers = {"Authorization":"Bearer {0}".format(gunnarToken)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data']['roleName'] == 'Citizen')

    ####
    test.new('Login as department')
    response = requests.post(url + 'account/login', json = '{"username": "Tobias", "password": "password"}').json()
    test.ensureSuccess(response)
    tobiasToken = response['data']

    # TODO: Change password