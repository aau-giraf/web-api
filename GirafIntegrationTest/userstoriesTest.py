# -*- coding: utf-8 -*-
from testLib import *
import time

def testUserstories():
    test = controllerTest("User stories")

    test.newTest('T914')
    # Login as guardian
    response = test.request('POST', 'account/login', '{"username": "Graatand", "password": "password"}')
    test.ensureSuccess(response)
    graatandToken = response['data']

    # Create week-schedule
    #DISCLAIMER: I have no idea what i'm doing and this is obviously not working (Other people will likely have the same problem...)
    response = test.request('POST', 'week', 
                    '''{
                      "thumbnail": {
                        "accessLevel": "PUBLIC",
                        "title": "string",
                        "id": 0,
                        "lastEdit": "2018-04-02T23:34:04.043Z"
                      },
                      "name": "string",
                      "id": 0,
                      "days": [
                        {
                          "elementsSet": true,
                          "elementIDs": [
                            0, 1, 2, 3, 4, 5, 6
                          ],
                          "day": "Monday",
                          "elements": [
                            {
                              "title": "string",
                              "id": 0,
                              "lastEdit": "2018-04-02T23:34:04.043Z"
                            }
                          ]
                          "day": "Tuesday",
                          "elements": [
                            {
                              "title": "string",
                              "id": 1,
                              "lastEdit": "2018-04-02T23:34:04.043Z"
                            }
                          ]
                          "day": "Wednesdat",
                          "elements": [
                            {
                              "title": "string",
                              "id": 2,
                              "lastEdit": "2018-04-02T23:34:04.043Z"
                            }
                          ]
                          "day": "Thursdag",
                          "elements": [
                            {
                              "title": "string",
                              "id": 3,
                              "lastEdit": "2018-04-02T23:34:04.043Z"
                            }
                          ]
                          "day": "Friday",
                          "elements": [
                            {
                              "title": "string",
                              "id": 4,
                              "lastEdit": "2018-04-02T23:34:04.043Z"
                            }
                          ]
                          "day": "Saturday",
                          "elements": [
                            {
                              "title": "string",
                              "id": 5,
                              "lastEdit": "2018-04-02T23:34:04.043Z"
                            }
                          ]
                          "day": "Sunday",
                          "elements": [
                            {
                              "title": "string",
                              "id": 6,
                              "lastEdit": "2018-04-02T23:34:04.043Z"
                            }
                          ]
                        }
                      ]
                    }''', auth=graatandToken)
    test.ensureSuccess(response)
