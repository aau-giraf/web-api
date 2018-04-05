# -*- coding: utf-8 -*-
import inspect
import subprocess
import json
import datetime
import getpass
from termcolor import colored

class response:
    def __init__(self, rawResponse):
        self.raw = rawResponse.split("\n")
        self._json = None

    def _parseJSON(self):
        self._json = json.loads(self.getBody())

    def __getitem__(self, key):
        if self._json is None:
            self._parseJSON()
        return self._json[key]

    def __contains__(self, item):
        if self._json is None:
            self._parseJSON()
        return item in self._json

    def getBody(self):
        for idx, line in enumerate(self.raw):
            if line.startswith("* Connection #0 to host localhost left intact"):
                # print self.raw[idx+1:][0]
                return self.raw[idx+1:][0]

    def getResponseCode(self):
        for line in self.raw:
            if line.startswith("< HTTP/1.1 "):
                return line[11:14]
        raise Exception("Response code not found!")


class Test:
    testsRun = 0  # Accross all tests
    thisTestHasFailed = False
    testsFailed = 0  # Accross all tests

    name = "unkonwn controller"
    currentTest = "unknown test"

    def __init__(self, name):
        self.name = name

    def call(self, *comm):
        process = subprocess.Popen(comm, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        output, error = process.communicate()
        return response(output)

    def login(self, username, password='password'):
        loginresult = self.request('POST', 'account/login',
                                   '{{"username": "{0}", "password": "{1}"}}'.format(username, password))
        self.ensure(loginresult['success'], 'Failed to log in as {0} using password {1}'.format(username, password))
        return loginresult['data']

    def request(self, requestType, url, data='', auth='nokey'):
        return self.call('curl', '-svX',
                         requestType,
                         'http://localhost:5000/v1/{0}'.format(url),
                         '-H',
                         'accept: text/plain',
                         '-H',
                         "Authorization: bearer {0}".format(auth),
                         '-H',
                         'Content-Type: application/json-patch+json',
                         '-d',
                         data,
                         )

    def newTest(self, title):
        if self.thisTestHasFailed:
            Test.testsFailed += 1
        Test.testsRun += 1

        self.currentTest = title
        self.thisTestHasFailed = False

    # self.ensure(self.request('POST', 'account/logout')['success']) #Logout should be explicit. No need for this.

    def isValidAuth(self, auth):
        return self.request('GET', 'user/username', auth=auth)['success']

    def ensure(self, fact, errormessage="", calldepth=1):
        if fact:
            return
        else:
            # Extra thick line dividing different tests.
            if not self.thisTestHasFailed:
                print colored("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                              attrs=['dark'])

            self.thisTestHasFailed = True
            print(colored('ENSURE', 'red') +
                  ' failed in ' + colored(self.name, attrs=['underline']) +
                  ' for test ' + colored(self.currentTest, attrs=['underline']))
            print(colored(errormessage, attrs=['bold']))

            # code for geting debug-info gratefully borrowed from
            # https://stackoverflow.com/questions/6810999/how-to-determine-file-function-and-line-number
            callerframerecord = inspect.stack()[calldepth]
            frame = callerframerecord[0]
            info = inspect.getframeinfo(frame)
            print colored('\n  File:     {0}'.format(info.filename), attrs=['dark'])
            print colored('  Function: {0}'.format(info.function), attrs=['dark'])
            print colored('  Line:     {0}'.format(info.lineno), attrs=['dark'])
            print colored("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                          attrs=['dark'])

    def ensureSuccess(self, response):
        print response[0:]
        errormessages = ''
        if 'errorProperties' in response:
            for message in response['errorProperties']:
                errormessages += '\nMessage:  ' + message
        self.ensure(response['success'] is True,
                    errormessage='Error: {0}'.format(response['errorKey'] + errormessages),
                    calldepth=2)

    def ensureError(self, response):
        self.ensure(response['success'] is False,
                    errormessage='Server responds success on illegal action.',
                    calldepth=2)

    def ensureErrorWithKey(self, response, errorKey):
        try:
            self.ensure(response['success'] is False and response['errorKey'] == errorKey,
                        errormessage='Server did not respond correct errorKey or success-flag',
                        calldepth=2)
        except Exception as e:
            self.fails("Server did not return valid JSON")

    def ensureNotAuthorized(self, response):
        try:
            self.ensure(response['success'] is False and response['errorKey'] == 'NotAuthorized',
                        errormessage='Server did not respond with NotAuthorized. Got "' + response['errorKey'] + '"',
                        calldepth=2)
        except Exception as e:
            self.fails("Server did not return valid JSON")

    def ensureNoData(self, response):
        self.ensure('data' not in response or response['data'] is None,
                    errormessage='Data was returned when it should not have been.',
                    calldepth=2)

    def ensureSomeData(self, response):
        self.ensure('data' in response and response['data'] is not None,
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
        self.ensure(False, errormessage=errormessage, calldepth=calldepth+1)
