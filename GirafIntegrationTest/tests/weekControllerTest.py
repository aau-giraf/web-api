# -*- coding: utf-8 -*-
from testLib import *
from integrate import TestCase, test
import time


def auth(token):
    return {"Authorization": "Bearer {0}".format(token)}


def day(number):
    return {
        "thumbnailID": 4,
        "elementIDs": [2],
        "day": number,
        "elements": [{
            "accessLevel": 0,
            "imageUrl": "/v1/pictogram/6/image/raw",
            "imageHash": "+8NDAclP6o11ft/Ba2yCZA==",
            "title": "sig",
            "id": 6,
            "lastEdit": "2018-03-28T10:47:51.628333"
        }]
    }


def differentDay(number):
    return {
        "thumbnailID": 4,
        "elementsSet": True,
        "elementIDs": [2],
        "day": number,
        "elements": [{
            "accessLevel": 0,
            "imageUrl": "/v1/pictogram/6/image/raw",
            "imageHash": "+8NDAclP6o11ft/Ba2yCZA==",
            "title": "JUNK",
            "id": 2,
            "lastEdit": "2018-03-28T10:47:51.628333"
        }]
    }


def week(days):
    return {
        "thumbnail": {
            "accessLevel": 0,
            "imageUrl": "junkdata",
            "imageHash": "junkdata",
            "title": "simpelt",
            "id": 5,
            "lastEdit": "2999-03-28T10:47:51.628376"
        },
        "name": "The best week of the day",
        "id": 0,
        "days": days
    }


class WeekControllerTest(TestCase):
    'Week Controller'

    correctWeekDTO = None
    differentCorrectWeekDTO = None
    tooManyDaysWeekDTO = None
    badEnumValueWeekDTO = None

    def setup_test(self):
        sevenDays = []
        differentSevenDays = []
        eightDays = []
        demonDays = []

        for i in range(0, 7, 1):
            sevenDays.append(day(i))

        for i in range(0, 7, 1):
            differentSevenDays.append(differentDay(i))

        for i in range(0, 7, 1):
            eightDays.append(day(i))
        eightDays.append(day(3))     # To torsdage i en uge

        for i in range(0, 7, 1):
            demonDays.append(day(i * 10))

        self.differentCorrectWeekDTO = week(differentSevenDays)

        self.correctWeekDTO = week(sevenDays)
        self.tooManyDaysWeekDTO = week(eightDays)
        self.badEnumValueWeekDTO = week(demonDays)

    gunnarUsername = None
    gunnar = None

    @test()
    def newGunnar(self, check):
        'Register Gunnar'
        self.gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
        response = requests.post(Test.url + 'account/register',
                                 json={"username": self.gunnarUsername, "password": "password",
                                       "departmentId": 1}).json()
        ensureSuccess(response, check)

        self.gunnar = login(self.gunnarUsername, check)

        response = requests.get(Test.url + 'User', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

    @test()
    def getNoWeeks(self, check):
        'Get (empty)List of all weeks'
        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        ensureError(response, check)

    weekBody = None

    @test(skip_if_failed=['newGunnar'])
    def newWeek(self, check):
        'Create new week'
        response = requests.post(Test.url + 'Week', headers=auth(self.gunnar), json=self.correctWeekDTO).json()
        ensureSuccess(response, check)

    weekID = None

    @test(skip_if_failed=['newWeek'])
    def getWeek(self, check):
        'Get List of all weeks again, find our week'
        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.equal('The best week of the day', response['data'][0]['name'])
        self.weekID = response['data'][0]['id']

    @test(depends=['getNoWeeks', 'newWeek'], expect_fail=True)
    def newWeekTooManyDays(self, check):
        'Try to create week with too many weekdays'
        response = requests.post(Test.url + 'Week', headers=auth(self.gunnar), json=self.tooManyDaysWeekDTO).json()
        ensureError(response, check)

        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        ensureError(response, check)

    @test(depends=['getNoWeeks', 'newWeek'], expect_fail=True)
    def newWeekInvalidEnums(self, check):
        'Try to create week with invalid weekday enum values'
        response = requests.post(Test.url + 'Week', headers=auth(self.gunnar), json=self.badEnumValueWeekDTO).json()
        ensureError(response, check)

        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        ensureError(response, check)

    @test(skip_if_failed=['getWeek'])
    def changeWeek(self, check):
        'Update whole week at once'
        response = requests.put(Test.url + 'Week/{0}'.format(self.weekID),
                                headers=auth(self.gunnar),
                                json=self.differentCorrectWeekDTO).json()
        ensureSuccess(response, check)

        response = requests.get(Test.url + 'Week/{0}'.format(self.weekID), headers=auth(self.gunnar)).json()

        for i in range(1, 6, 1):
            check.equal(2, response['data']['days'][i]['elements'][0]['id'])

    @test(skip_if_failed=['getWeek'])
    def changeDay(self, check):
        'Update single day(Day Controller is not getting its own file.)'
        someOtherDay = {
            "elementsSet": True,
            "elementIDs": [3],
            "day": "Wednesday",
            "elements": []
        }

        response = requests.put(Test.url + 'Day/{0}'.format(self.weekID),
                                headers=auth(self.gunnar),
                                json=someOtherDay).json()
        ensureSuccess(response, check)
        response = requests.get(Test.url + 'Week/{0}'.format(self.weekID), headers=auth(self.gunnar)).json()
        wednesdayIndex = 2
        for i in range(1, 6, 1):
            if wednesdayIndex == response['data']['days'][i]['day']:
                check.equal(3, response['data']['days'][i]['elements'][0]['id'])

    @test(depends=['changeDay', 'changeWeek'], skip_if_failed=['newWeek'])
    def deleteWeek(self, check):
        'Delete the week'
        response = requests.delete(Test.url + 'Week/{0}'.format(self.weekID), headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        ensureError(response, check)
