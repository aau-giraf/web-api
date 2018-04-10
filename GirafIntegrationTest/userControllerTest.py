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

    response = requests.post(Test.url() + 'account/register', json={
        "username": gunnarUsername,
        "password": "password",
        "departmentId": 1
        }).json()
    test.ensureSuccess(response)

    gunnar = test.login(gunnarUsername)

    ####
    test.new('Register Charlie')
    charlieUsername = 'Charlie{0}'.format(str(time.time()))
    response = requests.post(Test.url() + 'account/register', json={
        "username": charlieUsername,
        "password": "password",
        "departmentId": 1
        }).json()
    test.ensureSuccess(response)

    charlie = test.login(charlieUsername)
    response = requests.get(Test.url() + 'User', headers = {"Authorization":"Bearer {0}".format(charlie)}).json()

    ####
    test.new('Get Username')
    response = requests.get(Test.url() + 'User/username', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data'] == gunnarUsername)

    ####
    test.new('Get User info')
    response = requests.get(Test.url() + 'User', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data']['username'] == gunnarUsername)

    ####
    test.new('Gunnar tries to get Kurt\'s user info')
    response = requests.get(Test.url() + 'User/username', json={"username":"Kurt"}, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureError(response)
    test.ensureNoData(response)

    ####
    test.new('Graatand gets Kurt\'s user info')
    response = requests.get(Test.url() + 'User/Kurt', headers = {"Authorization":"Bearer {0}".format(graatand)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data']['username'] == 'Kurt',
                errormessage='Expected name: Kurt.\nActual name: {0}'.format(response['data']['username']))

    # TODO: Play with images(user avatar) when I figure out how

    ####
    test.new('Set display name')
    # PUTs a username, not ID

    # response = requests.put(Test.url() + 'User/Kurt', data=veryLongString, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    # test.ensureSuccess(response)
    # Check that display name was updated
    # response = test.request('GET', 'User', auth=gunnar)
    # test.ensure(unicode(response['data']['screenName']) == unicode(veryLongString))

    ####
    test.new('Post Wendesday pictogram')
    wednesday = 'Wednesday{0}'.format(str(time.time()))
    wednesdayBody = {
          "accessLevel": 2,
          "title": "wednesday",
          "id": 5,
          "lastEdit": "2018-03-19T10:40:26.587Z"
        }
    response = requests.post(Test.url() + 'Pictogram', json=wednesdayBody, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    wednesdayID = response['data']['id']

    ####
    test.new('Give Charlie pictogram')  # TODO: Is he allowed to do this?
    wednesdayIDBody = '''
        {{
          "id": {0}
        }}
    '''.format(wednesdayID)
    response = requests.post(Test.url() + 'User/resource/{0}'.format(charlieUsername), json=wednesdayIDBody, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)

    ####
    test.new('Remove pictogram from gunnar')
    response = requests.delete(Test.url() + 'User/resource', json= {
            "gunnarUsername": gunnarUsername
        }, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)

    ####
    test.new('Get settings')
    response = requests.get(Test.url() + 'User/settings', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)

    ####
    test.new('Disable grayscale')
    response = requests.post(Test.url() + 'User/grayscale/false', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    response = requests.get(Test.url() + 'User/settings', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureEqual(False, response['data']['useGrayscale'])

    ####
    test.new('Enable grayscale')
    response = requests.post(Test.url() + 'User/grayscale/true', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    response = requests.get(Test.url() + 'User/settings', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureEqual(True, response['data']['useGrayscale'])

    ####
    test.new('Disable launcher animations')
    response = requests.post(Test.url() + 'User/launcher_animations/false', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    response = requests.get(Test.url() + 'User/settings', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureEqual(False, response['data']['displayLauncherAnimations'])

    ####
    test.new('Enable launcher animations')
    response = requests.post(Test.url() + 'User/launcher_animations/true', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    response = requests.get(Test.url() + 'User/settings', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureEqual(True, response['data']['displayLauncherAnimations'])

    ####
    test.new('Set all settings')
    body =  {
                "useGrayscale": True,
                "displayLauncherAnimations": False,
                "appsUserCanAccess": [],
                "appGridSizeRows": 33,
                "appGridSizeColumns": 44
            }
    response = requests.put(Test.url() + 'User/settings', json=body, headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    response = requests.get(Test.url() + 'User/settings', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    test.ensureEqual(True,  response['data']['useGrayscale'])
    test.ensureEqual(False, response['data']['displayLauncherAnimations'])
    test.ensureEqual(33,    response['data']['appGridSizeRows'])
    test.ensureEqual(44,    response['data']['appGridSizeColumns'])

    ####
    test.new('Set Settings(without data)')
    response = requests.put(Test.url() + 'User/settings', headers = {"Authorization":"Bearer {0}".format(gunnar)})
    print(response.status_code)
    test.ensure(False, "Returns 404")
    # test.ensureError(response)

    ####
    test.new('Set Settings(empty body)')
    response = requests.put(Test.url() + 'User/settings', headers = {"Authorization":"Bearer {0}".format(gunnar)})
    print(response.status_code)
    test.ensure(False, "Returns 404")
    # test.ensureSuccess(response)
    response = requests.get(Test.url() + 'User/settings', headers = {"Authorization":"Bearer {0}".format(gunnar)}).json()
    test.ensureSuccess(response)
    test.ensureEqual(False,     response['data']['useGrayscale'])
    test.ensureEqual(False,     response['data']['displayLauncherAnimations'])
    test.ensureEqual(0,        response['data']['appGridSizeRows'])
    test.ensureEqual(0,        response['data']['appGridSizeColumns'])

    ####
    test.new('Get Kurt\'s citizens(none)')
    response = requests.get(Test.url() + 'User/Kurt/citizens', headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureError(response)

    ####
    test.new('Get Graatand\'s citizens(some)')
    response = requests.get(Test.url() + 'User/Graatand/citizens', headers = {"Authorization":"Bearer {0}".format(graatand)}).json()
    if test.ensureSuccess(response):
        test.ensure(response['data'][0]['username'] == 'Kurt')

    ####
    test.new('Get Kurt\'s guardians(some)')
    response = requests.get(Test.url() + 'User/Kurt/guardians', headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    if test.ensureSuccess(response):
        test.ensure(response['data'][0]['username'] == 'Graatand')

    ####
    test.new('Get Graatand\'s guardians(none)')
    response = requests.get(Test.url() + 'User/Graatand/guardians', headers = {"Authorization":"Bearer {0}".format(graatand)}).json()
    test.ensureError(response)

    ####
    test.new('Try to get Graatand\'s citizens(some) as Gunnar')
    response = requests.get(Test.url() + 'User/Graatand/citizens', headers = {"Authorization":"Bearer {0}".format(graatand)}).json()
    test.ensureError(response)

    ####
    test.new('Try to get Kurt\'s guardians(some) as Gunnar')
    response = requests.get(Test.url() + 'User/Kurt/guardians', headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureError(response)
