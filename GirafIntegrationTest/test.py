from testLib import *
import time
import json

acc = controllerTest("Account Controller")

acc.newTest('Login with valid credentials returns with "success"=True and "data"') 
response = acc.request('POST', 'account/login', '{"username": "Graatand", "password": "password"}')
acc.ensure(response['success'] is True)
acc.ensure('data' in response)
auth = response['data']

response = acc.request('GET', 'user/username', auth=auth)
acc.ensure(response['success'] is True)
acc.ensure(response['data'] == "Graatand")

response = acc.request('GET', 'user/username') # No authorization header
acc.ensure(response['success'] is False)
acc.ensure('data' not in response or response['data'] == None)

acc.newTest('Login with invalid password returns with "success"=False and no "data"') 
response = acc.request('POST', 'account/login', '{"username": "Graatand", "password": "wrongPassword"}')
acc.ensure(response['success'] is False)
acc.ensure('data' not in response or response['data'] == None)

acc.newTest('Login with invalid username returns with "success"=False and no "data"')
response = acc.request('POST', 'account/login', '{"username": "WrongGraatand", "password": "password"}')
acc.ensure(response['success'] is False)
acc.ensure('data' not in response or response['data'] == None)

acc.newTest('Register new user, without logging in')
# Will generate a unique enough number, so the user isn't already created
gunnarUsername = str(time.time())
response = acc.request('POST', 'account/register', '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
acc.ensure(response['success'] is True)

# Login as new user
response = acc.request('POST', 'account/login', '{"username": "' + gunnarUsername + '", "password": "password"}')
acc.ensure(response['success'] is True)
gunnarToken = response['data']

# Check if token is valid
response = acc.request('GET', 'user/username', auth=gunnarToken)
acc.ensure(response['success'] is True)
acc.ensure(response['data'] == gunnarUsername)

# Check that gunnar is a citizen
response = acc.request('GET', 'user', auth=gunnarToken)
acc.ensure(response['success'] is True)
acc.ensure(response['data']['roleName'] == 'Citizen')

# Login as department
response = acc.request('POST', 'account/login', '{"username": "Tobias", "password": "password"}')
acc.ensure(response['success'] is True)
tobiasToken = response['data']

# Add gunnar to guardians
response = acc.request('POST', 'role/guardian/' + gunnarUsername, auth=tobiasToken)
acc.ensure(response['success'] is True)

# Check that gunnar is a guardian
response = acc.request('GET', 'user', auth=gunnarToken)
# print(response)
acc.ensure(response['success'] is True)
acc.ensure(response['data']['roleName'] == 'Guardian')
