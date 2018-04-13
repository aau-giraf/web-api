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
    def newGunnar(self, check):
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

    @test(skip_if_failed=['newGunnar'])
    def userName(self, check):
        'Get Username'
        response = requests.get(Test.url + 'User/username',
                                headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.is_true(response['data'] == self.gunnarUsername)

    @test(skip_if_failed=['newGunnar'])
    def userInfo(self, check):
        'Get User info'
        response = requests.get(Test.url + 'User', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.is_true(response['data']['username'] == self.gunnarUsername)

    @test(skip_if_failed=['newGunnar'], expect_fail=True)
    def unauthorizedUserInfo(self, check):
        'Gunnar tries to get Kurt\'s user info'
        response = requests.get(Test.url + 'User/username', json={"username": "Kurt"},
                                headers=auth(self.gunnar)).json()
        ensureError(response, check)
        ensureNoData(response, check)

    @test(skip_if_failed=['newGunnar'])
    def guardianUserInfo(self, check):
        'Graatand gets Kurt\'s user info'
        response = requests.get(Test.url + 'User/Kurt', headers=auth(self.graatand)).json()
        ensureSuccess(response, check)
        check.is_true(response['data']['username'] == 'Kurt',
                      message='Expected name: Kurt.\nActual name: {0}'.format(response['data']['username']))

    # TODO: Play with images(user avatar) when I figure out how

    @test(skip_if_failed=['newGunnar'], expect_fail=True)
    def setDisplayName(self, check):
        'Set display name'
        response = requests.put(Test.url + 'User/display-name',
                                data='HE WHO WAITS BEHIND THE WALL',
                                headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        # Check that display name was updated
        response = requests.get(Test.url + 'User', headers=auth(self.gunnar)).json()
        check.is_true(response['data']['screenName'] == 'HE WHO WAITS BEHIND THE WALL')

    wednesday = None
    wendesdayID = None
    wednesdayIDBody = None

    @test(skip_if_failed=['newGunnar'])
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

    @test(skip_if_failed=['newGunnar'])
    def settings(self, check):
        'Get settings'
        response = requests.get(Test.url + 'User/settings',
                                headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

    @test(skip_if_failed=['settings'])
    def grayscaleOff(self, check):
        'Disable grayscale'
        response = requests.post(Test.url + 'User/grayscale/false',
                                 headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar)).json()
        check.equal(False, response['data']['useGrayscale'])

    @test(skip_if_failed=['grayscaleOff'])
    def grayscaleOn(self, check):
        'Enable grayscale'
        response = requests.post(Test.url + 'User/grayscale/true', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar)).json()
        check.equal(True, response['data']['useGrayscale'])

    @test(skip_if_failed=['settings'])
    def animationsOff(self, check):
        'Disable launcher animations'
        response = requests.post(Test.url + 'User/launcher_animations/false', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar)).json()
        check.equal(False, response['data']['displayLauncherAnimations'])

    @test(skip_if_failed=['animationsOff'])
    def animationsOn(self, check):
        'Enable launcher animations'
        response = requests.post(Test.url + 'User/launcher_animations/true', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar)).json()
        check.equal(True, response['data']['displayLauncherAnimations'])

    @test(skip_if_failed=['settings'])
    def setEverything(self, check):
        'Set all settings'
        body = {
            "useGrayscale": True,
            "displayLauncherAnimations": False,
            "appsUserCanAccess": [],
            "appGridSizeRows": 33,
            "appGridSizeColumns": 44
        }
        response = requests.put(Test.url + 'User/settings', json=body, headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

        response = requests.get(Test.url + 'User/settings', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.equal(True, response['data']['useGrayscale'])
        check.equal(False, response['data']['displayLauncherAnimations'])
        check.equal(33, response['data']['appGridSizeRows'])
        check.equal(44, response['data']['appGridSizeColumns'])

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

    @test(skip_if_failed=['newGunnar'], expect_fail=True)
    def unauthorizedGraatandGuardians(self, check):
        'Try to get Graatand\'s citizens(some) as Gunnar'
        response = requests.get(Test.url + 'User/Graatand/citizens',
                                headers=auth(self.graatand)).json()
        ensureError(response, check)
        ensureNoData(response, check)

    @test(skip_if_failed=['newGunnar'], expect_fail=True)
    def unauthorizedKurtGuardians(self, check):
        'Try to get Kurt\'s guardians(some) as Gunnar'
        response = requests.get(Test.url + 'User/Kurt/guardians',
                                headers=auth(self.kurt)).json()
        ensureError(response, check)
        ensureNoData(response, check)
