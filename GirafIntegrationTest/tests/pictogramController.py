# -*- coding: utf-8 -*-
from testLib import *
import time
from integrate import TestCase, test
from rawSampleImage import *


class PictogramController(TestCase):
    "PictogramController Test"

    kurtToken = None
    fishID = None
    fishName = None

    @test()
    def loginAsKurt(self, check):
        "Login as kurt"
        response = requests.post(Test.url + 'account/login', json={"username": "kurt", "password": "password"}).json()
        check.is_true(response['success'])
        check.is_not_none(response['data'])
        PictogramController.kurtToken = response['data']

    @test(skip_if_failed=["loginAsKurt"])
    def getAllPictograms(self, check):
        "Get all the pictograms"
        response = requests.get(Test.url + 'Pictogram?page=1&pageSize=10',
                                headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        check.is_true(response['success'])
        # TODO: Check that we receive some pictograms

    @test(skip_if_failed=["loginAsKurt"])
    def getPublicPictogram(self, check):
        "Get public pictogram"
        response = requests.get(Test.url + 'Pictogram/2',
                                headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        ensureSuccess(response, check)

    @test(skip_if_failed=["loginAsKurt"])
    def post(self, check):
        "Post (private) Fish pictogram"
        PictogramController.fishName = 'fish{0}'.format(str(time.time()))

        response = requests.post(Test.url + 'Pictogram', json={
            "accessLevel": 3,
            "title": PictogramController.fishName,
            "id": -213215,
            "lastEdit": "2099-03-19T10:40:26.587Z"
        }, headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        check.is_true(response['success'])
        check.not_equal(response['data']['lastEdit'], '2099-03-19T10:40:26.587Z', "Don't trust anybody")
        PictogramController.fishID = response['data']['id']
        check.not_equal(PictogramController.fishID, -213215, "Userdefined ID's should not be used in database")

    @test(skip_if_failed=["loginAsKurt"])
    def postInvalidAccesslevel(self, check):
        "Post pictogram with invalid accesslevel"
        response = requests.post(Test.url + 'Pictogram', json={
            "accessLevel": 0,
            "title": PictogramController.fishName,
            "id": -213215,
            "lastEdit": "2099-03-19T10:40:26.587Z"
        }, headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        ensureError(response, check)

    @test(skip_if_failed=["loginAsKurt", "post"])
    def getPosted(self, check):
        " the freshly posted Fish"
        response = requests.get(Test.url + 'Pictogram/{0}'.format(PictogramController.fishID),
                                headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()

        check.is_true(response['success'])
        check.equal(response['data']['title'], PictogramController.fishName)

    @test(skip_if_failed=["loginAsKurt", "getPosted"])
    def put(self, check):
        "Alter the Fish"
        PictogramController.fishName = "Cursed " + PictogramController.fishName
        response = requests.put(Test.url + 'Pictogram/{0}'.format(PictogramController.fishID), json={
            "accessLevel": 3,
            "title": PictogramController.fishName,
            "id": -213215,
            "lastEdit": "2099-03-19T10:40:26.587Z"
        }, headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        check.is_true(response['success'])

    @test(skip_if_failed=["put"])
    def checkPut(self, check):
        "Check that the name was updated"
        response = requests.get(Test.url + 'Pictogram/{0}'.format(PictogramController.fishID),
                                headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        check.is_true(response['success'])
        check.equal(response['data']['title'], PictogramController.fishName)

    @test(skip_if_failed=["loginAsKurt"])
    def getPublicPictogramImage(self, check):
        "Get image of public pictogram"
        response = requests.get(Test.url + 'Pictogram/2/image',
                                headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        check.is_true(response['success'])
        check.is_not_none(response['data'])

    @test(skip_if_failed=["loginAsKurt"])
    def getPublicPictogramImageRaw(self, check):
        "Get raw image of public pictogram"
        response = requests.get(Test.url + 'Pictogram/2/image/raw',
                                headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)})
        check.equal(response.status_code, 200)

    @test(skip_if_failed=["loginAsKurt", "post"])
    def putImage(self, check):
        "Put image into Cursed Fish"
        response = requests.put(Test.url + 'Pictogram/{0}/image'.format(PictogramController.fishID),
                                data=rawSampleImage,
                                headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        check.is_true(response['success'])

    @test(skip_if_failed=["loginAsKurt", "putImage", "checkPut"])
    def deletePictogram(self, check):
        "Delete the Cursed Fish"
        response = requests.delete(Test.url + 'Pictogram/{0}'.format(PictogramController.fishID),
                                   headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        check.is_true(response['success'])

    @test(skip_if_failed=["deletePictogram"])
    def checkDeletePictogram(self, check):
        "Check that the Cursed Fish has been purged"
        response = requests.get(Test.url + 'Pictogram/{0}'.format(PictogramController.fishID),
                                headers={"Authorization": "Bearer {0}".format(PictogramController.kurtToken)}).json()
        check.is_false(response['success'])
