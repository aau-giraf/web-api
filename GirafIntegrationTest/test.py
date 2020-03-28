import unittest
from testlib import GIRAFTestResults, GIRAFTestRunner, compare

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
