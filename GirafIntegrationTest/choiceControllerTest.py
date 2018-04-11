# -*- coding: utf-8 -*-
from testLib import *
import time

class ChoiceControllerTest(TestCase):
  "Choice Controller"

  @test()
  def RegisterGunnar(self, check):
    "Register Gunnar"
    # Will generate a unique enough number, so the user isn't already created
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = requests.post(url + 'account/register', json={"username": gunnarUsername,"password": "password","departmentId": 1}).json()
    check.is_true(response['success'])
    gunnar = test.login(gunnarUsername)

  @test()
  def TESTNAME(self, check):
    test.new("Post choice(OK)")
    body = {
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
    }

    response = requests.post(url + 'Choice', data=body, auth=gunnar)
    test.ensureSuccess(response)
    postedChoiceId = response.get('id')

  @test()
  def TESTNAME(self, check):
    "Check that choice is in database"
    response = requests.GET(url + 'Choice/{0}'.format(postedChoiceId), auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response.get('title') == 'Tegn OR Sølv')
    test.ensure(response.get('options').get(2).get('title') == 'sølv')

  @test()
  def TESTNAME(self, check):
    "Check that choice is in database"
    response = requests.GET(url + 'Choice/-1', auth=gunnar)
    test.ensureError(response)


  @test()
  def TESTNAME(self, check):
    test.new("Update choice(OK)")
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

    response = requests.PUT(url + 'Choice/{0}'.format(postedChoiceId), data=body, auth=gunnar)
    test.ensureSuccess(response)

  @test()
  def TESTNAME(self, check):
    "Check that it changed"
    response = requests.GET(url + 'Choice/{0}'.format(postedChoiceId), auth=gunnar)
    test.ensureSuccess(response)
    test.ensure(response.get('title') == 'Tegn OR Sætte')
    test.ensure(response.get('options').get(2).get('title') == 'sætte')

  @test()
  def TESTNAME(self, check):
    "Delete the choice"
    response = requests.DELETE(url + 'Choice/{0}'.format(postedChoiceId), auth=gunnar)
    test.ensure(response['success'] is True, errormessage='Error: {0}'.format(response['errorKey']))

  @test()
  def TESTNAME(self, check):
    "Check that it is no longer there"
    response = requests.GET(url + 'Choice/{0}'.format(postedChoiceId), auth=gunnar)
    test.ensureError(response)

  @test()
  def TESTNAME(self, check):
    "Delete the nonexistent choice"
    response = requests.DELETE(url + 'Choice/-1', auth=gunnar)
    test.ensureError(response)