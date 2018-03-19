from testLib import *

# Test that newTest works as expected
response = request('POST', 'account/login', '{"username": "Graatand", "password": "password"}')
newTest()
assert(not isLoggedIn())

newTest()
# Login with valid credentials returns with "success"=True and "data"
response = request('POST', 'account/login', '{"username": "Graatand", "password": "password"}')
assert(response['success'] is True)
assert('data' in response)

response = request('GET', 'user/username') # Does not work because it needs authentication, which is not implemented yet
assert(response['success'] is True)
assert(response['data'] == "Graatand")

newTest()
# Login with invalid password returns with "success"=False and no "data"
response = request('POST', 'account/login', '{"username": "Graatand", "password": "wrongPassword"}')
assert(response['success'] is False)
assert('data' not in response or response['data'] == None)
assert(not isLoggedIn())

newTest()
# Login with invalid username returns with "success"=False and no "data"
response = request('POST', 'account/login', '{"username": "WrongGraatand", "password": "password"}')
assert(response['success'] is False)
assert('data' not in response or response['data'] == None)
assert(not isLoggedIn())

newTest()
# Register new user
response = request('POST', 'account/register', '{"username": "Gunnar","password": "password","departmentId": 1}')
assert(response['success'] is True)
response = request('POST', 'account/login', '{"username": "Gunnar", "password": "password"}')
assert(response['success'] is True)
response = request('POST', 'user/username')
assert(response['data'] == "Gunnar")