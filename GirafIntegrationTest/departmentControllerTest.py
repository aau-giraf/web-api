
# -*- coding: utf-8 -*-
from testLib import *
import time
import json


def DepartmentControllerTest():
    test = Test('Department Controller')
    lee = test.login('Lee')
    graatand = test.login('Graatand')
    kurt = test.login('Kurt')

    dalgaardsholmstuen = 'Dalgaardsholmstuen{0}'.format(str(time.time()))

    ####
    test.new('Get list of departments')
    response = requests.get(Test.url() + 'Department').json()
    if test.ensureSuccess(response):
        numberOfDepartments = len(response['data'])
    else:
        numberOfDepartments = 0;

    ####
    test.new('Graatand tries to create department')

    response = requests.post(Test.url() + 'Department', json= {
            "id": 0,
            "name": dalgaardsholmstuen,
            "members": [ ],
            "resources": [ ]
        }, headers = {"Authorization":"Bearer {0}".format(graatand)}).json()

    test.ensureError(response)

    ####
    # Data for creating Dalgaardsholmstuen
    test.new('Kurt tries to create department')
    response = requests.post(Test.url() + 'Department', json= {
        "id": 0,
        "name": dalgaardsholmstuen,
        "members": [ ],
        "resources": [ ]
    }, headers = {"Authorization":"Bearer {0}".format(kurt)}).json()

    test.ensureError(response)

    ####
    test.new('Make sure number of departments has remained the same')
    response = requests.get(Test.url() + 'Department').json()

    test.ensureSuccess(response)
    test.ensure(numberOfDepartments == len(response['data']))

    ####
    test.new('Lee creates department')
    response = requests.post(Test.url() + 'Department', json= {
        "id": 0,
        "name": dalgaardsholmstuen,
        "members": [ ],
        "resources": [ ]
    }, headers = {"Authorization":"Bearer {0}".format(lee)}).json()

    test.ensure(response['success'] is True, errormessage='Error: {0}'.format(response['errorKey']))

    dalgardsholmstuenId = response.get('data').get('id')

    test.ensure(dalgardsholmstuenId is not None, errormessage='Could not get ID of Dalgaardsholmstuen')

    ####
    test.new('Get the newly created Dalgaardsholmstuen')
    response = requests.get(Test.url() + 'Department/{0}'.format(dalgardsholmstuenId)).json()
    test.ensure(response['success'] is True, errormessage='Error: {0}'.format(response['errorKey']))
    test.ensure(response.get('data').get('name') == dalgaardsholmstuen,
                errormessage='Name should\'ve been {0} but was {1}'.format(dalgaardsholmstuen, response.get('name')))

    ####
    test.new('Get nonexistent department')
    response = requests.get(Test.url() + 'Department/-1').json()
    test.ensureError(response)

    ####
    test.new('Register Gunnar to Tobias\' department')
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))

    response = requests.post(Test.url() + 'account/register', json={
        "username": gunnarUsername,
        "password": "password",
        "departmentId":1
        }).json()

    test.ensureSuccess(response)

    gunnar = test.login(gunnarUsername)

    response = requests.get(Test.url() + 'User', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()

    gunnarID = response.get('data').get('id')

    test.ensure(gunnarID is not None)

    # Data that has Gunnars ID in it.
    body = {
        "role": -1,
        "guardians": None,
        "id": gunnarID,
        "username": gunnarUsername,
        "screenName": None,
        "userIcon": None,
        "department": -1,
        "weekScheduleIds": []
    }


    ####
    test.new('Kurt tries to add Gunnar to Dalgaardsholmstuen')
    response = requests.post(Test.url() + 'Department/user/{0}'.format(dalgardsholmstuenId), json=body, headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureError(response)

    ####
    test.new('Graatand tries to add Gunnar to Dalgaardsholmstuen')
    response = requests.post(Test.url() + 'Department/user/{0}'.format(dalgardsholmstuenId), json=body, headers = {"Authorization":"Bearer {0}".format(graatand)}).json()
    test.ensureError(response)

    ####
    test.new('Check that Gunnar has not changed departments yet')
    response = requests.get(Test.url() + 'User', headers = {"Authorization":"Bearer {0}".format(graatand)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data']['department'] == 1, 'Gunnar was moved to new department!')

    ####
    test.new('Gunnar moves himself to Dalgaardsholmstuen')
    response = requests.post(Test.url() + 'Department/user/{0}'.format(dalgardsholmstuenId), json=body, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)

    ####
    test.new('Check that Gunnar has in fact changed departments')
    response = requests.get(Test.url() + 'User', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data']['department'] == dalgardsholmstuenId, 'Gunnar was not moved to new department!')

    # TODO : Recomment when endpoint is fixed
    ####
    test.new('Kurt tries to remove Gunnar from Dalgaardsholmstuen')
    response = requests.delete(Test.url() + 'Department/user/{0}'.format(dalgardsholmstuenId), json=body, headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureError(response)

    # TODO : Recomment when endpoint is fixed
    ####
    test.new('Graatand tries to remove Gunnar from Dalgaardsholmstuen')
    response = requests.delete(Test.url() + 'Department/user/{0}'.format(dalgardsholmstuenId), json=body, headers = {"Authorization":"Bearer {0}".format(graatand)}).json()
    test.ensureError(response)

    ####
    test.new('Check that Gunnar has not changed departments yet')
    response = requests.get(Test.url() + 'User', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data']['department'] == dalgardsholmstuenId, 'Gunnar was moved to new department!')

    # TODO : Recomment when endpoint is fixed
    ####
    test.new('Gunnar removes himself from Dalgaardsholmstuen')
    response = requests.delete(Test.url() + 'Department/user/{0}'.format(dalgardsholmstuenId), json=body, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)

    ####
    test.new('Check that Gunnar was removed')
    response = requests.get(Test.url() + 'User', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data']['department'] != dalgardsholmstuenId, 'Gunnar was not moved to new department!')

    ####
    test.new('Post Cyclopian pictogram')
    body = {  
            "accessLevel": 0,  
            "title": "Cyclopian",  
            "id": -1,  
            "lastEdit": "2018-03-19T10:40:26.587Z"
        }
    response = requests.post(Test.url() + 'pictogram', json=body, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    cyclopianBody = {
            "accessLevel": 0,  
            "title": "Cyclopian",  
            "id": response['data']['id'],  
            "lastEdit": "2018-03-19T10:40:26.587Z"  
        } 
 
    ####
    test.new('Kurt tries to add Cyclopian to Dalgaardsholmstuen')
    response = requests.post(Test.url() + 'Department/resource/{0}'.format(dalgardsholmstuenId), json=body, headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureError(response)

    ####
    test.new('Add Cyclopian to Dalgaardsholmstuen')
    response = requests.post(Test.url() + 'Department/resource/{0}'.format(dalgardsholmstuenId), json=cyclopianBody, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)

    ####
    test.new('Kurt tries to remove Cyclopian from Dalgaardsholmstuen')
    response = requests.delete(Test.url() + 'Department/resource/{0}'.format(dalgardsholmstuenId), json=body, headers = {"Authorization":"Bearer {0}".format(kurt)})
    print('Department/resource/{0}'.format(dalgardsholmstuenId))
    test.ensureError(response)

    ####
    test.new('Remove Cyclopian from Dalgaardsholmstuen')
    response = requests.delete(Test.url() + 'Department/resource', json={**cyclopianBody, **{"id":dalgardsholmstuenId}}, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)