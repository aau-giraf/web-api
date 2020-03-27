import unittest
from testlib import GIRAFTestResults, GIRAFTestRunner, compare


runner = GIRAFTestRunner(verbosity=5, tb_locals=True, resultclass=GIRAFTestResults)
unittest.defaultTestLoader.sortTestMethodsUsing = compare
suite = unittest.defaultTestLoader.discover(start_dir='tests')

if __name__ == '__main__':
    print('\033[33m' + 'Running integration tests for the GIRAF web API' + '\033[0m')
    print('----------------------------------------------------------------------\n')
    runner.run(suite)
