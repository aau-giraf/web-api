import unittest
import sys
from requests import get
from requests.exceptions import ConnectionError
from argparse import ArgumentParser
from testlib import GIRAFTestResults, GIRAFTestRunner, compare, BASE_URL

parser = ArgumentParser()
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

try:
    result = get(f'{BASE_URL}v1/Status').json()
except ConnectionError:
    print('\033[91m' + 'Error:' + '\033[0m' + ' could not get response from server.\nExiting...')
    sys.exit(1)

# setup runner with high verbosity, variable printing on fail/error, and custom results class
runner = GIRAFTestRunner(verbosity=5, tb_locals=True, resultclass=GIRAFTestResults)

# set comparison function
unittest.defaultTestLoader.sortTestMethodsUsing = compare

# load tests
suite = unittest.defaultTestLoader.discover(start_dir='tests')

# run
if __name__ == '__main__':
    print('\033[33m' + 'Running integration tests for the GIRAF web API' + '\033[0m')
    print('----------------------------------------------------------------------\n')
    runner.run(suite)
