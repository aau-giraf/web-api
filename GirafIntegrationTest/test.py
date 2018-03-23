from testLib import *
import time
import json

newTest() 
# Login with valid credentials returns with "success"=True and "data"
response = request('POST', 'account/login', '{"username": "Graatand", "password": "password"}')
assert(response['success'] is True)
assert('data' in response)
auth = response['data']

response = request('GET', 'user/username', auth=auth)
assert(response['success'] is True)
assert(response['data'] == "Graatand")

response = request('GET', 'user/username') # No authorization header
assert(response['success'] is False)
assert('data' not in response or response['data'] == None)

newTest() 
# Login with invalid password returns with "success"=False and no "data"
response = request('POST', 'account/login', '{"username": "Graatand", "password": "wrongPassword"}')
assert(response['success'] is False)
assert('data' not in response or response['data'] == None)

newTest()
# Login with invalid username returns with "success"=False and no "data"
response = request('POST', 'account/login', '{"username": "WrongGraatand", "password": "password"}')
assert(response['success'] is False)
assert('data' not in response or response['data'] == None)

newTest()
# Register new user, without logging in
# Will generate a unique enough number, so the user isn't already created
gunnarUsername = str(time.time())
response = request('POST', 'account/register', '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
assert(response['success'] is True)
# Login as new user
response = request('POST', 'account/login', '{"username": "' + gunnarUsername + '", "password": "password"}')
assert(response['success'] is True)
gunnarToken = response['data']
# Check if token is valid
response = request('GET', 'user/username', auth=gunnarToken)
assert(response['success'] is True)
assert(response['data'] == gunnarUsername)

# Check that gunnar is a citizen
response = request('GET', 'user', auth=gunnarToken)
assert(response['success'] is True)
assert(response['data']['roleName'] == 'Citizen')

# Login as department
response = request('POST', 'account/login', '{"username": "Tobias", "password": "password"}')
assert(response['success'] is True)
tobiasToken = response['data']

# Add gunnar to guardians
response = request('POST', 'role/guardian/' + gunnarUsername, auth=tobiasToken)
assert(response['success'] is True)

# Check that gunnar is a guardian
response = request('GET', 'user', auth=gunnarToken)
print(response)
assert(response['success'] is True)
assert(response['data']['roleName'] == 'Guardian')