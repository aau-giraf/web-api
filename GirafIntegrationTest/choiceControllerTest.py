# -*- coding: utf-8 -*-
from testLib import *
import time


def choiceControllerTest():
    test = test("Choice Controller")

    ####
    test.newTest('Register Gunnar')
    # Will generate a unique enough number, so the user isn't already created
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = test.request('POST', 'account/register',
                            '{"username": "' + gunnarUsername + '","password": "password","departmentId": 1}')
    test.ensureSuccess(response)

    gunnar = test.login(gunnarUsername)

    ####
    test.newTest("Post choice(OK)")
    body = '''{
      "options": [
        {
          "title": "tegn",
          "accessLevel": 0,
          "id": 24,
          "lastEdit": "2017-04-11T09:16:58.4315569",
          "users": [],
          "departments": []
        },
        {
          "title": "sølv",
          "accessLevel": 0,
          "id": 25,
          "lastEdit": "2017-04-06T15:43:19.898397",
          "users": [],
          "departments": []
        }
      ],
      "title": "Tegn OR Sølv",
      "id": 0,
      "lastEdit": "2018-03-16T08:24:58.902Z"
    }'''

    response = test.request('POST', 'Choice', data=body, auth=gunnar)
    test.ensureSuccess(response)
    postedChoiceId = response.get('id')

    ####
    test.newTest('Check that choice is in database')
    response = test.request('GET', 'Choice/{0}'.format(postedChoiceId), auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response.get('title') == 'Tegn OR Sølv')
    test.ensure(response.get('options').get(2).get('title') == 'sølv')

    ####
    test.newTest('Check that choice is in database')
    response = test.request('GET', 'Choice/-1', auth=gunnar)
    test.ensureError(response)


    ####
    test.newTest("Update choice(OK)")
    body = '''{
      "options": [
        {
          "title": "tegn",
          "accessLevel": 0,
          "id": 24,
          "lastEdit": "2017-04-11T09:16:58.4315569",
          "users": [],
          "departments": []
        },
        {
          "title": "sætte",
          "accessLevel": 0,
          "id": 26,
          "lastEdit": "2017-04-06T15:43:19.898397",
          "users": [],
          "departments": []
        }
      ],
      "title": "Tegn OR Sætte",
      "id": 0,
      "lastEdit": "2018-03-16T08:24:58.902Z"
    }'''

    response = test.request('PUT', 'Choice/{0}'.format(postedChoiceId), data=body, auth=gunnar)
    test.ensureSuccess(response)

    ####
    test.newTest('Check that it changed')
    response = test.request('GET', 'Choice/{0}'.format(postedChoiceId), auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response.get('title') == 'Tegn OR Sætte')
    test.ensure(response.get('options').get(2).get('title') == 'sætte')

    ####
    test.newTest('Delete the choice')
    response = test.request('DELETE', 'Choice/{0}'.format(postedChoiceId), auth=gunnar)
    test.ensure(response['success'] is True, errormessage='Error: {0}'.format(response['errorKey']))

    ####
    test.newTest('Check that it is no longer there')
    response = test.request('GET', 'Choice/{0}'.format(postedChoiceId), auth=gunnar)
    test.ensureError(response)

    ####
    test.newTest('Delete the nonexistent choice')
    response = test.request('DELETE', 'Choice/-1', auth=gunnar)
    test.ensureError(response)