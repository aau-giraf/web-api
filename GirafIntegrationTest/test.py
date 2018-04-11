#!/usr/bin/env python

from integrate import TestRunner
TestRunner(dirs=["tests"], pattern="*.py").run()

# Nice error message if the server is down.
# First time I encountered the exception it took me 20 minutes to figure out why.
# try:
#     result = Test('').request('GET', '/user')
#     result['success']
# except:
#     print('Could not get response from server. \n'
#           'Exiting...\n')
#     sys.exit()

# Run ALL the tests!
# AccountControllerTest()
# DepartmentControllerTest()
# PictogramControllerTest()
# UserControllerTest()
# WeekControllerTest()
# AuthorizationTest()
# ExpiredAuthorizationTest()
# UserstoriesTest()

# if Test.failedCount == 0:
#     print ('{0} tests were run. All tests passed.'.format(Test.runCount))
# else:
#     print ('{0} test(s) failed out of {1} test(s) run. Happy debugging.'
#            .format(Test.failedCount, Test.runCount))
