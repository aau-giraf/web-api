# -*- coding: utf-8 -*-
from testLib import *
import time
from integrate import TestCase, test

class ChoiceController(TestCase):
  "Choice Controller"

  gunnarToken = None
  choiceID = None
  url = Test.url

  @test()
  def registerGunnar(self, check):
    "Register Gunnar"
    # Will generate a unique enough number, so the user isn't already created
    gunnarUsername = 'Gunnar{0}'.format(str(time.time()))
    response = requests.post(ChoiceController.url + 'account/register', json={"username": gunnarUsername,"password": "password","departmentId": 1}).json()
    check.is_true(response['success'])
    check.is_not_none(response['data'])
    ChoiceController.gunnarToken = response["data"]

  @test(skip_if_failed=["registerGunnar"])
  def postChoice(self, check):
    "Post choice"
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
    response = requests.post(ChoiceController.url + 'Choice', json=body, headers = {"Authorization":"Bearer {0}".format(ChoiceController.gunnarToken)})
    print(response.text)
    response = response.json()
    check.is_true(response['success'])
    check.is_not_none(response['data']['id'])
    ChoiceController.choiceId = response['data']['id']

  @test(skip_if_failed=["postChoice", "registerGunnar"])
  def getChoice(self, check):
    "Get choice"
    response = requests.get(ChoiceController.url + 'Choice/{0}'.format(ChoiceController.choiceID), headers = {"Authorization":"Bearer {0}".format(ChoiceController.gunnarToken)}).json()
    check.is_true(response['success'])
    check.equal(response['title'], 'Tegn OR Sølv')
    check.equal(response['options'][1]['title'], 'tegn')
    check.equal(response['options'][1]['id'], '24')
    check.equal(response['options'][2]['title'], 'sølv')

  @test(skip_if_failed=["registerGunnar"])
  def getNonexistentChoice(self, check):
    "Get non-existent choice"
    response = requests.get(ChoiceController.url + 'Choice/-1', headers = {"Authorization":"Bearer {0}".format(ChoiceController.gunnarToken)}).json()
    check.is_false(response['success'])

  @test(skip_if_failed=["postChoice"])
  def updateChoice(self, check):
    "Update choice"
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
    }

    response = requests.PUT(ChoiceController.url + 'Choice/{0}'.format(ChoiceController.choiceID), json=body, headers = {"Authorization":"Bearer {0}".format(ChoiceController.gunnarToken)}).json()
    check.is_true(response['success'])

  @test(skip_if_failed=["updateChoice"])
  def checkChoiceUpdated(self, check):
    "Check that the choice changed"
    response = requests.get(ChoiceController.url + 'Choice/{0}'.format(ChoiceController.choiceID), headers = {"Authorization":"Bearer {0}".format(ChoiceController.gunnarToken)}).json()
    check.is_true(response['success'])
    check.equal(response['title'], 'Tegn OR Sætte')
    check.equal(response['options'][2]['title'], 'sætte')

  @test(skip_if_failed=["checkChoiceUpdated", "updateChoice", "getChoice", "postChoice"])
  def deleteChoice(self, check):
    "Delete the choice"
    response = requests.delete(ChoiceController.url + 'Choice/{0}'.format(ChoiceController.choiceID), headers = {"Authorization":"Bearer {0}".format(ChoiceController.gunnarToken)}).json()
    check.is_true(response['success'])

  @test(skip_if_failed=["deleteChoice"])
  def checkDeleted(self, check):
    "Check that it is no longer there"
    response = requests.get(ChoiceController.url + 'Choice/{0}'.format(ChoiceController.choiceID), headers = {"Authorization":"Bearer {0}".format(ChoiceController.gunnarToken)}).json()
    check.is_false(response['success'])

  @test()
  def deleteNonexistent(self, check):
    "Delete the nonexistent choice"
    response = requests.delete(ChoiceController.url + 'Choice/-1', headers = {"Authorization":"Bearer {0}".format(ChoiceController.gunnarToken)}).json()
    check.is_false(response['success'])