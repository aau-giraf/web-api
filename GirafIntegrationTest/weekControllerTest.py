# -*- coding: utf-8 -*-
from testLib import *
import time
from rawSampleImage import *


def weekControllerTest():
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
    test.ensureSuccess(response)

    ####
    test.newTest('Create new week')
    createWeekDTO = '''
        {
          "thumbnail" : {
            "accessLevel": 0,
            "imageUrl": "junkdata",
            "imageHash": "junkdata",
            "title": "simpelt",
            "id": 5,
            "lastEdit": "2999-03-28T10:47:51.628376"
        },
          "name" : "The best week of the day",
          "id" : 0,
          "days" : [ ]
        }
    '''
    response = test.request('POST', 'Week', data=createWeekDTO, auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.newTest('Get List of all weeks again, find our week')
    response = test.request('GET', 'Week', auth=gunnar)
    test.ensureSuccess(response)
    if response['success']:
        test.ensureEqual('The best week of the day', response['data'][0]['name'])
        weekID = response['data'][0]['id']
    else:
        print('Critical error in Week Controller Test: Not all tests could be run.')
        return

    ####
    test.newTest('Update whole week at once')
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
    test.ensureEqual(3, response['data']['days'][wednesdayIndex]['day']['elements'][0]['id'])

    ####
    test.newTest('Delete the week')
    response = test.request('DELETE', 'Week/{0}'.format(weekID), data='', auth=gunnar)
    test.ensureSuccess(response)
    response = test.request('GET', 'Week', auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(len(response['data']) == 0, errormessage='The week does not appear to have been deleted.')
