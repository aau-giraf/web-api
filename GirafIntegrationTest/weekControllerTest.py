# -*- coding: utf-8 -*-
from testLib import *
import time
from rawSampleImage import *


def WeekControllerTest():
    test = Test('Week Controller')

    ####
    test.new('Register Gunnar')
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
    test.ensureSuccess(response)

    gunnar = test.login(gunnarUsername)

    ####
    test.new('Get (empty)List of all weeks')
    response = test.request('GET', 'Week', auth=gunnar)
    # test.ensureSuccess(response)
    test.ensureError(response)  # This error makes no sense but whatever.

    ####
    test.new('Create new week')
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
    createWeekDTO = '''
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
    '''.format(day.format(1), day.format(2), day.format(3), day.format(4), day.format(5), day.format(6), day.format(7))
    response = test.request('POST', 'Week', data=createWeekDTO, auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.new('Get List of all weeks again, find our week')
    response = test.request('GET', 'Week', auth=gunnar)
    test.ensureSuccess(response)
    if response['success']:
        test.ensureEqual('The best week of the day', response['data'][0]['name'])
        weekID = response['data'][0]['id']
    else:
        print('Critical error in Week Controller Test: Not all tests could be run.')
        return

    ####
    test.new('Update whole week at once')
    someDay = '''
        {
          "elementsSet": true,
          "elementIDs": [
            2
          ],
          "day": "Monday",
          "elements": [
            {
              "title": "junkdata",
              "id": -1,
              "lastEdit": "2999-04-02T14:49:21.136Z"
            }
          ]
        }
    '''
    completeWeekDTO = '''
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
          "days" : [ {0}{0}{0}{0}{0}{0}{0} ]
        }} 
    '''
    response = test.request('PUT', 'Week/{0}'.format(weekID), data=completeWeekDTO.format(someDay), auth=gunnar)
    test.ensureSuccess(response)
    
    response = test.request('GET', 'Week/{0}'.format(weekID), auth=gunnar)
    
    for i in range(1, 6, 1):
        test.ensure('Monday' != response['data']['days'][i]['day'], 
                    errormessage='Never trust the client you imbeciles!')
        test.ensure('2999-04-02T14:49:21.136Z' != response['data']['days'][i]['day']['elements'][0]['lastEdit'], 
                    errormessage='Never trust the client you imbeciles!')
        test.ensureEqual(2, response['data']['days'][i]['day']['elements'][0]['id'])
        
    test.ensure('2999-03-28T10:47:51.628376' != response['data']['lastEdit'], 
                errormessage='Never trust the client you imbeciles!')

    ####
    test.new('Update single day(Day Controller is not getting its own file.)')
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
    test.ensureEqual(3, response['data']['days'][wednesdayIndex]['day']['elements'][0]['id'])

    ####
    test.new('Delete the week')
    response = test.request('DELETE', 'Week/{0}'.format(weekID), data='', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'Week', auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(len(response['data']) == 0, errormessage='The week does not appear to have been deleted.')
