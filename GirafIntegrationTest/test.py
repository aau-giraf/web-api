import testLib

newTest()
# Login with valid credentials returns with "success"=True and "data"
response = request('POST', 'account/login', '{"username": "Graatand", "password": "password"}')
assert(response['success'] is True)
assert('data' in response)

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
# Register new user without being logged in
response = request('POST', 'account/login', '{"username": "WrongGraatand", "password": "password"}')
assert(response['success'] is False)
assert('data' not in response or response['data'] == None)