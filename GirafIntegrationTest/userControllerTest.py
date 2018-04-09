# -*- coding: utf-8 -*-
from testLib import *
import time
from rawSampleImage import *


def UserControllerTest():

    test = Test('User Controller')

    graatand = test.login('Graatand')
    kurt =     test.login('Kurt')

    ####
    test.new('Register Gunnar')
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
    test.ensureSuccess(response)

    gunnar = test.login(gunnarUsername)

    ####
    test.new('Register Charlie')
    charlieUsername = 'Charlie{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + charlieUsername + '","password": "password","departmentId": 1}')
    test.ensureSuccess(response)

    charlie = test.login(charlieUsername)
    response = test.request('GET', 'User', auth=charlie)

    ####
    test.new('Get Username')
    response = test.request('GET', 'User/username', auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response['data'] == gunnarUsername)

    ####
    test.new('Get User info')
    response = test.request('GET', 'User', auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response['data']['username'] == gunnarUsername)

    ####
    test.new('Gunnar tries to get Kurt\'s user info')
    response = test.request('GET', 'User/?username=Kurt', auth=gunnar)
    test.ensureError(response)
    test.ensureNoData(response)

    ####
    test.new('Graatand gets Kurt\'s user info')
    response = test.request('GET', 'User/Kurt', auth=graatand)
    test.ensureSuccess(response)
    test.ensure(response['data']['username'] == 'Kurt',
                errormessage='Expected name: Kurt.\nActual name: {0}'.format(response['data']['username']))

    # TODO: Play with images(user avatar) when I figure out how

    ####
    test.new('Set display name')
    response = test.request('PUT', 'User/display-name', data='"{0}"'.format(veryLongString), auth=gunnar)
    test.ensureSuccess(response)
    # Check that display name was updated
    response = test.request('GET', 'User', auth=gunnar)
    test.ensure(unicode(response['data']['screenName']) == unicode(veryLongString))

    ####
    test.new('Post Wendesday pictogram')
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
    test.new('Give Charlie pictogram')  # TODO: Is he allowed to do this?
    wednesdayIDBody = '''
        {{
          "id": {0}
        }}
    '''.format(wednesdayID)
    response = test.request('POST', 'User/resource/{0}'.format(charlieUsername), data=wednesdayIDBody, auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.new('Remove pictogram from gunnar')
    response = test.request('DELETE', 'User/resource/?username={0}'.format(gunnarUsername), data=wednesdayIDBody,
                            auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.new('Get settings')
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.new('Disable grayscale')
    response = test.request('POST', 'User/grayscale/false', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureEqual(False, response['data']['useGrayscale'])

    ####
    test.new('Enable grayscale')
    response = test.request('POST', 'User/grayscale/true', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureEqual(True, response['data']['useGrayscale'])

    ####
    test.new('Disable launcher animations')
    response = test.request('POST', 'User/launcher_animations/false', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureEqual(False, response['data']['displayLauncherAnimations'])

    ####
    test.new('Enable launcher animations')
    response = test.request('POST', 'User/launcher_animations/true', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureEqual(True, response['data']['displayLauncherAnimations'])

    ####
    test.new('Set all settings')
    body = '''
            {
                "useGrayscale": true,
                "displayLauncherAnimations": false,
                "appsUserCanAccess": [],
                "appGridSizeRows": 33,
                "appGridSizeColumns": 44
            }'''
    response = test.request('PUT', 'User/Settings', data=body, auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureSuccess(response)
    test.ensureEqual(True,  response['data']['useGrayscale'])
    test.ensureEqual(False, response['data']['displayLauncherAnimations'])
    test.ensureEqual(33,    response['data']['appGridSizeRows'])
    test.ensureEqual(44,    response['data']['appGridSizeColumns'])

    ####
    test.new('Set Settings(without data)')
    response = test.request('PUT', 'User/Settings',  auth=gunnar)
    test.ensureError(response)

    ####
    test.new('Set Settings(empty body)')
    response = test.request('PUT', 'User/Settings', '{}', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'User/settings', auth=gunnar)
    test.ensureSuccess(response)
    test.ensureEqual(False,     response['data']['useGrayscale'])
    test.ensureEqual(False,     response['data']['displayLauncherAnimations'])
    test.ensureEqual(0,        response['data']['appGridSizeRows'])
    test.ensureEqual(0,        response['data']['appGridSizeColumns'])

    ####
    test.new('Get Kurt\'s citizens(none)')
    response = test.request('GET', 'User/getcitizens/Kurt', kurt)
    test.ensureError(response)

    ####
    test.new('Get Graatand\'s citizens(some)')
    response = test.request('GET', 'User/getCitizens/Graatand', graatand)
    if test.ensureSuccess(response):
        test.ensure(response['data'][0]['username'] == 'Kurt')

    ####
    test.new('Get Kurt\'s guardians(some)')
    response = test.request('GET', 'User/getGuardians/Kurt', kurt)
    if test.ensureSuccess(response):
        test.ensure(response['data'][0]['username'] == 'Graatand')

    ####
    test.new('Get Graatand\'s guardians(none)')
    response = test.request('GET', 'User/getguardians/Graatand', graatand)
    test.ensureError(response)

    ####
    test.new('Try to get Graatand\'s citizens(some) as Gunnar')
    response = test.request('GET', 'User/getcitizens/Graatand', graatand)
    test.ensureError(response)

    ####
    test.new('Try to get Kurt\'s guardians(some) as Gunnar')
    response = test.request('GET', 'User/getguardians/Kurt', kurt)
    test.ensureError(response)

