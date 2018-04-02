from testLib import *
from accountControllerTest import *
from departmentControllerTest import *
from roleControllerTest import *
from pictogramControllerTest import *
from userControllerTest import *
from weekControllerTest import *
import time
import sys
import json

# Nice error message if the server is down.
# First time I encountered the exception it took me 20 minutes to figure out why.
try:
    result = controllerTest('').request('GET', '/user')
    result['success']
except:
    print('Could not get response from server. \n'
          'Exiting...\n')
    sys.exit()

# Run ALL the tests!
testAccountController()
testDepartmentController()
testPictogramController()
testRoleController()
testUserController()
testWeekController()

if controllerTest.testsFailed == 0:
    print '{0} tests were run. All tests passed.'.format(controllerTest.testsRun)
else:
    print ('{0} test(s) failed out of {1} test(s) run. Happy debugging.'
           .format(controllerTest.testsFailed, controllerTest.testsRun))
