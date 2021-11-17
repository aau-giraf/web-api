import json

from requests import get, post, put, delete
import time
from testlib import order, BASEURL, auth, isSequence, GIRAFTestCase
from http import HTTPStatus

citizenUsername = f'Gunnar{time.time()}'
citizenId = ''
citizenToken = ''
weekYear = 0
weekNumber = 0
superUserToken = ''


class TestWeekController(GIRAFTestCase):
    """
    Testing API requests on Week endpoints
    """

    @classmethod
    def SetUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestWeekController, cls).setUpClass()
        print(f'file:/{_file_}\n')

    @classmethod
    def TearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestWeekController, cls).tearDownClass()

    def SetUp(self) -> None:
        """
        Setup necessary data and states before each test
        """
        self.correctWeek = self.week([self.day(i) for i in range(1, 8)])
        self.differentCorrectWeek = self.week([self.differentDay(i) for i in range(1, 8)])
        self.tooManyDaysWeek = self.week([self.day(i) for i in range(1, 8)] + [self.day(3)])
        self.badEnumValueWeek = self.week([self.day(i * 10) for i in range(1, 8)])

    @staticmethod
    def Day(dayNumber: int) -> dict:
        return {
            'day': dayNumber,
            'activities': [
                {
                    'pictograms': [
                        {
                            'title': 'sig',
                            'id': 4,
                            'state': 1,
                            'lastEdit': '2018-03-28T10:47:51.628333',
                            'accessLevel': 0
                        }
                    ],
                    'order': 0,
                }
            ]
        }

    @staticmethod
    def DifferentDay(dayNumber: int) -> dict:
        return {
            'day': dayNumber,
            'activities': [
                {
                    'pictograms': [
                        {
                            'title': 'JUNK',
                            'id': 2,
                            'state': 3,
                            'lastEdit': '2017-03-28T10:47:51.628333',
                            'accessLevel': 0
                        }
                    ],
                    'order': 0
                }
            ]
        }

    @staticmethod
    def Week(days: list) -> dict:
        return {
            'thumbnail': {
                'title': 'simpelt',
                'id': 5,
                'lastEdit': '2018-04-20T13:17:51.033Z',
                'accessLevel': 0
            },
            'name': 'Coronabots roll out',
            'id': 0, 'days': days
        }

    @order
    def TestWeekCanLoginAsSuperUser(self):
        """
        Testing logging in as Super User

        Endpoint: POST:/v2/Account/login
        """
        global superUserToken
        data = {'username': 'Lee', 'password': 'password'}
        response = post(f'{BASEURL}v2/Account/login', json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])
        superUserToken = responseBody['data']

    @order
    def TestWeekCanRegisterCitizen(self):
        """
        Testing registering Citizen

        Endpoint: POST:/v2/Account/register
        """
        data = {'username': citizenUsername, 'displayname': citizenUsername, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASEURL}v2/Account/register', headers=auth(superUserToken), json=data)
        
        self.assertEqual(response.statusCode, HTTPStatus.CREATED)

    @order
    def TestWeekCanLoginAsCitizen(self):
        """
        Testing logging in as Citizen

        Endpoint: POST:/v2/Account/login
        """
        global citizenToken
        data = {'username': citizenUsername, 'password': 'password'}
        response = post(f'{BASEURL}v2/Account/login', json=data)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])
        citizenToken = responseBody['data']

    @order
    def TestWeekCanGetCitizenId(self):
        """
        Testing getting Citizen's id

        Endpoint: GET:/v1/User
        """
        global citizenId
        response = get(f'{BASEURL}v1/User', headers=auth(citizenToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])
        self.assertIsNotNone(responseBody['data']['id'])
        citizenId = responseBody['data']['id']

    @order
    def TestWeekCanGetNoWeeks(self):
        """
        Testing getting empty list of weeks

        Endpoint: GET:/v1/Week/{userId}/weekName
        """
        response = get(f'{BASEURL}v1/Week/{citizenId}/weekName', headers=auth(citizenToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertFalse(responseBody['data'])
        self.assertTrue(isSequence(responseBody['data']))

    @order
    def TestWeekCanAddWeek(self):
        """
        Testing adding week

        Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
        """
        global weekYear, weekNumber
        response = put(f'{BASEURL}v1/Week/{citizenId}/2018/11', headers=auth(superUserToken),
                       json=self.correctWeek)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(2018, responseBody['data']['weekYear'])
        self.assertEqual(11, responseBody['data']['weekNumber'])
        weekYear = responseBody['data']['weekYear']
        weekNumber = responseBody['data']['weekNumber']

    @order
    def TestWeekCanGetNewWeeks(self):
        """
        Testing getting list containing new week

        Endpoint: GET:/v1/Week/{userId}/weekName
        """
        response = get(f'{BASEURL}v1/Week/{citizenId}/weekName', headers=auth(citizenToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(2018, responseBody['data'][0]['weekYear'])
        self.assertEqual(11, responseBody['data'][0]['weekNumber'])

    @order
    def TestWeekCanGetNewWeeksNewV2Endpoint(self):
        """
        Testing getting list containing new week v2 endpoint

        Endpoint: GET:/v1/Week/{userId}/week
        """
        response = get(f'{BASEURL}v1/Week/{citizenId}/week', headers=auth(citizenToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(2018, responseBody['data'][0]['weekYear'])
        self.assertEqual(11, responseBody['data'][0]['weekNumber'])

    @order
    def TestWeekCanAddWeekWithTooManyDaysShouldFail(self):
        """
        Testing adding week containing too many days

        Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
        """
        response = put(f'{BASEURL}v1/Week/{citizenId}/2018/12', headers=auth(superUserToken),
                       json=self.tooManyDaysWeek)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.BADREQUEST)
        self.assertEqual(responseBody['errorKey'], 'InvalidAmountOfWeekdays')

    @order
    def TestWeekEnsureWeekWithTooManyDaysNotAdded(self):
        """
        Testing ensuring week containing too many days was not added

        Endpoint: GET:/v1/Week/{userId}/weekName
        """
        response = get(f'{BASEURL}v1/Week/{citizenId}/weekName', headers=auth(citizenToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(1, len(responseBody['data']))

    @order
    def TestWeekCanAddWeekWithInvalidEnumsShouldFail(self):
        """
        Testing adding new week with invalid enums

        Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
        """
        response = put(f'{BASEURL}v1/Week/{citizenId}/2018/13', headers=auth(superUserToken),
                       json=self.badEnumValueWeek)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.BADREQUEST)
        self.assertEqual(responseBody['errorKey'], 'InvalidDay')

    @order
    def TestWeekEnsureWeekWithInvalidEnumsNotAdded(self):
        """
        Testing ensuring week with invalid enums was not added

        Endpoint: GET:/v1/Week/{userId}/weekName
        """
        response = get(f'{BASEURL}v1/Week/{citizenId}/weekName', headers=auth(citizenToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])
        self.assertEqual(1, len(responseBody['data']))

    @order
    def TestWeekCanUpdateWeek(self):
        """
        Testing updating week

        Endpoint: PUT:/v1/Week/{userId}/{weekYear}/{weekNumber}
        """
        response = put(f'{BASEURL}v1/Week/{citizenId}/{weekYear}/{weekNumber}', headers=auth(superUserToken),
                       json=self.differentCorrectWeek)
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])

    @order
    def TestWeekEnsureWeekIsUpdated(self):
        """
        Testing ensuring week has been updated

        Endpoint: GET:/v1/Week/{userId}/{weekYear}/{weekNumber}
        """
        response = get(f'{BASEURL}v1/Week/{citizenId}/{weekYear}/{weekNumber}',
                       headers=auth(citizenToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertIsNotNone(responseBody['data'])
        self.assertTrue(responseBody['data']['days'][i]['activities'][0]['pictogram']['id'] == 2 for i in range(1, 6))

    @order
    def TestWeekCanDeleteWeek(self):
        """
        Testing deleting week

        Endpoint: DELETE:/v1/Week/{userId}/{weekYear}/{weekNumber}
        """
        response = delete(f'{BASEURL}v1/Week/{citizenId}/{weekYear}/{weekNumber}',
                          headers=auth(superUserToken))
        
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        
    @order
    def TestWeekEnsureWeekDeleted(self):
        """
        Testing ensuring week has been deleted

        Endpoint: GET:/v1/Week/{userId}/weekName
        """
        response = get(f'{BASEURL}v1/Week/{citizenId}/weekName', headers=auth(citizenToken))
        responseBody = response.json()
        self.assertEqual(response.statusCode, HTTPStatus.OK)
        self.assertFalse(responseBody['data'])
        self.assertTrue(isSequence(responseBody['data']))
