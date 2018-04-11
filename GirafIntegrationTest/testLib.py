# -*- coding: utf-8 -*-
import http
import inspect
import subprocess
import json
import datetime
import getpass
from termcolor import colored
import requests
import logging


class Request:
    connection = None
    response = None
    headers = {'Content-type': 'json-patch+json', 'Accept-Encoding': 'text/plain'}

    def __init__(self, requestType, url, data='', auth='nokey', debuglevel=1):
        if Request.connection is None:
            Request.connection = http.client.HTTPConnection('127.0.0.1', 5000)
        self.request(requestType, url, data, auth)

    def login(self, username, password='password'):
        self.request('POST', 'account/login',
                     '{{"username": "{0}", "password": "{1}"}}'.format(username, password))
        return self.JSON()['data']

    def request(self, requestType, url, data='', auth='nokey'):
        Request.connection.request(requestType, "/v1/" + url, data,
                                   {**Request.headers, **{'Authorization': 'bearer {0}'.format(auth)}})
        self.response = Request.connection.getresponse()

    def JSON(self):
        return json.loads(self.response.read())

    def isValidAuth(self, auth):
        return self.request('GET', 'user/username', auth=auth)['success']


class Test:
    url = "http://0:5000/v1/"

    runCount = 0  # Accross all Tests
    failed = False
    failedCount = 0  # Accross all Tests

    name = "unkonwn controller"
    current = "unknown test"

    def __init__(self, name):
        self.name = name

    def login(self, username, password='password'):
        response = requests.post(Test.url + 'account/login', json={"username": username, "password": password})
        self.ensure(response.json()['success'], 'Failed to log in as {0} using password {1}'.format(username, password))
        return response.json()['data']

    def new(self, title):
        if self.failed:
            Test.failedCount += 1
        Test.runCount += 1

        self.current = title
        self.failed = False

    def ensure(self, fact, errormessage="", calldepth=1):
        if fact:
            return False
        else:
            # Extra thick line dividing different tests.
            if not self.failed:
                print(colored("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                              attrs=['dark']))

            self.failed = True
            print(colored('ENSURE', 'red') +
                  ' failed in ' + colored(self.name, attrs=['underline']) +
                  ' for test ' + colored(self.current, attrs=['underline']))
            print(colored(errormessage, attrs=['bold']))

            # code for geting debug-info gratefully borrowed from
            # https://stackoverflow.com/questions/6810999/how-to-determine-file-function-and-line-number
            callerframerecord = inspect.stack()[calldepth]
            frame = callerframerecord[0]
            info = inspect.getframeinfo(frame)
            print(colored('\n  File:     {0}'.format(info.filename), attrs=['dark']))
            print(colored('  Function: {0}'.format(info.function), attrs=['dark']))
            print(colored('  Line:     {0}'.format(info.lineno), attrs=['dark']))
            print(colored("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                          attrs=['dark']))
            return True

    def ensureValidResponse(self, response, calldepth=1):
        return self.ensure(response is not None, 'Invalid response. Likely a 404 or a stacktrace.', calldepth=calldepth)

    def ensureSuccess(self, response):
        if not self.ensureValidResponse(response):
            return False

        errormessages = ''

        if response is not None:
            for message in response['errorProperties']:
                errormessages += '\nMessage:  ' + message

        self.ensure(response['success'] is True,
                    errormessage='Error: {0}'.format(response['errorKey'] + errormessages),
                    calldepth=2)
        return response['success'] is True

    def ensureError(self, response):
        if not self.ensureValidResponse(response, calldepth=3):
            return False
        return self.ensure(response['success'] is False,
                           errormessage='Server responds success on illegal action.',
                           calldepth=2)

    def ensureErrorWithKey(self, response, errorKey):
        if not self.ensureValidResponse(response, calldepth=3):
            return False
        return self.ensure(response['success'] is False and response['errorKey'] == errorKey,
                           errormessage='Server did not respond correct errorKey or success-flag',
                           calldepth=2)

    def ensureNotAuthorized(self, response):
        if not self.ensureValidResponse(response, calldepth=3):
            return False
        return self.ensure(response['success'] is False and response['errorKey'] == 'NotFound',
                           errormessage='Server did not respond with NotFound. Got "' + response['errorKey'] + '"',
                           calldepth=2)

    def ensureNoData(self, response):
        if not self.ensureValidResponse(response, calldepth=3):
            return False
        return self.ensure('data' not in response or response['data'] is None,
                           errormessage='Data was returned when it should not have been.',
                           calldepth=2)

    def ensureSomeData(self, response):
        if not self.ensureValidResponse(response, calldepth=3):
            return False
        return self.ensure('data' in response and response['data'] is not None,
                           errormessage='Data expected but none returned',
                           calldepth=2)

    def ensureEqual(self, expected, actual):
        self.ensure(expected == actual,
                    errormessage='Expected: {0}\nActual:   {1}'.format(expected, actual),
                    calldepth=2)

    def ensureNotEqual(self, expected, actual):
        self.ensure(expected != actual,
                    errormessage='Expected: {0}\nActual:   {1}'.format(expected, actual),
                    calldepth=2)

    def fails(self, errormessage, calldepth=2):
        self.ensure(False, errormessage=errormessage, calldepth=calldepth + 1)
