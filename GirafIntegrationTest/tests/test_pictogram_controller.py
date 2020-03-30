from requests import get, post, put, delete
import time
from testlib import order, BASE_URL, auth, GIRAFTestCase, parse_image

citizen_token = ''
new_picto_id = ''
new_picto_name = f'fish{time.time()}'


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
        cls.PICTOGRAMS = [{'id': 1, 'lastEdit': '2020-03-26T09:22:28.252106Z', 'title': 'Epik', 'accessLevel': 1,
                           'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/1/image/raw'},
                          {'id': 2, 'lastEdit': '2020-03-26T09:22:28.275836Z', 'title': 'som', 'accessLevel': 1,
                           'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/2/image/raw'},
                          {'id': 5, 'lastEdit': '2020-03-26T09:22:28.275793Z', 'title': 'simpelt', 'accessLevel': 1,
                           'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/5/image/raw'},
                          {'id': 8, 'lastEdit': '2020-03-26T09:22:28.275735Z', 'title': 'sejt', 'accessLevel': 1,
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
    def test_pictogram_pictogram_login_as_citizen(self):
        """
        Testing logging in as Citizen

        Endpoint: POST:/v1/Account/login
        """
        global citizen_token
        data = {'username': 'Kurt', 'password': 'password'}
        response = post(f'{BASE_URL}v1/Account/login', json=data).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        citizen_token = response['data']

    @order
    def test_pictogram_pictogram_get_all_pictograms(self):
        """
        Testing getting all pictograms

        Endpoint: GET:/v1/Pictogram
        """
        params = {'page': 1, 'pageSize': 10}
        response = get(f'{BASE_URL}v1/Pictogram', headers=auth(citizen_token), params=params).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        for picto in self.PICTOGRAMS:
            self.assertIn(picto, response['data'])

    @order
    def test_pictogram_get_single_public_pictogram(self):
        """
        Testing getting single public pictogram

        Endpoint: GET:/v1/Pictogram/{id}
        """
        response = get(f'{BASE_URL}v1/Pictogram/2', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertDictEqual(self.PICTOGRAMS[1], response['data'])

    @order
    def test_pictogram_add_pictogram_invalid_access_level(self):
        """
        Testing adding pictogram with invalid access level

        Endpoint: POST:/v1/Pictogram
        """
        response = post(f'{BASE_URL}v1/Pictogram', json=self.FISH, headers=auth(citizen_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'MissingProperties')

    @order
    def test_pictogram_add_pictogram_private(self):
        """
        Testing adding private pictogram

        Endpoint: POST:/v1/Pictogram
        """
        global new_picto_id
        self.FISH['accessLevel'] = 3
        response = post(f'{BASE_URL}v1/Pictogram', json=self.FISH, headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertNotEqual(self.FISH['lastEdit'], response['data']['lastEdit'])
        self.assertNotEqual(self.FISH['id'], response['data']['id'])
        new_picto_id = response['data']['id']

    @order
    def test_pictogram_get_new_picto_pictogram(self):
        """
        Testing getting the newly added pictogram

        Endpoint: GET:/v1/Pictogram/{id}
        """
        response = get(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(new_picto_name, response['data']['title'])

    @order
    def test_pictogram_update_new_picto_pictogram(self):
        """
        Testing updating the newly added pictogram

        Endpoint: PUT:/v1/Pictogram/{id}
        """
        global new_picto_name
        new_picto_name = f'cursed_{new_picto_name}'
        self.FISH['title'] = new_picto_name
        response = put(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token), json=self.FISH).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_pictogram_check_updated_new_picto_pictogram(self):
        """
        Testing checking if update was successful

        Endpoint: GET:/v1/Pictogram/{id}
        """
        response = get(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])
        self.assertEqual(new_picto_name, response['data']['title'])

    @order
    def test_pictogram_get_public_pictogram_image(self):
        """
        Testing getting image of public pictogram

        Endpoint: GET:/v1/Pictogram/{id}/image
        """
        response = get(f'{BASE_URL}v1/Pictogram/2/image', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')
        self.assertIsNotNone(response['data'])

    @order
    def test_pictogram_get_raw_public_pictogram_image(self):
        """
        Testing getting raw image of public pictogram

        Endpoint: GET:/v1/Pictogram/{id}/image/raw
        """
        response = get(f'{BASE_URL}v1/Pictogram/6/image/raw', headers=auth(citizen_token))
        image = parse_image(response.content)
        self.assertTupleEqual(image.size, (200, 200))
        self.assertEqual(image.format, 'PNG')

    @order
    def test_pictogram_update_new_picto_image(self):
        """
        Testing updating image of newly added pictogram

        Endpoint: PUT:/v1/Pictogram/{id}/image
        """
        response = put(f'{BASE_URL}v1/Pictogram/{new_picto_id}/image', headers=auth(citizen_token), data=self.RAW_IMAGE).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_pictogram_delete_new_picto_pictogram(self):
        """
        Testing deleting the newly added pictogram

        Endpoint: DELETE:/v1/Pictogram/{id}
        """
        response = delete(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token)).json()
        self.assertTrue(response['success'])
        self.assertEqual(response['errorKey'], 'NoError')

    @order
    def test_pictogram_ensure_new_picto_pictogram_deleted(self):
        """
        Testing checking if the pictogram was deleted

        Endpoint: GET:/v1/Pictogram/{id}
        """
        response = get(f'{BASE_URL}v1/Pictogram/{new_picto_id}', headers=auth(citizen_token)).json()
        self.assertFalse(response['success'])
        self.assertEqual(response['errorKey'], 'PictogramNotFound')

