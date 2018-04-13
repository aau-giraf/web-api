#!/usr/bin/env python
import sys
import requests
import integrate
from testLib import *

# Nice error message if the server is down.
try:
    result = requests.post(Test.url + 'User').json()
    result['success']
except:
    print('Could not get response from server. \n'
          'Exiting...\n')
    sys.exit()

integrate.TestRunner(dirs=["tests"], pattern="*.py").run()

