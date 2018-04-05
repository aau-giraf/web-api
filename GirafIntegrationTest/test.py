from testLib import *
from accountControllerTest import *
from departmentControllerTest import *
from roleControllerTest import *
from pictogramControllerTest import *
from userControllerTest import *
from weekControllerTest import *
from authorizationTest import *
from userstoriesTest import *
import time
import sys
import json

# Nice error message if the server is down.
# First time I encountered the exception it took me 20 minutes to figure out why.
# TODO: Catch specific exception instead of all exceptions
# try:
result = Test('').request('GET', 'status')
result['success']
# except:
#     print('Could not get response from server. \n'
#           'Exiting...\n')
#     sys.exit()

# # Run ALL the tests!
# accountControllerTest()
# departmentControllerTest()
# pictogramControllerTest()
# roleControllerTest()
# userControllerTest()
# weekControllerTest()
# authorizationTest()
# expiredAuthorizationTest()
userstoriesTest()

if Test.testsFailed == 0:
    print '{0} tests were run. All tests passed.'.format(test.testsRun)
else:
    print ('{0} test(s) failed out of {1} test(s) run. Happy debugging.'
           .format(test.testsFailed, test.testsRun))
