from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase, parse_image
from http import HTTPStatus

citizen_token = ''
new_picto_id = ''
new_picto_name = f'fish{time.time()}'

LAST_EDIT_TIMESTAMP = '2020-03-26T09:22:28.252106Z'

class TestPictogramController(GIRAFTestCase):
    """
    Testing API requests on Pictogram endpoints
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestPictogramController, cls).setUpClass()
        print(f'file:/{__file__}\n')
        cls.PICTOGRAMS = [{'id': 1, 'lastEdit': LAST_EDIT_TIMESTAMP, 'title': 'som', 'accessLevel': 1,
                           'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/1/image/raw'},
                          {'id': 2, 'lastEdit': LAST_EDIT_TIMESTAMP, 'title': 'gul', 'accessLevel': 1,
                           'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/2/image/raw'},
                          {'id': 5, 'lastEdit': LAST_EDIT_TIMESTAMP, 'title': 'alle', 'accessLevel': 1,
                           'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/5/image/raw'},
                          {'id': 8, 'lastEdit': LAST_EDIT_TIMESTAMP, 'title': 'berøre', 'accessLevel': 1,
                           'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/8/image/raw'}]
        cls.FISH = {'accessLevel': 0, 'title': new_picto_name, 'id': -1, 'lastEdit': '2099-03-19T10:40:26.587Z'}
        cls.RAW_IMAGE = 'ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿñÇÿÿõ×ÿÿñÇÿÿÿÿÿÿÿ' \
                        'ÿÿÿÿÿÿÿøÿÿüÿÿþ?ÿÿü?ÿÿøÿÿøÿÿà?ÿÿÿ'

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestPictogramController, cls).tearDownClass()

    @order
    def test_pictogram_can_login_as_citizen(self):
        """
        Testing logging in as Citizen

        Endpoint: POST:/v2/Account/login
        """
        global citizen_token
        data = {'username': 'Kurt', 'password': 'password'}
        response = post(f'{BASE_URL}v2/Account/login', json=data)
        response_body = response.json()
        
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        citizen_token = response_body['data']

    @order
    def test_pictogram_can_get_all_pictograms(self):
        """
        Testing getting all pictograms

        Endpoint: GET:/v1/Pictogram
        """
        params = {'page': 1, 'pageSize': 10}
        response = get(f'{BASE_URL}v1/Pictogram', headers=auth(citizen_token), params=params)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])

        collected_pictos = response_body['data']

        # Setting all recieved pictogram's last edit to LAST_EDIT_TIMESTAMP
        for picto in collected_pictos:
            picto['lastEdit'] = LAST_EDIT_TIMESTAMP

        for picto in self.PICTOGRAMS:
            self.assertIn(picto, response_body['data'])

    @order
    def test_pictogram_can_get_single_public_pictogram(self):
        """
        Testing getting single public pictogram

        Endpoint: GET:/v1/Pictogram/{id}
        """
        response = get(f'{BASE_URL}v1/Pictogram/2', headers=auth(citizen_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])

        collected_picto = response_body['data']
        collected_picto['lastEdit'] = LAST_EDIT_TIMESTAMP

        self.assertDictEqual(self.PICTOGRAMS[1], response_body['data'])

    @order
    def test_pictogram_can_add_pictogram_invalid_access_level_should_fail(self):
        """
        Testing adding pictogram with invalid access level

        Endpoint: POST:/v1/Pictogram
        """
        response = post(f'{BASE_URL}v1/Pictogram', json=self.FISH, headers=auth(citizen_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.BAD_REQUEST)
        self.assertEqual(response_body['errorKey'], 'MissingProperties')

    @order
    def test_pictogram_can_add_private_pictogram(self):
        """
        Testing adding private pictogram

        Endpoint: POST:/v1/Pictogram
        """
        global new_picto_id
        self.FISH['accessLevel'] = 3
        response = post(f'{BASE_URL}v1/Pictogram', json=self.FISH, headers=auth(citizen_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.CREATED)

        self.assertIsNotNone(response_body['data'])
        self.assertNotEqual(self.FISH['lastEdit'], response_body['data']['lastEdit'])
        self.assertNotEqual(self.FISH['id'], response_body['data']['id'])
        new_picto_id = response_body['data']['id']

    @order
    def test_pictogram_can_get_new_pictogram(self):
        """
        Testing getting the newly added pictogram

        Endpoint: GET:/v1/Pictogram/{id}
        """
        response = get(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(new_picto_name, response_body['data']['title'])

    @order
    def test_pictogram_can_update_new_pictogram(self):
        """
        Testing updating the newly added pictogram

        Endpoint: PUT:/v1/Pictogram/{id}
        """
        global new_picto_name
        new_picto_name = f'cursed_{new_picto_name}'
        self.FISH['title'] = new_picto_name
        response = put(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token), json=self.FISH)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_pictogram_check_updated_new_pictogram(self):
        """
        Testing checking if update was successful

        Endpoint: GET:/v1/Pictogram/{id}
        """
        response = get(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])
        self.assertEqual(new_picto_name, response_body['data']['title'])

    @order
    def test_pictogram_can_get_public_pictogram_image(self):
        """
        Testing getting image of public pictogram

        Endpoint: GET:/v1/Pictogram/{id}/image
        """
        response = get(f'{BASE_URL}v1/Pictogram/2/image', headers=auth(citizen_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.OK)

        self.assertIsNotNone(response_body['data'])

    @order
    def test_pictogram_can_get_raw_public_pictogram_image(self):
        """
        Testing getting raw image of public pictogram

        Endpoint: GET:/v1/Pictogram/{id}/image/raw
        """
        response = get(f'{BASE_URL}v1/Pictogram/6/image/raw', headers=auth(citizen_token))
        image = parse_image(response.content)
        self.assertTupleEqual(image.size, (200, 200))
        self.assertEqual(image.format, 'PNG')

    @order
    def test_pictogram_can_update_new_pictogram_image(self):
        """
        Testing updating image of newly added pictogram

        Endpoint: PUT:/v1/Pictogram/{id}/image
        """
        response = put(f'{BASE_URL}v1/Pictogram/{new_picto_id}/image', headers=auth(citizen_token), data=self.RAW_IMAGE)
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_pictogram_can_delete_new_pictogram(self):
        """
        Testing deleting the newly added pictogram

        Endpoint: DELETE:/v1/Pictogram/{id}
        """
        response = delete(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token))
        self.assertEqual(response.status_code, HTTPStatus.OK)


    @order
    def test_pictogram_ensure_new_pictogram_deleted(self):
        """
        Testing checking if the pictogram was deleted

        Endpoint: GET:/v1/Pictogram/{id}
        """
        response = get(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token))
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)
        self.assertEqual(response_body['errorKey'], 'PictogramNotFound')


    @order
    def test_pictogram_query_(self):
        """
        Testing querying for a pictogram

        Endpoint: GET:/v1/Pictogram?query=Epik
        """
        params = {'query': 'Epik', 'page': 1, 'pageSize': 10}
        response = get(f'{BASE_URL}v1/Pictogram', headers=auth(citizen_token), params=params)
        response_body = response.json()

        self.assertEqual(response.status_code, HTTPStatus.OK)
        self.assertIsNotNone(response_body['data'])

