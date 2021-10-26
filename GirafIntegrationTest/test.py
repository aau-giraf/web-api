from time import sleep
import unittest
import sys
from requests import get
from requests.exceptions import ConnectionError
from argparse import ArgumentParser
from testlib import GIRAFTestResults, GIRAFTestRunner, compare, BASE_URL

RETRYLIMIT = 5

def ConnectServer(count):
    if (count < RETRYLIMIT):
        try:
            get(f'{BASE_URL}v1/Status').json()
            return
        except ConnectionError:
            sleep(10)
            print('\033[91m' + 'Error:' + '\033[0m' + f' could not get response from server. Retrying {count}')
            ConnectServer(count+1)
    print(f'\033[91m' + 'Error:' + '\033[0m' + ' could not get response from server.\n Exiting...')
    sys.exit(1)


parser = ArgumentParser()
parser.add_argument('--pattern', type=str, help='custom search pattern',)
parser.add_argument('--missing', action='store_true', help='print untested endpoints')
parser.add_argument('--tested', action='store_true', help='print tested endpoints')

args = parser.parse_args()

# importing util globally borks the tests due to test class imports
if args.missing:
    from util import missing_endpoints
    missing_endpoints()
    sys.exit(0)
if args.tested:
    from util import tested_endpoints
    tested_endpoints()
    sys.exit(0)

ConnectServer(0)

# setup runner with high verbosity, variable printing on fail/error, and custom results class
runner = GIRAFTestRunner(verbosity=5, tb_locals=True, resultclass=GIRAFTestResults)

# set comparison function
unittest.defaultTestLoader.sortTestMethodsUsing = compare

# load tests
test_pattern = args.pattern if args.pattern else 'test*.py'
suite = unittest.defaultTestLoader.discover(start_dir='tests', pattern=test_pattern)

# run
if __name__ == '__main__':
    print('\033[33m' + 'Running integration tests for the GIRAF web API' + '\033[0m')
    print('----------------------------------------------------------------------\n')
    res = runner.run(suite)
    if not res.wasSuccessful():
        sys.exit(1)
