from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase
from http import HTTPStatus

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
        response = get(f'{BASE_URL}v1/Error')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
        self.assertEqual(response_body['errorKey'], 'UnknownError')

    @order
    def test_ERROR_can_put_error(self):
        """
        Testing PUT error

        Endpoint: PUT:/v1/Error
        """
        response = put(f'{BASE_URL}v1/Error')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
        self.assertEqual(response_body['errorKey'], 'UnknownError')

    @order
    def test_ERROR_can_post_error(self):
        """
        Testing POST error

        Endpoint: POST:/v1/Error
        """
        response = post(f'{BASE_URL}v1/Error')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
        self.assertEqual(response_body['errorKey'], 'UnknownError')

    @order
    def test_ERROR_can_delete_error(self):
        """
        Testing DELETE error

        Endpoint: DELETE:/v1/Error
        """
        response = delete(f'{BASE_URL}v1/Error')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
        self.assertEqual(response_body['errorKey'], 'UnknownError')

    @order
    def test_error_lul(self):
        """
        Doing dumb shit here. This test should get redirected to the Error endpoint.

        Endpoint: DELETE:/v1/Error
        """
        response = delete(f'{BASE_URL}hahahaha/')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)
        self.assertEqual(response_body['errorKey'], 'NotFound')

    @order
    def test_status_code_401(self):
        """
        Testing statuscode 401.

        Endpoint: DELETE:/v1/Error
        """
        params = {'statusCode': 401}
        response = get(f'{BASE_URL}v1/Error', params=params)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_status_code_403(self):
        """
        Testing statuscode 403.

        Endpoint: DELETE:/v1/Error
        """
        params = {'statusCode': 403}
        response = get(f'{BASE_URL}v1/Error', params=params)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'Forbidden')

    @order
    def test_status_code_404(self):
        """
        Testing statuscode 404.

        Endpoint: DELETE:/v1/Error
        """
        params = {'statusCode': 404}
        response = get(f'{BASE_URL}v1/Error', params=params)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)
        self.assertEqual(response_body['errorKey'], 'NotFound')

    @order
    def test_status_code_im_a_teapot(self):
        """
        Testing whether the endpoint can brew coffee.
        (Testing the fallback in the error controller)

        Endpoint: DELETE:/v1/Error
        """
        params = {'statusCode': 418}
        response = get(f'{BASE_URL}v1/Error', params=params)
        response_body = response.json()
        self.assertEqual(response.status_code, 418)
        self.assertEqual(response_body['errorKey'], 'UnknownError')
