# -*- coding: utf-8 -*-
from testLib import *
import time
from rawSampleImage import *


def userControllerTest():
    test = controllerTest('User Controller')

    graatand = test.login('Graatand')
    kurt =     test.login('Kurt')

    ####
    test.newTest('Register Gunnar')
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
    test.ensureSuccess(response)

    gunnar = test.login(gunnarUsername)

    ####
    test.newTest('Register Charlie')
    charlieUsername = 'Charlie{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + charlieUsername + '","password": "password","departmentId": 1}')
    test.ensureSuccess(response)

    charlie = test.login(charlieUsername)
    response = test.request('GET', 'User', auth=charlie)

    ####
    test.newTest('Get Username')
    response = test.request('GET', 'User/username', auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response['data'] == gunnarUsername)

    ####
    test.newTest('Get User info')
    response = test.request('GET', 'User', auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response['data']['username'] == gunnarUsername)

    ####
    test.newTest('Gunnar tries to get Kurt\'s user info')
    response = test.request('GET', 'User/?username=Kurt', auth=gunnar)
    test.ensureError(response)
    test.ensureNoData(response)

    ####
    test.newTest('Graatand gets Kurt\'s user info')
    response = test.request('GET', 'User/Kurt', auth=graatand)
    test.ensureSuccess(response)
    test.ensure(response['data']['username'] == 'Kurt',
                errormessage='Expected name: Kurt.\nActual name: {0}'.format(response['data']['username']))

    # Example of how Gunnar's user info might look before being altered
    # {
    #    "role": 0,
    #    "roleName": "Citizen",
    #    "citizens": null,
    #    "guardians": null,
    #    "id": "40b2a546-a19a-4c5d-bc50-fb75e66ab484",
    #    "username": "Gunnar1522570015.67",
    #    "screenName": null,
    #    "userIcon": null,
    #    "department": 1,
    #    "weekScheduleIds": [],
    #    "resources": [],
    #    "settings": {
    #        "useGrayscale": false,
    #        "displayLauncherAnimations": false,
    #        "appsUserCanAccess": [],
    #        "appGridSizeRows": 0,
    #        "appGridSizeColumns": 0
    #    }
    # }

    ####
    test.newTest('Set Gunnars userinfo')
    body = '''
        {
            "role": 25,
            "roleName": "Guardian",
            "citizens": null,
            "guardians": [],
            "id": "aaaaaaaa-aaaa-AAaa-aaaa-aaaaaaAAAAAA",
            "username": "ROOT",
            "screenName": "ROOT",
            "department": 2,
            "weekScheduleIds": [],
            "resources": [],
            "settings": {
                "useGrayscale": true,
                "displayLauncherAnimations": true,
                "appsUserCanAccess": [],
                "appGridSizeRows": 11,
                "appGridSizeColumns": 22
            }
        }
    '''
    response = test.request('PUT', 'User', data=body, auth=gunnar)
    test.ensureSuccess(response)

    if response['success']:
        # Now, basically, only things I'm allowed to actually change are screenName and settings.
        response = test.request('GET', 'User', auth=gunnar)
        test.ensureEqual(0,              response['data']['role'])
        test.ensureEqual('Citizen',      response['data']['roleName'])
        test.ensureEqual(gunnarUsername, response['data']['username'])
        test.ensureEqual("Glade Gunnar", response['data']['screenName'])
        test.ensureEqual(1,              response['data']['department'])
        test.ensureNotEqual('aaaaaaaa-aaaa-AAaa-aaaa-aaaaaaAAAAAA', response['data']['id'])
        # Settings
        test.ensureEqual(True,  response['data']['settings']['useGrayscale'])
        test.ensureEqual(True,  response['data']['settings']['displayLauncherAnimations'])
        test.ensureEqual(11,    response['data']['settings']['appGridSizeRows'])
        test.ensure(22,         response['data']['settings']['appGridSizeColumns'])

    ####
    test.newTest('Try to set Graatand\'s user info as Gunnar')
    response = test.request('PUT', 'User/?id=8aa78445-c27b-41b8-b219-c31a50edd740', data=body, auth=gunnar)
    test.ensureError(response)

    # TODO: Play with images(user avatar) when I figure out how

    ####
    test.newTest('Set display name')
    response = test.request('PUT', 'User/display-name', data='"{0}"'.format(veryLongString), auth=gunnar)
    test.ensureSuccess(response)
    # Check that display name was updated
    response = test.request('GET', 'User', auth=gunnar)
    test.ensure(unicode(response['data']['screenName']) == unicode(veryLongString))

    ####
    test.newTest('Post Wendesday pictogram')
    wednesday = 'Wednesday{0}'.format(str(time.time()))
    wednesdayBody = '''
        {{
          "accessLevel": 2,
          "title": "{0}",
          "id": 5,
          "lastEdit": "2018-03-19T10:40:26.587Z"
        }}
    '''.format(wednesday)
    response = test.request('POST', 'Pictogram', data=wednesdayBody, auth=gunnar)
    test.ensureSuccess(response)
    wednesdayID = response['data']['id']

    ####
    test.newTest('Give Charlie pictogram')  # TODO: Is he allowed to do this?
    wednesdayIDBody = '''
        {{
          "id": {0}
        }}
    '''.format(wednesdayID)
    response = test.request('POST', 'User/resource/{0}'.format(charlieUsername), data=wednesdayIDBody, auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.newTest('Remove pictogram from gunnar')
    response = test.request('DELETE', 'User/resource/?username={0}'.format(gunnarUsername), data=wednesdayIDBody,
                            auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.newTest('Get settings')
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response['data']['useGrayscale'] is True)
    test.ensure(response['data']['displayLauncherAnimations'] is True)
    test.ensure(response['data']['appGridSizeRows'] == 11)
    test.ensure(response['data']['appGridSizeColumns'] == 22)

    ####
    test.newTest('Disable grayscale')
    response = test.request('POST', 'User/grayscale/false', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensure(response['data']['useGrayscale'] is False)

    ####
    test.newTest('Disable launcher animations')
    response = test.request('POST', 'User/launcher_animations/false', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensure(response['data']['displayLauncherAnimations'] is False)

    ####
    test.newTest('Set Settings')
    body = '''
            {
                "useGrayscale": false,
                "displayLauncherAnimations": false,
                "appsUserCanAccess": [],
                "appGridSizeRows": 33,
                "appGridSizeColumns": 44
            }'''
    response = test.request('PUT', 'User/Settings', data=body, auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureSuccess(response)
    test.ensureEqual(False,     response['data']['useGrayscale'])
    test.ensureEqual(False,     response['data']['displayLauncherAnimations'])
    test.ensureEqual(33,        response['data']['appGridSizeRows'])
    test.ensureEqual(44,        response['data']['appGridSizeColumns'])

    ####
    test.newTest('Set Settings(without data)')
    response = test.request('PUT', 'User/Settings',  auth=gunnar)
    test.ensureError(response)

    ####
    test.newTest('Set Settings(empty body)')
    response = test.request('PUT', 'User/Settings', '{}', auth=gunnar)
    test.ensureError(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureSuccess(response)
    test.ensureEqual(False,     response['data']['useGrayscale'])
    test.ensureEqual(False,     response['data']['displayLauncherAnimations'])
    test.ensureEqual(33,        response['data']['appGridSizeRows'])
    test.ensureEqual(44,        response['data']['appGridSizeColumns'])

    ####
    test.newTest('Get Kurt\'s citizens(none)')
    response = test.request('GET', 'User/getcitizens/Kurt', kurt)
    test.ensureError(response)

    ####
    test.newTest('Get Graatand\'s citizens(some)')
    response = test.request('GET', 'User/getcitizens/Graatand', graatand)
    test.ensureSuccess(response)
    if response['success']:
        test.ensure(response['data'][0]['username'] == 'Kurt')

    ####
    test.newTest('Get Kurt\'s guardians(some)')
    response = test.request('GET', 'User/getguardians/Kurt', kurt)
    test.ensureSuccess(response)
    if response['success']:
        test.ensure(response['data'][0]['username'] == 'Graatand')

    ####
    test.newTest('Get Graatand\'s guardians(none)')
    response = test.request('GET', 'User/getguardians/Graatand', graatand)
    test.ensureError(response)

    ####
    test.newTest('Try to get Graatand\'s citizens(some) as Gunnar')
    response = test.request('GET', 'User/getcitizens/Graatand', graatand)
    test.ensureError(response)
    ####
    test.newTest('Try to get Kurt\'s guardians(some) as Gunnar')
    response = test.request('GET', 'User/getguardians/Kurt', kurt)
    test.ensureError(response)

