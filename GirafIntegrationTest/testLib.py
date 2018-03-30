import inspect
import subprocess
import json
import datetime
import getpass


class controllerTest:
    testsRun = 0     # Accross all controllertests
    thisTestHasFailed = False
    testsFailed = 0  # Accross all controllertests

    name = "unkonwn controller"
    currentTest = "unknown test"

    def __init__(self, name):
        self.name = name

    def call(self, *comm):
        process = subprocess.Popen(comm, stdout=subprocess.PIPE)
        output, error = process.communicate()
        try:
            return json.loads(output)
        except ValueError:
            return error

    def login(self, username, password='password'):
        loginresult = self.request('POST', 'account/login',
                                   '{{"username": "{0}", "password": "{1}"}}'.format(username, password))
        self.ensure(loginresult['success'], 'Failed to log in as {0} using password {1}'.format(username, password))
        return loginresult['data']

    def request(self, requestType, url, data='', auth='nokey'):
        return self.call('curl', '-sX',
                         requestType,
                         'http://localhost:5000/v1/{0}'.format(url),
                         '-H',
                         'accept: text/plain',
                         '-H',
                         "Authorization: bearer {0}".format(auth),
                         '-H',
                         'Content-Type: application/json-patch+json',
                         '-d',
                         data
                         )

    def newTest(self, title):
        if self.thisTestHasFailed:
            controllerTest.testsFailed += 1
        controllerTest.testsRun += 1

        self.currentTest = title
        self.thisTestHasFailed = False

    # self.ensure(self.request('POST', 'account/logout')['success']) #Logout should be explicit. No need for this.

    def isValidAuth(self, auth):
        return self.request('GET', 'user/username', auth=auth)['success']

    def ensure(self, fact, errormessage=""):
        if fact:
            return
        else:
            self.thisTestHasFailed = True
            print("ENSURE failed in {0} for test `{1}`".format(self.name, self.currentTest))
            print(errormessage)

            # code for geting debug-info gratefully borrowed from
            # https://stackoverflow.com/questions/6810999/how-to-determine-file-function-and-line-number
            callerframerecord = inspect.stack()[1]
            frame = callerframerecord[0]
            info = inspect.getframeinfo(frame)
            print ('\n  File:     {0}'.format(info.filename))
            print ('  Function: {0}'.format(info.function))
            print ('  Line:     {0}'.format(info.lineno))
            print("======================\n")
