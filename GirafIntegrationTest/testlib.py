import collections
import unittest
from unittest.runner import TextTestResult, TextTestRunner
from unittest.case import TestCase
import time
import warnings
import io
from typing import Any
from PIL import Image

# base API url
BASE_URL = 'http://127.0.0.1:5000/'


def auth(token: str) -> dict:
    """
    Yields a dict containing an authorization header
    :param token: auth token
    :return: header dict
    """
    return {'Authorization': f'Bearer {token}'}


def parse_image(content: bytes) -> Image.Image:
    """
    Parses pictogram as a PIL image
    :param content: binary image data
    :return: image object
    """
    return Image.open(io.BytesIO(content))


def order_handler():
    """
    Creates functions for ordering tests.
    :return: order and comparison handlers
    """
    ordered = {}

    def ordered_handler(f):
        """
        Order handler. Sorts the function it decorates in the order it is written in the code.
        :param f: the function it decorates
        :return: the function it decorates
        """
        ordered[f.__name__] = len(ordered)
        return f

    def compare_handler(a, b):
        """
        Comparison handler. The default sorter in unittest is set to this in test.py.
        :param a: function to compare
        :param b: function to compare
        :return: ordered function list
        """
        return [1, -1][ordered[a] < ordered[b]]

    return ordered_handler, compare_handler


def is_sequence(obj: Any) -> bool:
    """Return true if object is instance of list or tuple."""
    if isinstance(obj, (str, bytes)):
        return False
    return isinstance(obj, collections.abc.Sequence)


# instances of ordering and comparison functions for external import
order, compare = order_handler()


class GIRAFTestCase(unittest.TestCase):
    """
    Class for customizing the TestCase class. Should be passed in all GIRAF test classes.
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Print the first line of the class doc string when initialized.
        """
        doc = cls.__doc__.strip().split('\n')[0].strip() if cls.__doc__ else cls.__name__
        print('\033[33m' + f'{doc}' + '\033[0m')

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Print a separation line when class is finished.
        """
        print('\n----------------------------------------------------------------------\n')

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
        """
        Yield the methods doc string, or notify it is missing.
        :return: doc string if present
        """
        if self._testMethodDoc:
            return self._testMethodDoc
        else:
            return 'Missing method doc string'


class GIRAFTestResults(TextTestResult):
    """
    Modifications to the base TextTestResult class. This class contains the results of the tests.
    """

    def getDescription(self, test: TestCase) -> str:
        """
        Gets the doc string of the current method.
        :param test: test case class
        :return: method doc string
        """
        return str(test.shortDescription())

    def addSuccess(self, test: TestCase) -> None:
        """
        If the test passes, this is printed to its line in the terminal, and a result is added to the results object
        :param test: test case class
        """
        super(TextTestResult, self).addSuccess(test)
        if self.showAll:
            # pretty printing
            self.stream.writeln(f'{" " * (120 - len(test.shortDescription()))}' + '\033[32m' + 'OK' + '\033[0m')
        elif self.dots:
            self.stream.write('.')
            self.stream.flush()

    def addError(self, test: TestCase, err) -> None:
        """
        If the test yields an error, this is printed to its line in the terminal, and a result is added to the
        results object
        :param test: test case class
        """
        super(TextTestResult, self).addError(test, err)
        if self.showAll:
            # pretty printing
            self.stream.writeln(f'{" " * (120 - len(test.shortDescription()))}' + '\033[93m' + 'ERROR' + '\033[0m')
        elif self.dots:
            self.stream.write('E')
            self.stream.flush()

    def addFailure(self, test: TestCase, err) -> None:
        """
        If the test fails, this is printed to its line in the terminal, and a result is added to the results object
        :param test: test case class
        """
        super(TextTestResult, self).addFailure(test, err)
        if self.showAll:
            # pretty printing
            self.stream.writeln(f'{" " * (120 - len(test.shortDescription()))}' + '\033[91m' + 'FAIL' + '\033[0m')
        elif self.dots:
            self.stream.write('F')
            self.stream.flush()

    def printErrors(self) -> None:
        """
        Prints stack traces and variable info of every failure and error.
        """
        if self.dots or self.showAll:
            if len(self.errors) > 0 or len(self.failures) > 0:
                # print blank line if no errors or failures are present
                self.stream.writeln()
        self.printErrorList('ERROR', self.errors)
        self.printErrorList('FAIL', self.failures)


class GIRAFTestRunner(TextTestRunner):
    """
    Modifications to the base TextTestRunner class. This class is in charge of running the tests.
    """

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

        # pretty printing
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
