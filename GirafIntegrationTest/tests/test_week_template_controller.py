from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase
from http import HTTPStatus

guardian_token = ''
citizen_token = ''
citizen_username = f'Alice{time.time()}'
template_id = 0


class TestWeekTemplateController(GIRAFTestCase):
    """
    Testing API requests on WeekTemplate endpoints
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestWeekTemplateController, cls).setUpClass()
        print(f'file:/{__file__}\n')
        cls.TEMPLATES = [{'thumbnail': {'id': 28}, 'name': 'Template1', 'days':
                         [{'day': 'Monday', 'activities': [{'pictograms': [{'id': 1}], 'order': 0, 'state': 'Active'},
                                                           {'pictograms': [{'id': 6}], 'order': 0, 'state': 'Active'}]},
                         {'day': 'Friday', 'activities': [{'pictograms': [{'id': 2}], 'order': 0, 'state': 'Active'},
                                                          {'pictograms': [{'id': 7}], 'order': 0, 'state': 'Active'}]}]},
                         {'thumbnail': {'id': 29}, 'name': 'Template2', 'days':
                         [{'day': 'Monday', 'activities': [{'pictograms': [{'id': 2}], 'order': 1, 'state': 'Active'},
                                                           {'pictograms': [{'id': 7}], 'order': 2, 'state': 'Active'}]},
                          {'day': 'Friday', 'activities': [{'pictograms': [{'id': 3}], 'order': 1, 'state': 'Active'},
                                                           {'pictograms': [{'id': 8}], 'order': 2, 'state': 'Active'}]}]}]

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestWeekTemplateController, cls).tearDownClass()

    @order
    def test_week_template_can_login_as_guardian(self):
        """
        Testing logging in as Guardian

        Endpoint: POST:/v2/Account/login
        """
        global guardian_token
        data = {'username': 'Graatand', 'password': 'password'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        guardian_token = response_body['data']

    @order
    def test_week_template_can_register_citizen(self):
        """
        Testing registering Citizen

        Endpoint: POST:/v2/Account/register
        """
        data = {'username': citizen_username, 'displayname': citizen_username, 'password': 'password', 'role': 'Citizen', 'departmentId': 1}
        response = post(f'{BASE_URL}v2/Account/register', headers=auth(guardian_token), json=data)

        self.assertEqual(response.status_code, HTTPStatus.CREATED)


    @order
    def test_week_template_can_login_as_citizen(self):
        """
        Testing logging in as Citizen

        Endpoint: POST:/v2/Account/login
        """
        global citizen_token
        data = {'username': citizen_username, 'password': 'password'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        citizen_token = response_body['data']

    @order
    def test_week_template_can_get_all_templates(self):
        """
        Testing getting all templates

        Endpoint: GET:/v1/WeekTemplate
        """
        response = get(f'{BASE_URL}v1/WeekTemplate', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual('SkabelonUge', response_body['data'][0]['name'])
        self.assertEqual(1, response_body['data'][0]['templateId'])

    @order
    def test_week_template_can_get_specific_template(self):
        """
        Testing getting specific template

        Endpoint: GET:/v1/WeekTemplate/{id}
        """
        response = get(f'{BASE_URL}v1/WeekTemplate/1', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual('SkabelonUge', response_body['data']['name'])
        self.assertEqual(77, response_body['data']['thumbnail']['id'])
        self.assertEqual(1, response_body['data']['days'][0]['day'])
        self.assertEqual(6, response_body['data']['days'][5]['day'])

        # No activities are found in the days of the template, is this a mistake in the sample data?
        # self.assertEqual(70, response_body['data']['days'][4]['activities'][1]['pictogram']['id'])

    @order
    def test_week_template_can_get_template_outside_department_should_fail(self):
        """
        Testing getting template from outside department

        Endpoint: GET:/v1/WeekTemplate/{id}
        """
        response = get(f'{BASE_URL}v1/WeekTemplate/1', headers=auth(citizen_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)
        self.assertEqual(response_body['errorKey'], 'NoWeekTemplateFound')

    @order
    def test_week_template_can_add_new_template(self):
        """
        Testing adding new template

        Endpoint: POST:/v1/WeekTemplate
        """
        global template_id
        response = post(f'{BASE_URL}v1/WeekTemplate', headers=auth(guardian_token), json=self.TEMPLATES[0])
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.CREATED)

        self.assertIsNotNone(response_body['data'])
        self.assertIsNotNone(response_body['data']['id'])
        template_id = response_body['data']['id']

    @order
    def test_week_template_ensure_template_is_added(self):
        """
        Testing ensuring template has been added

        Endpoint: GET:/v1/WeekTemplate/{id}
        """
        response = get(f'{BASE_URL}v1/WeekTemplate/{template_id}', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(28, response_body['data']['thumbnail']['id'])
        self.assertEqual(6, response_body['data']['days'][0]['activities'][1]['pictograms'][0]['id'])
        self.assertEqual(7, response_body['data']['days'][1]['activities'][1]['pictograms'][0]['id'])

    @order
    def test_week_template_can_update_template(self):
        """
        Testing updating template

        Endpoint: PUT:/v1/WeekTemplate/{id}
        """
        response = put(f'{BASE_URL}v1/WeekTemplate/{template_id}', headers=auth(guardian_token),
                       json=self.TEMPLATES[1])
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])


    @order
    def test_week_template_ensure_template_is_updated(self):
        """
        Testing ensuring template has been updated

        Endpoint: GET:/v1/WeekTemplate/{id}
        """
        response = get(f'{BASE_URL}v1/WeekTemplate/{template_id}', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(29, response_body['data']['thumbnail']['id'])
        self.assertEqual(7, response_body['data']['days'][0]['activities'][1]['pictograms'][0]['id'])
        self.assertEqual(8, response_body['data']['days'][1]['activities'][1]['pictograms'][0]['id'])

    @order
    def test_week_template_can_delete_template(self):
        """
        Testing deleting template

        Endpoint: DELETE:/v1/WeekTemplate/{id}
        """
        response = delete(f'{BASE_URL}v1/WeekTemplate/{template_id}', headers=auth(guardian_token))

        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_week_template_ensure_template_is_deleted(self):
        """
        Testing ensuring template has been deleted

        Endpoint: GET:/v1/WeekTemplate/{id}
        """
        response = get(f'{BASE_URL}v1/WeekTemplate/{template_id}', headers=auth(guardian_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)
        self.assertEqual(response_body['errorKey'], 'NoWeekTemplateFound')
