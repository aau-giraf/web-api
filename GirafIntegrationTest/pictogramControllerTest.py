# -*- coding: utf-8 -*-
from testLib import *
import time
import json
from rawSampleImage import *


def PictogramControllerTest():
    test = Test('Pictogram Controller')

    kurt = test.login('Kurt')

    ####
    test.new('Get ALL the pictograms!')
    response = requests.get(Test.url() + 'Pictogram').json()
    test.ensureSuccess(response)

    ####
    test.new('Get public pictogram')
    response = requests.get(Test.url() + 'Pictogram/2', headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureSuccess(response)

    ####
    test.new('Get someone else\'s Private pictogram')
    response = requests.get(Test.url() + 'Pictogram/62', headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureError(response)

    ####
    test.new('Post (private) Fish pictogram')
    fishName = 'fish{0}'.format(str(time.time()))

    response = requests.post(Test.url() + 'Pictogram', json={
          "accessLevel": 0,
          "title": fishName,
          "id": -213215,
          "lastEdit": "2099-03-19T10:40:26.587Z"
        }, headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data']['lastEdit'] != '2099-03-19T10:40:26.587Z', errormessage='Never trust the client.')
    fishID = response['data']['id']
    test.ensure(fishID != -213215, errormessage='Especially don\'t let the client choose their own database IDs!')

    ####
    test.new('Get the freshly posted Fish')
    response = requests.get(Test.url() + 'Pictogram/{0}'.format(fishID), headers = {"Authorization":"Bearer {0}".format(kurt)}).json()

    test.ensureSuccess(response)
    test.ensure(response['data']['title'] == fishName,
                errormessage='Writing this error-message is left as an exercise for the reader.')

    ####
    test.new('Alter the Fish')
    response = requests.post(Test.url() + 'Pictogram/{0}'.format(fishID), json={
          "accessLevel": 0,
          "title": "Cursed fishy",
          "id": -213215,
          "lastEdit": "2099-03-19T10:40:26.587Z"
        }, headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureSuccess(response)

    ####
    test.new('Check that the name was updated')
    response = requests.get(Test.url() + 'Pictogram/{0}'.format(fishID), headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureSuccess(response)
    test.ensure(response['data']['title'] == 'Cursed ' + fishName,
                errormessage='Expected name: {0} \nActual name : {1}'.format('Cursed ' + fishName, response['data']['title']))

    ####
    test.new('Get image of public pictogram')
    response = requests.get(Test.url() + 'Pictogram/2/image').json()
    test.ensureSuccess(response)
    test.ensureSomeData(response)

    ####
    test.new('Get raw image of public pictogram')
    response = requests.get(Test.url() + 'Pictogram/2/image/raw')
    test.ensure(response.status_code == 200, 'Response wasn\'t 200 OK!')

    ####
    test.new('Get image of private pictogram')
    response = requests.get(Test.url() + 'Pictogram/62/image').json()
    test.ensureError(response)

    ####
    test.new('Put image into Cursed Fish')
    
    response = requests.put(Test.url() + 'Pictogram/{0}/image'.format(fishID), data=rawSampleImage, headers = {"Authorization":"Bearer {0}".format(kurt)})
    response = requests.put(Test.url() + 'Pictogram/{0}/image'.format(fishID), headers = {"Authorization":"Bearer {0}".format(kurt)})
    test.ensure(response.headers['content-type'] == "image/PNG", 'Response type was not PNG!')

    ####
    test.new('Delete the Cursed Fish')
    response = requests.delete(Test.url() + 'Pictogram/{0}'.format(fishID), headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureSuccess(response)

    ####
    test.new('Check that the Cursed Fish has been purged')
    response = requests.get(Test.url() + 'Pictogram/{0}'.format(fishID), headers = {"Authorization":"Bearer {0}".format(kurt)}).json()
    test.ensureError(response)