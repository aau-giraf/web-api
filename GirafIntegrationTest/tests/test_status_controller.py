from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase
from http import HTTPStatus

class TestStatusController(GIRAFTestCase):
    """
    Testing API requests on Error endpoints.
    """
    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestStatusController, cls).setUpClass()
        print(f'file:/{__file__}\n')

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestStatusController, cls).tearDownClass()

    @order
    def test_status_is_online(self):
        """
        Testing GET for status on web-api

        Endpoint: GET:/v1/Status
        """
        response = get(f'{BASE_URL}v1/status')
        
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_status_webapi_database_connection_is_online(self):
        """
        Testing GET for status on web-api connection to database

        Endpoint: GET:/v1/Status/database
        """
        response = get(f'{BASE_URL}v1/status/database')
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_status_versioninfo_is_online(self):
        """
        Testing GET for status on web-api

        Endpoint: GET:/v1/Status/version-info
        """
        response = get(f'{BASE_URL}v1/status/version-info')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])