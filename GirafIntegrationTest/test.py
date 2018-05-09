#!/usr/bin/env python
import sys
import requests
import integrate
from testLib import *

# Nice error message if the server is down.
try:
    result = requests.get(Test.url + 'Account/access-denied')
    result = result.json()
except :
    print('Could not get response from server. \n'
          'Exiting...\n')
    sys.exit()

integrate.TestRunner(dirs=["tests"], pattern="departmentController.py").run()

