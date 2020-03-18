# -*- coding: utf-8 -*-
import LargeData
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
    lee = None
    tobias = None
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

    @test()
    def loginAsLee(self, check):
        'Log in as Lee'
        self.lee = login('Lee', check)

    @test()
    def loginAsTobias(self, check):
        'Log in as Tobias'
        self.tobias = login('Tobias', check)

    @test(skip_if_failed=['loginAsKurt'])
    def GetKurtID(self, check):
        'Get User info for kurt'
        response = requests.get(Test.url + 'User', headers=auth(self.kurt)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], 'Kurt')
        self.kurtId = response['data']['id']

    @test(skip_if_failed=['loginAsGraatand'])
    def GetGraatandID(self, check):
        'Get User info graatand'
        response = requests.get(Test.url + 'User', headers=auth(self.graatand)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], 'Graatand')
        self.graatandId = response['data']['id']

    @test()
    def registerGunnar(self, check):
        'Register Gunnar'
        self.gunnarUsername = 'Gunnar{0}'.format(str(time.time()))

        response = requests.post(Test.url + 'account/register', headers=auth(self.graatand), json={
            "username": self.gunnarUsername,
            "password": "password",
            "role": "Citizen",
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
        response = requests.post(Test.url + 'account/register', headers=auth(self.graatand), json={
            "username": self.charlieUsername,
            "password": "password",
            "role": "Citizen",
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
    def getCharlieID(self, check):
        'Get User info'
        response = requests.get(Test.url + 'User', headers=auth(self.charlie)).json()
        ensureSuccess(response, check)
        check.equal(response['data']['username'], self.charlieUsername)
        self.charlieId = response['data']['id']

    @test(skip_if_failed=['newCharlie', 'getCharlieID'])
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
        check.equal(response['data']['username'], self.gunnarUsername);

    # TODO: Play with images(user avatar) when I figure out how

    @test(skip_if_failed=['registerGunnar', 'userInfo'])
    def setDisplayName(self, check):
        'Set display name'
        response = requests.put(Test.url + 'User/{0}'.format(self.gunnarId),
                                json={"userName": self.gunnarUsername, "screenName": "HE WHO WAITS BEHIND THE WALL"},
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
                                 headers=auth(self.gunnar)).json()

        ensureError(response, check)
        check.is_false(hasPictogram(self.charlie, self.wednesdayID))

    @test(skip_if_failed=['registerGunnar', 'userInfo'])
    def settings(self, check):
        'Get settings'
        response = requests.get(Test.url + 'User/{0}/settings'.format(self.gunnarId),
                                headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

    @test(skip_if_failed=['registerGunnar', 'userInfo'])
    def settingsSetTheme(self, check):
        'Enable grayscale'
        response = requests.put(Test.url + 'User/{0}/settings'.format(self.gunnarId), json=LargeData.grayscaleSettings, headers=auth(self.graatand)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/{0}/settings'.format(self.gunnarId), headers=auth(self.graatand)).json()
        check.equal(LargeData.grayscaleSettings['theme'], response['data']['theme'])

    @test(skip_if_failed=['registerGunnar', 'userInfo'])
    def settingsSetTimerSeconds(self, check):
        'Set default countdown time'
        response = requests.put(Test.url + 'User/{0}/settings'.format(self.gunnarId), json=LargeData.timer1HourSettings,
                                headers=auth(self.graatand)).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'User/{0}/settings'.format(self.gunnarId), headers=auth(self.graatand)).json()
        check.equal(LargeData.timer1HourSettings['timerSeconds'], response['data']['timerSeconds'])

    @test(skip_if_failed=['registerGunnar', 'userInfo'], depends=['settingsSetTheme', 'settingsSetLauncherAnimationsOn',
                                                                  'settingsSetLauncherAnimationsOff'])
    def settingsMultiple(self, check):
        'Set all settings'
        response = requests.put(Test.url + 'User/{0}/settings'.format(self.gunnarId), json=LargeData.allSettings, headers=auth(self.graatand)).json()
        ensureSuccess(response, check)

        response = requests.get(Test.url + 'User/{0}/settings'.format(self.gunnarId), headers=auth(self.graatand)).json()
        ensureSuccess(response, check)
        check.equal(2, response['data']['orientation'])
        check.equal(2, response['data']['completeMark'])
        check.equal(1, response['data']['cancelMark'])
        check.equal(2, response['data']['defaultTimer'])
        check.equal(60, response['data']['timerSeconds'])
        check.equal(3, response['data']['activitiesCount'])
        check.equal(3, response['data']['theme'])
        check.equal(2, response['data']['nrOfDaysToDisplay'])
        check.equal(True, response['data']['greyScale'])
        check.equal("#FF00FF", response['data']['weekDayColors'][0]['hexColor'])
        check.equal(1, response['data']['weekDayColors'][0]['day'])

    @test(skip_if_failed=['GetKurtID'])
    def kurtCitizens(self, check):
        'Get Kurt\'s citizens'
        response = requests.get(Test.url + 'User/{0}/citizens'.format(self.kurtId),
                                headers=auth(self.kurt)).json()
        ensureError(response, check)

    @test(skip_if_failed=['GetGraatandID'])
    def graatandCitizens(self, check):
        'Get Graatand\'s citizens'
        response = requests.get(Test.url + 'User/{0}/citizens'.format(self.graatandId),
                                headers=auth(self.graatand)).json()
        ensureSuccess(response, check)
        check.equal('Kurt', response['data'][0]['userName'])

    @test(skip_if_failed=['GetKurtID'])
    def kurtGuardians(self, check):
        'Get Kurt\'s guardians'
        response = requests.get(Test.url + 'User/{0}/guardians'.format(self.kurtId),
                                headers=auth(self.kurt)).json()
        ensureSuccess(response, check)
        check.is_true('Graatand', response['data'][0]['userName'])

    @test(skip_if_failed=['GetGraatandID'])
    def graatandGuardians(self, check):
        'Get Graatand\'s guardians'
        response = requests.get(Test.url + 'User/{0}/guardians'.format(self.graatandId),
                                headers=auth(self.graatand)).json()
        ensureError(response, check)


    @test(skip_if_failed=['loginAsKurt', 'GetKurtID'])
    def citizenCantChangeSettings(self, check):
        'Checking citizen can\'t change settings'
        response = requests.put(Test.url + 'User/{0}/settings'.format(self.kurtId), json=LargeData.allSettings,
                               headers=auth(self.kurt)).json()
        ensureError(response, check)

    @test(skip_if_failed=['registerGunnar','loginAsLee', 'settingsMultiple'])
    def superUserChangeCitizenSetting(self, check):
        'Checking superUser can change citizens setting'
        response = requests.put(Test.url + 'User/{0}/settings'.format(self.gunnarId), json=LargeData.updatedSettings1,
                                headers=auth(self.lee)).json()
        ensureSuccess(response, check)
        check.equal(1, response['data']['theme'])


    @test(skip_if_failed=['registerGunnar','loginAsTobias', 'settingsMultiple'])
    def departmentChangeCitizenSetting(self, check):
        'Checking department can change citizens setting'
        response = requests.put(Test.url + 'User/{0}/settings'.format(self.gunnarId), json=LargeData.updatedSettings2,
                                headers=auth(self.tobias)).json()
        ensureSuccess(response, check)
        check.equal(2, response['data']['theme'])