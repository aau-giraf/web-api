# -*- coding: utf-8 -*-
from testLib import *
from integrate import TestCase, test
import time


def auth(token):
    return {"Authorization": "Bearer {0}".format(token)}


def hasPictogram(userToken, pictogramID):
    response = requests.get(Test.url + 'Pictogram/{0}'.format(pictogramID),
                            headers=auth(userToken)).json()
    return response['success']


class UserControllerTest(TestCase):
    'User Controller'
    graatand = None
    kurt = None
    gunnarUsername = None
    gunnar = None
    charlieUsername = None
    charlie = None
    wednesday = None
    wendesdayID = None
    wednesdayIDBody = None

    @test()
    def loginAsKurt(self, check):
        'Log in as Kurt'
        self.kurt = login('Kurt', check)

    @test()
    def loginAsGraatand(self, check):
        'Log in as Graatand'
        self.graatand = login('Graatand', check)

    @test(skip_if_failed=['loginAsKurt'])
    def GetKurt(self, check):
        'Get User info for kurt'
        response = requests.get(Test.url + 'User', headers=auth(self.kurt)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], 'Kurt')
        self.kurtId = response['data']['id']

    @test(skip_if_failed=['loginAsGraatand'])
    def GetGraatand(self, check):
        'Get User info graatand'
        response = requests.get(Test.url + 'User', headers=auth(self.graatand)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], 'Graatand')
        self.graatandId = response['data']['id']

    @test()
    def registerGunnar(self, check):
        'Register Gunnar'
        self.gunnarUsername = 'Gunnar{0}'.format(str(time.time()))

        response = requests.post(Test.url + 'account/register', json={
            "username": self.gunnarUsername,
            "password": "password",
            "departmentId": 1
        }).json()

        ensureSuccess(response, check)

        self.gunnar = login(self.gunnarUsername, check)

        response = requests.get(Test.url + 'User', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

    @test()
    def newCharlie(self, check):
        'Register Charlie'
        self.charlieUsername = 'Charlie{0}'.format(str(time.time()))
        response = requests.post(Test.url + 'account/register', json={
            "username": self.charlieUsername,
            "password": "password",
            "departmentId": 1
        }).json()
        ensureSuccess(response, check)

        self.charlie = login(self.charlieUsername, check)

    @test(skip_if_failed=['registerGunnar'])
    def userInfo(self, check):
        'Get User info'
        response = requests.get(Test.url + 'User/', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], self.gunnarUsername)
        self.gunnarId = response['data']['id']

    @test(skip_if_failed=['newCharlie'])
    def getCharlieInfo(self, check):
        'Get User info'
        response = requests.get(Test.url + 'User' , headers=auth(self.charlie)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], self.charlieUsername)
        self.charlieId = response['data']['id']

    @test(skip_if_failed=['newCharlie', 'getCharlieInfo'])
    def unauthorizedUserInfo(self, check):
        'Gunnar tries to get charlies user info'
        response = requests.get(Test.url + 'User/' + self.charlieId,
                                headers=auth(self.gunnar)).json()
        ensureError(response, check)
        ensureNoData(response, check)

    @test(skip_if_failed=['registerGunnar', 'userInfo'])
    def guardianUserInfo(self, check):
        'Graatand gets gunnars user info'
        response = requests.get(Test.url + 'User/' + self.gunnarId, headers=auth(self.graatand)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], 'Gunnar');

    # TODO: Play with images(user avatar) when I figure out how

    @test(skip_if_failed=['registerGunnar', 'userInfo'])   # TODO: This call is somehow incorrect.
    def setDisplayName(self, check):
        'Set display name'
        response = requests.put(Test.url + 'User/{0}display-name'.format(self.gunnarId),
                                json='HE WHO WAITS BEHIND THE WALL',
                                headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        # Check that display name was updated
        response = requests.get(Test.url + 'User', headers=auth(self.gunnar)).json()
        check.equal(response['data']['screenName'], 'HE WHO WAITS BEHIND THE WALL')


    @test(skip_if_failed=['registerGunnar'])
    def newPictogram(self, check):
        'Post Wendesday pictogram'
        self.wednesday = 'Wednesday{0}'.format(str(time.time()))
        wednesdayBody = {
            "accessLevel": 3,
            "title": "wednesday",
            "id": 5,
            "lastEdit": "2018-03-19T10:40:26.587Z"
        }
        response = requests.post(Test.url + 'Pictogram', json=wednesdayBody,
                                 headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        self.wednesdayID = response['data']['id']
        self.wednesdayIDBody = {
            "id": self.wednesdayID
        }

    @test(skip_if_failed=['newPictogram', 'newCharlie'])
    def giveCharliePictogram(self, check):
        # TODO: Is he allowed to do this?
        'Gunnar gives Charlie his Wednesday pictogram'
        response = requests.post(Test.url + 'User/{0}/resource/'.format(self.charlieId),
                                 json=self.wednesdayIDBody,
                                 headers=auth(self.charlie)).json()
        ensureSuccess(response, check)

        check.is_true(hasPictogram(self.charlie, self.wednesdayID),
                      message='Charlie did not get Wednesday pictogram')

    @test(skip_if_failed=['newPictogram', 'giveCharliePictogram'])
    def removePictogram(self, check):
        'Remove wednesday pictogram'
        response = requests.delete(Test.url + 'User/{0}/resource'.format(self.charlieId),
                                   json=self.wednesdayIDBody,
                                   headers=auth(self.charlie)).json()
        ensureSuccess(response, check)

        check.is_false(hasPictogram(self.gunnar, self.wednesdayID),
                       message='Gunnar still has Wednesday pictogram')

        check.is_true(hasPictogram(self.charlie, self.wednesdayID),
                      message='Charlie lost Wednesday pictogam as well')

    @test(skip_if_failed=['registerGunnar', 'userInfo'])
    def settings(self, check):
        'Get settings'
        response = requests.get(Test.url + 'User/{0}/settings'.format(self.gunnarId),
                                headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

    @test(skip_if_failed=['registerGunnar', 'userInfo'])
    def settingsSetTheme(self, check):
        'Enable grayscale'
        response = requests.put(Test.url + 'User/settings', json={"theme": 3}, headers=auth(self.gunnar))
        ensureSuccess(response.json(), check)
        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar))
        check.equal(3, response.json()['data']['theme'])

    @test(skip_if_failed=['registerGunnar', 'userInfo'])
    def settingsSetTimerSeconds(self, check):
        'Set default countdown time'
        response = requests.put(Test.url + 'User/settings', json={"timerSeconds": 3600}, headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/{0}/settings'.format(self.gunnarId), headers=auth(self.gunnar)).json()
        check.equal(3600, response['data']['timerSeconds'])

    @test(skip_if_failed=['registerGunnar', 'userInfo'], depends=['settingsSetTheme', 'settingsSetLauncherAnimationsOn', 'settingsSetLauncherAnimationsOff']) # Run depends first, but if they fail, this can still run
    def settingsMultiple(self, check):
        'Set all settings'
        body = {
                "orientation": 2,
                "completeMark": 2,
                "cancelMark": 1,
                "defaultTimer": 2,
                "timerSeconds": 60,
                "activitiesCount": 3,
                "theme": 3,
                "nrOfDaysToDisplay": 2,
                "greyScale": True,
                "weekDayColors": [
                    {
                        "hexColor": "#FF00FF",
                        "day": 1
                    }
                ]
        }
        response = requests.put(Test.url + 'User/settings', json=body, headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

        response = requests.get(Test.url + 'User/{0}/settings'.format(self.gunnarId), headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.equal(2,          response['data']['orientation'])
        check.equal(2,          response['data']['completeMark'])
        check.equal(1,          response['data']['cancelMark'])
        check.equal(2,          response['data']['defaultTimer'])
        check.equal(60,         response['data']['timerSeconds'])
        check.equal(3,          response['data']['activitiesCount'])
        check.equal(3,          response['data']['theme'])
        check.equal(2,          response['data']['nrOfDaysToDisplay'])
        check.equal(True,       response['data']['greyScale'])
        # THIS WORKS, BUT IS CONFUSING
        check.equal("#FF00FF",  response['data']['weekDayColors'][0]['hexColor'])
        check.equal(1,          response['data']['weekDayColors'][0]['day'])

    @test(skip_if_failed=['loginAsKurt'])
    def kurtCitizens(self, check):
        'Get Kurt\'s citizens(none)'
        response = requests.get(Test.url + 'User/{0}/citizens'.format(self.kurtId),
                                headers=auth(self.kurt)).json()
        ensureError(response, check)

    @test(skip_if_failed=['loginAsGraatand'])
    def graatandCitizens(self, check):
        'Get Graatand\'s citizens(some)'
        response = requests.get(Test.url + 'User/{0}/citizens'.format(self.graatandId),
                                headers=auth(self.graatand)).json()
        if ensureSuccess(response, check):
            check.equal('Kurt', response['data'][0]['userName'])

    @test(skip_if_failed=['loginAsKurt'])
    def kurtGuardians(self, check):
        'Get Kurt\'s guardians(some)'
        response = requests.get(Test.url + 'User/{0}/guardians'.format(self.kurtId),
                                headers=auth(self.kurt)).json()
        if ensureSuccess(response, check):
            check.is_true('Graatand', response['data'][0]['userName'])

    @test(skip_if_failed=['loginAsGraatand'])
    def graatandGuardians(self, check):
        'Get Graatand\'s guardians(none)'
        response = requests.get(Test.url + 'User/{0}/guardians'.format(self.graatandId),
                                headers=auth(self.graatand)).json()
        ensureError(response, check)
