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

    @test()
    def logins(self, check):
        'Log in as Graatand, Kurt'
        self.graatand = login('Graatand', check)
        self.kurt = login('Kurt', check)

    gunnarUsername = None
    gunnar = None

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

    charlieUsername = None
    charlie = None

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
        response = requests.get(Test.url + 'User', headers=auth(self.charlie)).json()
        ensureSuccess(response, check)

    @test(skip_if_failed=['registerGunnar'])
    def userName(self, check):
        'Get Username'
        response = requests.get(Test.url + 'User/username',
                                headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.equal(response['data'], self.gunnarUsername)

    @test(skip_if_failed=['registerGunnar'])
    def userInfo(self, check):
        'Get User info'
        response = requests.get(Test.url + 'User', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], self.gunnarUsername)

    @test(skip_if_failed=['registerGunnar'])
    def unauthorizedUserInfo(self, check):
        'Gunnar tries to get Kurt\'s user info'
        response = requests.get(Test.url + 'User/Kurt', json={"username": "Kurt"},
                                headers=auth(self.gunnar)).json()
        ensureError(response, check)
        ensureNoData(response, check)

    @test(skip_if_failed=['registerGunnar'])
    def guardianUserInfo(self, check):
        'Graatand gets Kurt\'s user info'
        response = requests.get(Test.url + 'User/Kurt', headers=auth(self.graatand)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], 'Kurt');

    # TODO: Play with images(user avatar) when I figure out how

    @test(skip_if_failed=['registerGunnar'])   # TODO: This call is somehow incorrect.
    def setDisplayName(self, check):
        'Set display name'
        response = requests.put(Test.url + 'User/display-name',
                                json='HE WHO WAITS BEHIND THE WALL',
                                headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        # Check that display name was updated
        response = requests.get(Test.url + 'User', headers=auth(self.gunnar)).json()
        check.equal(response['data']['screenName'], 'HE WHO WAITS BEHIND THE WALL')

    wednesday = None
    wendesdayID = None
    wednesdayIDBody = None

    @test(skip_if_failed=['registerGunnar'])
    def newPictogram(self, check):
        'Post Wendesday pictogram'
        self.wednesday = 'Wednesday{0}'.format(str(time.time()))
        wednesdayBody = {
            "accessLevel": 2,
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

    @test(skip_if_failed=['newPictogram'])
    def giveCharliePictogram(self, check):
        # TODO: Is he allowed to do this?
        'Gunnar gives Charlie his Wednesday pictogram'
        response = requests.post(Test.url + 'User/{0}/resource/'.format(self.charlieUsername),
                                 json=self.wednesdayIDBody,
                                 headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

        check.is_true(hasPictogram(self.charlie, self.wednesdayID),
                      message='Charlie did not get Wednesday pictogram')

    @test(skip_if_failed=['newPictogram', 'giveCharliePictogram'])
    def removePictogram(self, check):
        'Remove wednesday pictogram'
        response = requests.delete(Test.url + 'User/resource',
                                   json=self.wednesdayIDBody,
                                   headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

        check.is_false(hasPictogram(self.gunnar, self.wednesdayID),
                       message='Gunnar still has Wednesday pictogram')

        check.is_true(hasPictogram(self.charlie, self.wednesdayID),
                      message='Charlie lost Wednesday pictogam as well')

    @test(skip_if_failed=['registerGunnar'])
    def settings(self, check):
        'Get settings'
        response = requests.get(Test.url + 'User/settings',
                                headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

    @test(skip_if_failed=['registerGunnar'])
    def settingsSetTheme(self, check):
        'Enable grayscale'
        response = requests.patch(Test.url + 'User/settings', json={"theme": 3}, headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar)).json()
        check.equal(3, response['data']['theme'])

    @test(skip_if_failed=['registerGunnar'])
    def settingsSetLauncherAnimationsOff(self, check):
        'Disable launcher animations'
        response = requests.patch(Test.url + 'User/settings', json={"displayLauncherAnimations": False}, headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar)).json()
        check.equal(False, response['data']['displayLauncherAnimations'])

    @test(skip_if_failed=['registerGunnar', 'settingsSetLauncherAnimationsOff'])
    def settingsSetLauncherAnimationsOn(self, check):
        'Enable launcher animations'
        response = requests.patch(Test.url + 'User/settings', json={"displayLauncherAnimations": True}, headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar)).json()
        check.equal(True, response['data']['displayLauncherAnimations'])

    @test(skip_if_failed=['registerGunnar'], depends=['settingsSetTheme', 'settingsSetLauncherAnimationsOn', 'settingsSetLauncherAnimationsOff']) # Run depends first, but if they fail, this can still run
    def settingsMultiple(self, check):
        'Set all settings'
        body = {
            "theme":                    3,
            "appGridSizeColumns":       1,
            "appGridSizeRows":          2,
            "displayLauncherAnimations":False,
            "orientation":              2,
            "completeMark":  2,
            "defaultTimer":             2,
            "timerSeconds":             3,
            "activitiesCount":          4
        }
        response = requests.patch(Test.url + 'User/settings', json=body, headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.equal(3,      response['data']["theme"]);
        check.equal(1,      response['data']["appGridSizeColumns"]);
        check.equal(2,      response['data']["appGridSizeRows"]);
        check.equal(False,  response['data']["displayLauncherAnimations"]);
        check.equal(2,      response['data']["orientation"]);
        check.equal(2,      response['data']["completeMark"]);
        check.equal(2,      response['data']["defaultTimer"]);
        check.equal(3,      response['data']["timerSeconds"]);
        check.equal(4,      response['data']["activitiesCount"]);

    @test(skip_if_failed=['logins'])
    def kurtCitizens(self, check):
        'Get Kurt\'s citizens(none)'
        response = requests.get(Test.url + 'User/Kurt/citizens',
                                headers=auth(self.kurt)).json()
        ensureError(response, check)

    @test(skip_if_failed=['logins'])
    def graatandCitizens(self, check):
        'Get Graatand\'s citizens(some)'
        response = requests.get(Test.url + 'User/Graatand/citizens',
                                headers=auth(self.graatand)).json()
        if ensureSuccess(response, check):
            check.equal('Kurt', response['data'][0]['userName'])

    @test(skip_if_failed=['logins'])
    def kurtGuardians(self, check):
        'Get Kurt\'s guardians(some)'
        response = requests.get(Test.url + 'User/Kurt/guardians',
                                headers=auth(self.kurt)).json()
        if ensureSuccess(response, check):
            check.is_true('Graatand', response['data'][0]['userName'])

    @test(skip_if_failed=['logins'])
    def graatandGuardians(self, check):
        'Get Graatand\'s guardians(none)'
        response = requests.get(Test.url + 'User/Graatand/guardians',
                                headers=auth(self.graatand)).json()
        ensureError(response, check)
