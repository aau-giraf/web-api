import unittest
from requests import get
from requests.exceptions import ConnectionError
from testlib import GIRAFTestResults, GIRAFTestRunner, compare, BASE_URL
from tests.test_activity_controller import TestActivityController

try:
    result = get(f'{BASE_URL}v1/Error').json()
except ConnectionError:
    print('\033[91m' + 'Error:' + '\033[0m' + ' could not get response from server.\nExiting...')
    exit(1)

# setup runner with high verbosity, variable printing on fail/error, and custom results class
runner = GIRAFTestRunner(verbosity=5, tb_locals=True, resultclass=GIRAFTestResults)

# set comparison function
unittest.defaultTestLoader.sortTestMethodsUsing = compare

# load tests
#suite = unittest.defaultTestLoader.discover(start_dir='tests')
suite = unittest.defaultTestLoader.loadTestsFromTestCase(TestActivityController)

# run
if __name__ == '__main__':
    print('\033[33m' + 'Running integration tests for the GIRAF web API' + '\033[0m')
    print('----------------------------------------------------------------------\n')
    runner.run(suite)
