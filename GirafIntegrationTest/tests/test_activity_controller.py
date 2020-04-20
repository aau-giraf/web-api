import json
import time

from testlib import GIRAFTestCase, order, BASE_URL, auth
from requests import get, post, put, delete


class TestActivity(GIRAFTestCase):

    @classmethod
    def setUpClass(cls) -> None:
        super(TestActivity, cls).setUpClass()
        print(f'file:/{__file__}\n')

    @order
    def test_activity_can_login_as_citizen(self):
        pass


    @classmethod
    def tearDownClass(cls) -> None:
        super(TestActivity, cls).tearDownClass()
