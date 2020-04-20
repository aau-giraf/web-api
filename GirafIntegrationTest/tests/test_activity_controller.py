import json

from testlib import GIRAFTestCase, order, BASE_URL, auth
from requests import get, post, put, delete


class TestActivity(GIRAFTestCase):

    @classmethod
    def setUpClass(cls) -> None:
        super(TestActivity, cls).setUpClass()
        print(f'file:/{__file__}\n')

    @order
    def test_activity_can_login_as_citizen(self):
        guardian_token = ''
        data = {'username': 'Graatand', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        guardian_token = response['data']

        """
        Testing getting specific template

        Endpoint: GET:/v2/User/{userId}/week/
        """
        response = get(f'{BASE_URL}v2/User/1/week', headers=auth(guardian_token)).json()
        with open('response.json', 'w') as f:
            f.write(json.dumps(response))


    @classmethod
    def tearDownClass(cls) -> None:
        super(TestActivity, cls).tearDownClass()
