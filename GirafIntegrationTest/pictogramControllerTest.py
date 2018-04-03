# -*- coding: utf-8 -*-
from testLib import *
import time
import json
from rawSampleImage import *


def testPictogramController():
    test = controllerTest('Pictogram Controller')

    kurt = test.login('Kurt')

    ####
    test.newTest('Get ALL the pictograms!')
    response = test.request('GET', 'Pictogram')
    test.ensureSuccess(response)

    ####
    test.newTest('Get public pictogram')
    response = test.request('GET', 'pictogram/2', auth=kurt)
    test.ensureSuccess(response)

    ####
    test.newTest('Get someone else\'s Private pictogram')
    response = test.request('GET', 'pictogram/62', auth=kurt)
    test.ensureError(response)

    ####
    test.newTest('Post (private) Fish pictogram')
    fishName = 'fish{0}'.format(str(time.time()))
    body = '''
        {{
          "accessLevel": 0,
          "title": "{0}",
          "id": -213215,
          "lastEdit": "2099-03-19T10:40:26.587Z"
        }}
    '''.format(fishName)
    response = test.request('POST', 'Pictogram', data=body, auth=kurt)
    test.ensureSuccess(response)
    test.ensure(response['data']['lastEdit'] != '2099-03-19T10:40:26.587Z', errormessage='Never trust the client.')
    fishID = response['data']['id']
    test.ensure(fishID != -213215, errormessage='Especially don\'t let the client choose their own database IDs!')

    ####
    test.newTest('Get the freshly posted Fish')
    response = test.request('GET', 'Pictogram/{0}'.format(fishID), auth=kurt)
    test.ensureSuccess(response)
    test.ensure(response['data']['title'] == fishName,
                errormessage='Writing this error-message is left as an exercise for the reader.')

    ####
    test.newTest('Alter the Fish')
    body = '''
        {{
          "accessLevel": 0,
          "title": "Cursed {0}",
          "id": -213215,
          "lastEdit": "2099-03-19T10:40:26.587Z"
        }}
    '''.format(fishName)
    response = test.request('PUT', 'Pictogram/{0}'.format(fishID), data=body, auth=kurt)
    test.ensureSuccess(response)

    ####
    test.newTest('Check that the name was updated')
    response = test.request('GET', 'Pictogram/{0}'.format(fishID), auth=kurt)
    test.ensureSuccess(response)
    test.ensure(response['data']['title'] == 'Cursed ' + fishName,
                errormessage='Expected name: {0} \nActual name : {1}'.format('Cursed ' + fishName, response['data']['title']))

    ####
    test.newTest('Get image of public pictogram')
    response = test.request('GET', 'pictogram/2/image')
    test.ensureSuccess(response)
    test.ensureSomeData(response)

    ####
    test.newTest('Get raw image of public pictogram')
    response = test.request('GET', 'pictogram/2/image/raw')
    test.ensure(response is not None, 'Nothing was returned in response!')

    ####
    test.newTest('Get image of private pictogram')
    response = test.request('GET', 'pictogram/62/image')
    test.ensureError(response)

    ####
    test.newTest('Get raw image of private pictogram')
    response = test.request('GET', 'pictogram/62/image/raw')
    test.ensure(response is None, 'Should not return private image')

    ####
    test.newTest('Put image into Cursed Fish')
    response = test.request('PUT', 'Pictogram/{0}/image'.format(fishID), data=rawSampleImage, auth=kurt)

    response = test.request('PUT', 'Pictogram/{0}/image'.format(fishID), auth=kurt)
    test.ensure(response is not None, 'Image expected')  # Let's see. This may crash the poor thing.

    ####
    test.newTest('Delete the Cursed Fish')
    response = test.request('DELETE', 'Pictogram/{0}'.format(fishID), data=' ', auth=kurt)
    test.ensureSuccess(response)

    ####
    test.newTest('Check that the Cursed Fish has been purged')
    response = test.request('GET', 'Pictogram/{0}'.format(fishID), auth=kurt)
    test.ensureError(response)