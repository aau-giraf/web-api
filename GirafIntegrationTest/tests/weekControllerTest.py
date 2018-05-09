# -*- coding: utf-8 -*-
from testLib import *
from integrate import TestCase, test
import time


def auth(token):
    return {"Authorization": "Bearer {0}".format(token)}


def day(number, state=1):
    return {
        "day": number,
        "activities": [{
            "pictogram": {
                "title": "sig",
                "id": 4,
                "state": state,
                "lastEdit": "2018-03-28T10:47:51.628333",
                "accessLevel": 0
            },
            "order": 0
        }]
    }


def differentDay(number, state=3, pictogram=2):
    return {
        "day": number,
        "activities": [{
            "pictogram": {
                "title": "JUNK",
                "id": pictogram,
                "state": state,
                "lastEdit": "2017-03-28T10:47:51.628333",
                "accessLevel": 0
            },
            "order": 0
        }]
    }


def week(days):
    return {
        "thumbnail": {
            "title": "simpelt",
            "id": 5,
            "lastEdit": "2018-04-20T13:17:51.033Z",
            "accessLevel": 0
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

        for i in range(1, 8, 1):
            sevenDays.append(day(i))

        for i in range(1, 8, 1):
            differentSevenDays.append(differentDay(i))

        for i in range(1, 8, 1):
            eightDays.append(day(i))
        eightDays.append(day(3))     # To torsdage i en uge

        for i in range(1, 8, 1):
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

    @test(skip_if_failed=['newGunnar'])
    def getNoWeeks(self, check):
        'Get (empty)List of all weeks'
        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        ensureError(response, check)

    weekBody = None
    weekId = None

    @test(skip_if_failed=['newGunnar'])
    def newWeek(self, check):
        'Update week'
        response = requests.put(Test.url + 'Week/2018/11', headers=auth(self.gunnar), json=self.correctWeekDTO).json()
        ensureSuccess(response, check)
        self.weekYear = response['data']['weekYear']
        self.weekNumber = response['data']['weekNumber']

    @test(skip_if_failed=['newWeek'])
    def getWeek(self, check):
        'Get List of all weeks again, find our week'
        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)
        check.equal('The best week of the day', response['data'][0]['name'])
        self.weekYear = response['data'][0]['weekYear']
        self.weekNumber = response['data'][0]['weekNumber']

    @test(depends=['getNoWeeks', 'newWeek'])
    def newWeekTooManyDays(self, check):
        'Try to create week with too many weekdays'
        response = requests.put(Test.url + 'Week/2018/11', headers=auth(self.gunnar), json=self.tooManyDaysWeekDTO).json()
        ensureError(response, check)

        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        check.equal(1, len(response['data']))

    @test(depends=['getNoWeeks', 'newWeek'])
    def newWeekInvalidEnums(self, check):
        'Try to create week with invalid weekday enum values'
        response = requests.put(Test.url + 'Week/2018/11', headers=auth(self.gunnar), json=self.badEnumValueWeekDTO).json()
        ensureError(response, check)

        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        check.equal(1, len(response['data']))

    @test(skip_if_failed=['getWeek'])
    def changeWeek(self, check):
        'Update whole week at once'
        response = requests.put(Test.url + 'Week/{0}/{1}'.format(self.weekYear, self.weekNumber),
                                headers=auth(self.gunnar),
                                json=self.differentCorrectWeekDTO).json()
        ensureSuccess(response, check)

        response = requests.get(Test.url + 'Week/{0}/{1}'.format(self.weekYear, self.weekNumber), headers=auth(self.gunnar)).json()

        for i in range(1, 6, 1):
            check.equal(2, response['data']['days'][i]['activities'][0]['pictogram']['id'])

    @test(depends=['changeWeek'], skip_if_failed=['newWeek'])
    def deleteWeek(self, check):
        'Delete the week'
        response = requests.delete(Test.url + 'Week/{0}/{1}'.format(self.weekYear, self.weekNumber), headers=auth(self.gunnar)).json()
        ensureSuccess(response, check)

        response = requests.get(Test.url + 'Week', headers=auth(self.gunnar)).json()
        ensureError(response, check)