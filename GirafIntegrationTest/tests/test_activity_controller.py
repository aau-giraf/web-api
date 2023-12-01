from requests import get, post, put, delete, patch
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase
from unittest import skip
from http import HTTPStatus

guardian_token = ""
user_id = ""
user_token = ""
activity_id = ""
class TestActivityController(GIRAFTestCase):
    """
    Testing API requests on Activity endpoints
    """
    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestActivityController, cls).setUpClass()
        print(f'file:/{__file__}\n')
        cls.weekplan_name = 'Normal Uge'
        cls.week_year = 0
        cls.week_number = 0
        cls.week_day_number = 1

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestActivityController, cls).tearDownClass()

    @order
    def test_activity_can_login_as_guardian(self):
        """
        Testing logging in as Guardian

        Endpoint: POST:/v2/Account/login
        """
        global guardian_token
        data = {'username': 'graatand', 'password': 'password'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        guardian_token = response_body['data']

    @order
    def test_activity_can_login_as_citizen1(self):
        """
        Testing logging in as Citizen1

        Endpoint: POST:/v2/Account/login
        """
        global user_token
        data = {'username': 'Kurt', 'password': 'password'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        user_token = response_body['data']

    @order
    def test_activity_can_get_citizen1_id(self):
        """
        Testing getting Citizen1's id

        Endpoint: GET:/v1/User
        """
        global user_id
        response = get(f'{BASE_URL}v1/User', headers=auth(user_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])
        user_id = response_body['data']['id']

    @order
    @skip("Skipping since endpoint is broken")
    def test_activity_set_new_user_activity(self):
        """
        Testing creation of user specific activity

        Endpoint: POST:/v2/Activity/{user_id}/{weekplan_name}/{week_year}/{week_number}/{week_day_number}
        """
        global activity_id
        data = {"pictogram": {"id": 1}}
        response = post(f'{BASE_URL}v2/Activity/{user_id}/{self.weekplan_name}/{self.week_year}/{self.week_number}/{self.week_day_number}', headers=auth(guardian_token), json=data,)
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.CREATED)
        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])
        activity_id = response_body['data']['id']


    @order
    @skip("Skipping since test is broken")
    def test_activity_update_user_activity(self):
        """
        Testing PATCH update to activity for a specific user

        Endpoint: PATCH:/v2/Activity/{user_id}/update
        """

        data = {'pictogram': {'id': 6}, 'id': activity_id}
        response = patch(f'{BASE_URL}v2/Activity/{user_id}/update', json=data,
                         headers=auth(guardian_token))
        
        self.assertEqual(response.status_code, HTTPStatus.OK)

    @order
    @skip("Skipping since test is broken")
    def test_activity_delete_user_activity(self):
        """
        Testing DELETE on user specific activity

        Endpoint: DELETE:/v2/Activity/{user_id}/delete/{activity_id}
        """

        response = delete(f'{BASE_URL}v2/Activity/{user_id}/delete/{activity_id}', headers=auth(guardian_token))
        
        self.assertEqual(response.status_code, HTTPStatus.OK)
