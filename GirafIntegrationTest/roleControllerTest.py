from testLib import *
import time
import json


def testRoleController():
    test = controllerTest('Role Controller')
    tobias = test.login('Tobias')
    lee = test.login('Lee')

    ####
    test.newTest('Register Gunnar')
    # Will generate a unique enough number, so the user isn't already created
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
    test.ensureSuccess(response)

    gunnar = test.login(gunnarUsername)

    ####
    test.newTest('Gunnar tries to make himself guardian')
    response = test.request('POST', 'role/guardian/' + gunnarUsername, auth=gunnar)
    test.ensureError(response)

    ####
    test.newTest('Gunnar tries to make himself admin')
    response = test.request('POST', 'role/admin/' + gunnarUsername, auth=gunnar)
    test.ensureError(response)

    ####
    test.newTest('Gunnar should still be citizen')
    response = test.request('GET', 'user', auth=gunnar)
    test.ensure(response['data']['roleName'] == 'Citizen', 'Role was {0}'.format(response['data']['roleName']))

    response = test.request('GET', 'role', auth=gunnar)
    test.ensure(response['data'] == 'Citizen', 'Role was {0}'.format(response['data']))

    ####
    test.newTest('Add Gunnar to guardians')
    response = test.request('POST', 'role/guardian/' + gunnarUsername, auth=tobias)
    test.ensureSuccess(response)

    response = test.request('GET', 'user', auth=gunnar)
    test.ensureSuccess(response)
    test.ensureEqual('Guardian', response['data']['roleName'])

    ####
    test.newTest('Remove Gunnar from guardians')
    response = test.request('DELETE', 'role/guardian/' + gunnarUsername, auth=tobias)
    test.ensureSuccess(response)

    ####
    test.newTest('Try to remove Gunnar from guardians a second time')
    response = test.request('DELETE', 'role/guardian/' + gunnarUsername, auth=tobias)
    test.ensureError(response)

    ####
    test.newTest('Gunnar should be citizen again')
    response = test.request('GET', 'user', auth=gunnar)
    test.ensure(response['data']['roleName'] == 'Citizen', 'Role was {0}'.format(response['data']['roleName']))

    response = test.request('GET', 'role', auth=gunnar)
    test.ensure(response['data'] == 'Citizen', 'Role was {0}'.format(response['data']))

    ####
    test.newTest('Add Gunnar to admin')
    response = test.request('POST', 'role/admin/{0}'.format(gunnarUsername), auth=lee)
    test.ensureSuccess(response)
    response = test.request('GET', 'user', auth=gunnar)
    test.ensureEqual('SuperUser', response['data']['roleName'])

    response = test.request('GET', 'role', auth=gunnar)
    test.ensure(response['data'] == 'SuperUser', 'Role was {0}'.format(response['data']))

    ####
    test.newTest('Remove Gunnar from admin(as gunnar himself)')
    response = test.request('DELETE', 'role/admin/' + gunnarUsername, data='{}', auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.newTest('Gunnar should be citizen again')
    response = test.request('GET', 'user', auth=gunnar)
    test.ensure(response['data']['roleName'] == 'Citizen', 'Role was {0}'.format(response['data']['roleName']))

    response = test.request('GET', 'role', auth=gunnar)
    test.ensure(response['data'] == 'Citizen', 'Role was {0}'.format(response['data']))