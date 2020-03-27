import unittest
from unittest.runner import TextTestResult, TextTestRunner
from unittest.case import TestCase
import time
import warnings

BASE_URL = 'http://127.0.0.1:5000/'


def auth(token: str) -> dict:
    return {'Authorization': f'Bearer {token}'}


def order_handler():
    ordered = {}

    def ordered_handler(f):
        ordered[f.__name__] = len(ordered)
        return f

    def compare_handler(a, b):
        return [1, -1][ordered[a] < ordered[b]]

    return ordered_handler, compare_handler


order, compare = order_handler()
unittest.defaultTestLoader.sortTestMethodsUsing = compare


class GIRAFTestCase(unittest.TestCase):
    """
    Class for customizing the TestCase class. Should be passed in all GIRAF test classes.
    """

    def shortDescription(self) -> str:
        """
        Use the first line of the test method doc string as terminal output for info.
        """
        if self._testMethodDoc:
            return '\033[36m' + self._testMethodDoc.strip().split('\n')[0].strip() + '\033[0m'
        else:
            return ''

    def __str__(self):
        """
        Print the method name as a header for each test.
        """
        return f'[{self._testMethodName}]'

    @property
    def method_doc(self):
        if self._testMethodDoc:
            return self._testMethodDoc
        else:
            return '.'


class GIRAFTestResults(TextTestResult):

    def getDescription(self, test: TestCase) -> str:
        doc_first_line = test.shortDescription()
        return str(doc_first_line)

    def addSuccess(self, test: TestCase) -> None:
        super(TextTestResult, self).addSuccess(test)
        if self.showAll:
            self.stream.writeln(f'{" " * (120 - len(test.shortDescription()))}' + '\033[32m' + 'OK' + '\033[0m')
        elif self.dots:
            self.stream.write('.')
            self.stream.flush()

    def addError(self, test: TestCase, err) -> None:
        super(TextTestResult, self).addError(test, err)
        if self.showAll:
            self.stream.writeln(f'{" " * (120 - len(test.shortDescription()))}' + '\033[93m' + 'ERROR' + '\033[0m')
        elif self.dots:
            self.stream.write('E')
            self.stream.flush()

    def addFailure(self, test: TestCase, err) -> None:
        super(TextTestResult, self).addFailure(test, err)
        if self.showAll:
            self.stream.writeln(f'{" " * (120 - len(test.shortDescription()))}' + '\033[91m' + 'FAIL' + '\033[0m')
        elif self.dots:
            self.stream.write('F')
            self.stream.flush()

    def printErrors(self) -> None:
        if self.dots or self.showAll:
            if len(self.errors) > 0 or len(self.failures) > 0:
                self.stream.writeln()
        self.printErrorList('ERROR', self.errors)
        self.printErrorList('FAIL', self.failures)


class GIRAFTestRunner(TextTestRunner):

    def run(self, test):
        "Run the given test case or test suite."
        result = self._makeResult()
        unittest.signals.registerResult(result)
        result.failfast = self.failfast
        result.buffer = self.buffer
        result.tb_locals = self.tb_locals
        with warnings.catch_warnings():
            if self.warnings:
                # if self.warnings is set, use it to filter all the warnings
                self.warnings.simplefilter(self.warnings)
                # if the filter is 'default' or 'always', special-case the
                # warnings from the deprecated unittest methods to show them
                # no more than once per module, because they can be fairly
                # noisy.  The -Wd and -Wa flags can be used to bypass this
                # only when self.warnings is None.
                if self.warnings in ['default', 'always']:
                    self.warnings.filterwarnings('module',
                                 category=DeprecationWarning,
                                 message=r'Please use assert\w+ instead.')
            startTime = time.perf_counter()
            startTestRun = getattr(result, 'startTestRun', None)
            if startTestRun is not None:
                startTestRun()
            try:
                test(result)
            finally:
                stopTestRun = getattr(result, 'stopTestRun', None)
                if stopTestRun is not None:
                    stopTestRun()
            stopTime = time.perf_counter()
        timeTaken = stopTime - startTime
        result.printErrors()
        run = result.testsRun
        self.stream.writeln("Ran %d test%s in %.3fs" %
                            (run, run != 1 and "s" or "", timeTaken))
        self.stream.writeln()
        self.stream.writeln(f'{result.testsRun - len(result.errors) - len(result.failures)} passed ('
                            f'{len(result.unexpectedSuccesses)} unexpected), {len(result.errors)} errors, '
                            f'{len(result.failures)} failed ({len(result.expectedFailures)} expected), '
                            f'{len(result.skipped)} skipped')
        self.stream.writeln()

        expectedFails = unexpectedSuccesses = skipped = 0
        try:
            results = map(len, (result.expectedFailures,
                                result.unexpectedSuccesses,
                                result.skipped))
        except AttributeError:
            pass
        else:
            expectedFails, unexpectedSuccesses, skipped = results

        infos = []
        if not result.wasSuccessful():
            self.stream.write("FAILED")
            failed, errored = len(result.failures), len(result.errors)
            if failed:
                infos.append("failures=%d" % failed)
            if errored:
                infos.append("errors=%d" % errored)
        else:
            self.stream.write("OK")
        if skipped:
            infos.append("skipped=%d" % skipped)
        if expectedFails:
            infos.append("expected failures=%d" % expectedFails)
        if unexpectedSuccesses:
            infos.append("unexpected successes=%d" % unexpectedSuccesses)
        if infos:
            self.stream.writeln(" (%s)" % (", ".join(infos),))
        else:
            self.stream.write("\n")
        return result
