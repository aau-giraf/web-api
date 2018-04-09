# -*- coding: utf-8 -*-
from testLib import *
import time
import json


def testDepartmentController():
    test = controllerTest('Department Controller')
    lee = test.login('Lee')
    graatand = test.login('Graatand')
    kurt = test.login('Kurt')

    dalgaardsholmstuen = 'Dalgaardsholmstuen{0}'.format(str(time.time()))

    ####
    test.newTest('Get list of departments')
    response = test.request('GET', 'Department')
    if test.ensureSuccess(response):
        numberOfDepartments = len(response['data'])

    ####
    test.newTest('Graatand tries to create department')
    body = '''
        {{
            "id": 0,
            "name": "{0}",
            "members": [ ],
            "resources": [ ]
        }}'''.format(dalgaardsholmstuen)

    response = test.request('POST', 'Department', data=body, auth=graatand)
    test.ensureError(response)

    # Data for creating Dalgaardsholmstuen
    body = '''
        {{
            "id": 0,
            "name": "{0}",
            "members": [ ],
            "resources": [ ]
        }}'''.format(dalgaardsholmstuen)

    ####
    test.newTest('Kurt tries to create department')
    response = test.request('POST', 'Department', data=body, auth=kurt)
    test.ensureError(response)

    ####
    test.newTest('Make sure number of departments has remained the same')
    response = test.request('GET', 'Department')
    test.ensureSuccess(response)
    test.ensure(numberOfDepartments == len(response['data']))

    ####
    test.newTest('Lee creates department')
    response = test.request('POST', 'Department', data=body, auth=lee)
    test.ensure(response['success'] is True, errormessage='Error: {0}'.format(response['errorKey']))

    dalgardsholmstuenId = response.get('data').get('id')
    test.ensure(dalgardsholmstuenId is not None, errormessage='Could not get ID of Dalgaardsholmstuen')

    ####
    test.newTest('Get the newly created Dalgaardsholmstuen')
    response = test.request('GET', 'Department/{0}'.format(dalgardsholmstuenId))
    test.ensure(response['success'] is True, errormessage='Error: {0}'.format(response['errorKey']))
    test.ensure(response.get('data').get('name') == dalgaardsholmstuen,
                errormessage='Name should\'ve been {0} but was {1}'.format(dalgaardsholmstuen, response.get('name')))

    ####
    test.newTest('Get nonexistent department')
    response = test.request('GET', 'Department/-1')
    test.ensureError(response)

    ####
    test.newTest('Register Gunnar to Tobias\' department')
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
    test.ensureSuccess(response)

    gunnar = test.login(gunnarUsername)

    gunnarID = test.request('GET', 'User', auth=gunnar).get('data').get('id')
    test.ensure(gunnarID is not None)

    # Data that has Gunnars ID in it.
    body = '''
    {{
        "role": -1,
        "guardians": null,
        "id": "{0}",
        "username": "{1}",
        "screenName": null,
        "userIcon": null,
        "department": -1,
        "weekScheduleIds": []
    }}
    '''.format(gunnarID, gunnarUsername)


    ####
    test.newTest('Kurt tries to add Gunnar to Dalgaardsholmstuen')
    response = test.request('POST', 'Department/user/{0}'.format(dalgardsholmstuenId), data=body, auth=kurt)
    test.ensureError(response)

    ####
    test.newTest('Graatand tries to add Gunnar to Dalgaardsholmstuen')
    response = test.request('POST', 'Department/user/{0}'.format(dalgardsholmstuenId), data=body, auth=graatand)
    test.ensureError(response)

    ####
    test.newTest('Check that Gunnar has not changed departments yet')
    response = test.request('GET', 'User', auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response['data']['department'] == 1, 'Gunnar was moved to new department!')

    ####
    test.newTest('Gunnar moves himself to Dalgaardsholmstuen')
    response = test.request('POST', 'Department/user/{0}'.format(dalgardsholmstuenId), data=body, auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.newTest('Check that Gunnar has in fact changed departments')
    response = test.request('GET', 'User', auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response['data']['department'] == dalgardsholmstuenId, 'Gunnar was not moved to new department!')

    # TODO : Recomment when endpoint is fixed
    ####
    #test.newTest('Kurt tries to remove Gunnar from Dalgaardsholmstuen')
    #response = test.request('DELETE', 'Department/user/{0}'.format(dalgardsholmstuenId), data=body, auth=kurt)
    #test.ensureError(response)

    # TODO : Recomment when endpoint is fixed
    ####
    #test.newTest('Graatand tries to remove Gunnar from Dalgaardsholmstuen')
    #response = test.request('DELETE', 'Department/user/{0}'.format(dalgardsholmstuenId), data=body, auth=graatand)
    #test.ensureError(response)

    ####
    test.newTest('Check that Gunnar has not changed departments yet')
    response = test.request('GET', 'User', auth=gunnar)
    if test.ensureSuccess(response):
        test.ensure(response['data']['department'] == dalgardsholmstuenId, 'Gunnar was moved to new department!')

    # TODO : Recomment when endpoint is fixed
    ####
    #test.newTest('Gunnar removes himself from Dalgaardsholmstuen')
    #response = test.request('DELETE', 'Department/user/{0}'.format(dalgardsholmstuenId), data=body, auth=gunnar)
    #test.ensureSuccess(response)

    ####
    test.newTest('Check that Gunnar was removed')
    response = test.request('GET', 'User', auth=gunnar)
    if test.ensureSuccess(response):
        test.ensure(response['data']['department'] != dalgardsholmstuenId, 'Gunnar was not moved to new department!')

    ####
    test.newTest('Post Cyclopian pictogram')
    body = ''' 
        {  
            "accessLevel": 0,  "title": "Cyclopian",  "id": -1,  "lastEdit": "2018-03-19T10:40:26.587Z"  
        }  
    '''
    test.request('POST', 'Pictogram', data=body, auth=gunnar)
    test.ensureSuccess(response)
    cyclopianBody = ''' 
        {{
            "accessLevel": 0,  "title": "Cyclopian",  "id": {0},  "lastEdit": "2018-03-19T10:40:26.587Z"  
        }} 
    '''.format(response['data']['id'])

    ####
    test.newTest('Kurt tries to add Cyclopian to Dalgaardsholmstuen')
    test.request('POST', 'Department/resource/{0}'.format(dalgardsholmstuenId), data=body, auth=kurt)
    test.ensureError(response)

    ####
    test.newTest('Add Cyclopian to Dalgaardsholmstuen')
    test.request('POST', 'Department/resource/{0}'.format(dalgardsholmstuenId), data=cyclopianBody, auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.newTest('Kurt tries to remove Cyclopian from Dalgaardsholmstuen')
    test.request('DELETE', 'Department/resource/{0}'.format(dalgardsholmstuenId), data=body, auth=kurt)
    test.ensureError(response)

    ####
    test.newTest('Remove Cyclopian from Dalgaardsholmstuen')
    test.request('DELETE', 'Department/resource/{0}'.format(dalgardsholmstuenId), data=cyclopianBody, auth=gunnar)
    test.ensureSuccess(response)
