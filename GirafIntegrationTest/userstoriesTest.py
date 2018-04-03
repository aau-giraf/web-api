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
    response = test.request('POST', 'week', 
                     '''{
                          "thumbnail": {
                            "accessLevel": 2,
                            "imageUrl": "/v1/pictogram/1/image/raw",
                            "imageHash": "bjQ6eiEnZPp3CbW06iLZ6w==",
                            "title": "Epik",
                            "id": 1,
                            "lastEdit": "2018-03-23T08:36:23.275831"
                          },
                          "name": null,
                          "id": 1,
                          "days": [
                            {
                              "elementsSet": true,
                              "elementIDs": [
                                1,
                                74,
                                75,
                                76
                              ],
                              "day": 0,
                              "elements": [
                                {
                                  "accessLevel": 2,
                                  "imageUrl": "/v1/pictogram/1/image/raw",
                                  "imageHash": "bjQ6eiEnZPp3CbW06iLZ6w==",
                                  "title": "Epik",
                                  "id": 1,
                                  "lastEdit": "2018-03-23T08:36:23.275831"
                                },
                                {
                                  "accessLevel": 1,
                                  "imageUrl": "/v1/pictogram/74/image/raw",
                                  "imageHash": "mZfa7s0eJJnWhC8VwPduNw==",
                                  "title": "alting",
                                  "id": 74,
                                  "lastEdit": "2018-03-23T08:36:23.320533"
                                },
                                {
                                  "accessLevel": 2,
                                  "imageUrl": "/v1/pictogram/75/image/raw",
                                  "imageHash": "mZfa7s0eJJnWhC8VwPduNw==",
                                  "title": "alle",
                                  "id": 75,
                                  "lastEdit": "2018-03-23T08:36:23.275836"
                                },
                                {
                                  "accessLevel": 2,
                                  "imageUrl": "/v1/pictogram/76/image/raw",
                                  "imageHash": "M3VYTIo8N47tsQ7pG5N8dA==",
                                  "title": "alfabet",
                                  "id": 76,
                                  "lastEdit": "2018-03-23T08:36:23.275835"
                                }
                              ]
                            },
                            {
                              "elementsSet": false,
                              "elementIDs": [],
                              "day": 1,
                              "elements": []
                            },
                            {
                              "elementsSet": false,
                              "elementIDs": [],
                              "day": 2,
                              "elements": []
                            },
                            {
                              "elementsSet": false,
                              "elementIDs": [],
                              "day": 3,
                              "elements": []
                            },
                            {
                              "elementsSet": true,
                              "elementIDs": [
                                1
                              ],
                              "day": 4,
                              "elements": [
                                {
                                  "accessLevel": 2,
                                  "imageUrl": "/v1/pictogram/1/image/raw",
                                  "imageHash": "bjQ6eiEnZPp3CbW06iLZ6w==",
                                  "title": "Epik",
                                  "id": 1,
                                  "lastEdit": "2018-03-23T08:36:23.275831"
                                }
                              ]
                            },
                            {
                              "elementsSet": false,
                              "elementIDs": [],
                              "day": 5,
                              "elements": []
                            },
                            {
                              "elementsSet": true,
                              "elementIDs": [
                                74
                              ],
                              "day": 6,
                              "elements": [
                                {
                                  "accessLevel": 1,
                                  "imageUrl": "/v1/pictogram/74/image/raw",
                                  "imageHash": "mZfa7s0eJJnWhC8VwPduNw==",
                                  "title": "alting",
                                  "id": 74,
                                  "lastEdit": "2018-03-23T08:36:23.320533"
                                }
                              ]
                            }
                          ]
                        }''', auth=graatandToken)
    test.ensureSuccess(response)
    # TODO: Check that the database got updated correctly