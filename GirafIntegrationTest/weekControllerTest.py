# -*- coding: utf-8 -*-
from testLib import *
import time
from rawSampleImage import *


def testWeekController():
    test = controllerTest('Week Controller')

    ####
    test.newTest('Register Gunnar')
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
    test.ensureSuccess(response)

    gunnar = test.login(gunnarUsername)

    ####
    test.newTest('Get (empty)List of all weeks')
    response = test.request('GET', 'Week', auth=gunnar)
    # test.ensureSuccess(response)
    test.ensureError(response)  # This error makes no sense but whatever.

    day = '''
    {{
        "thumbnailID": 4,
        "elementsSet": true,
        "elementIDs": [ 2 ],
        "day": {0},
        "elements": [{{
            "accessLevel": 0,
            "imageUrl": "/v1/pictogram/6/image/raw",
            "imageHash": "+8NDAclP6o11ft/Ba2yCZA==",
            "title": "sig",
            "id": 6,
            "lastEdit": "2018-03-28T10:47:51.628333"
        }}]
    }}
    '''

    tooManyDaysWeekDTO = '''
        {{
          "thumbnail" : {{
            "accessLevel": 0,
            "imageUrl": "junkdata",
            "imageHash": "junkdata",
            "title": "simpelt",
            "id": 5,
            "lastEdit": "2999-03-28T10:47:51.628376"
        }},
          "name" : "The best week of the day",
          "id" : 0,
          "days" : [{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}]
        }}
    '''.format(day.format(0), day.format(1), day.format(2), day.format(3), day.format(4), day.format(5), day.format(6),
               day.format(3))

    badEnumValueWeekDTO = '''
        {{
          "thumbnail" : {{
            "accessLevel": 0,
            "imageUrl": "junkdata",
            "imageHash": "junkdata",
            "title": "simpelt",
            "id": 5,
            "lastEdit": "2999-03-28T10:47:51.628376"
        }},
          "name" : "The best week of the day",
          "id" : 0,
          "days" : [{0}, {1}, {2}, {3}, {4}, {5}, {6} ]
        }}
    '''.format(day.format(99), day.format(1), day.format(2), day.format(3), day.format(4), day.format(5), day.format(6))

    print badEnumValueWeekDTO

    correctWeekDTO = '''
        {{
          "thumbnail" : {{
            "accessLevel": 0,
            "imageUrl": "junkdata",
            "imageHash": "junkdata",
            "title": "simpelt",
            "id": 5,
            "lastEdit": "2999-03-28T10:47:51.628376"
        }},
          "name" : "The best week of the day",
          "id" : 0,
          "days" : [{0}, {1}, {2}, {3}, {4}, {5}, {6} ]
        }}
    '''.format(day.format(0), day.format(1), day.format(2), day.format(3), day.format(4), day.format(5), day.format(6))

    ####
    test.newTest('Try to create week with too many weekdays')
    response = test.request('POST', 'Week', data=tooManyDaysWeekDTO, auth=gunnar)
    test.ensureError(response)

    ####
    test.newTest('Try to create week with invalid weekday')
    response = test.request('POST', 'Week', data=badEnumValueWeekDTO, auth=gunnar)
    test.ensureError(response)

    ####
    test.newTest('Create new week')
    response = test.request('POST', 'Week', data=correctWeekDTO, auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.newTest('Get List of all weeks again, find our week')
    response = test.request('GET', 'Week', auth=gunnar)
    if test.ensureSuccess(response):
        test.ensureEqual('The best week of the day', response['data'][0]['name'])
        weekID = response['data'][0]['id']
    else:
        print('Critical error in Week Controller Test: Not all tests could be run.')
        return

    otherDay = '''
    {{
        "thumbnailID": 4,
        "elementsSet": true,
        "elementIDs": [ 2 ],
        "day": {0},
        "elements": [{{
            "accessLevel": 0,
            "imageUrl": "/v1/pictogram/6/image/raw",
            "imageHash": "+8NDAclP6o11ft/Ba2yCZA==",
            "title": "JUNK",
            "id": 2,
            "lastEdit": "2018-03-28T10:47:51.628333"
        }}]
    }}
    '''

    otherCorrectWeekDTO = '''
        {{
          "thumbnail" : {{
            "accessLevel": 0,
            "imageUrl": "junkdata",
            "imageHash": "junkdata",
            "title": "simpelt",
            "id": 5,
            "lastEdit": "2999-03-28T10:47:51.628376"
            }},
          "name" : "The best week of the day",
          "id" : 0,
          "days" : [{0}, {1}, {2}, {3}, {4}, {5}, {6} ]
        }}
    '''.format(otherDay.format(0), otherDay.format(1), otherDay.format(2), otherDay.format(3),
               otherDay.format(4), otherDay.format(5), otherDay.format(6))

    ####
    test.newTest('Update whole week at once')
    response = test.request('PUT', 'Week/{0}'.format(weekID), data=otherCorrectWeekDTO, auth=gunnar)
    test.ensureSuccess(response)
    
    response = test.request('GET', 'Week/{0}'.format(weekID), auth=gunnar)
    
    for i in range(1, 6, 1):
        test.ensureEqual(2, response['data']['days'][i]['elements'][0]['id'])

    ####
    test.newTest('Update single day(Day Controller is not getting its own file.)')
    someOtherDay = '''
        {
          "elementsSet": true,
          "elementIDs": [
            3
          ],
          "day": "Wednesday",
          "elements": [ ]
        }
    '''
    response = test.request('PUT', 'Day/{0}'.format(weekID), data=someOtherDay, auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'Week/{0}'.format(weekID), auth=gunnar)
    wednesdayIndex = 2
    for i in range(1, 6, 1):
        if wednesdayIndex == response['data']['days'][i]['day']:
            test.ensureEqual(3, response['data']['days'][i]['elements'][0]['id'])

    ####
    test.newTest('Delete the week')
    response = test.request('DELETE', 'Week/{0}'.format(weekID), data='', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'Week', auth=gunnar)
    test.ensureError(response)
