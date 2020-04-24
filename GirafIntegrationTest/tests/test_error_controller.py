from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase

class TestErrorController(GIRAFTestCase):
    """
    Testing API requests on Error endpoints.
    """
    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestErrorController, cls).setUpClass()
        print(f'file:/{__file__}\n')


    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestErrorController, cls).tearDownClass()

    @order
    def test_ERROR_can_get_error(self):
        """
        Testing GET error

        Endpoint: GET:/v1/Error
        """
        response = get(f'{BASE_URL}v1/Error').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_ERROR_can_put_error(self):
        """
        Testing PUT error

        Endpoint: PUT:/v1/Error
        """
        response = put(f'{BASE_URL}v1/Error').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_ERROR_can_post_error(self):
        """
        Testing POST error

        Endpoint: POST:/v1/Error
        """
        response = post(f'{BASE_URL}v1/Error').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_ERROR_can_delete_error(self):
        """
        Testing DELETE error

        Endpoint: DELETE:/v1/Error
        """
        response = delete(f'{BASE_URL}v1/Error').json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

    @order
    def test_error_lul(self):
        """
        Doing dumb shit here. This test should get redirected to the Error endpoint.

        Endpoint: DELETE:/v1/Error
        """
        response = delete(f'{BASE_URL}hahahaha/')
        x = response.url
        response = response.json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'NotFound')

