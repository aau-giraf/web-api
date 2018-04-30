# -*- coding: utf-8 -*-
from testLib import *
from integrate import TestCase, test
import time


def auth(token):
    return {"Authorization": "Bearer {0}".format(token)}


# Make sure the pictogram isn't in the list
def pictogramIsInList(pictogramId, pictogramList):
    for pictogram in pictogramList:
        if pictogram == pictogramId:
            return True
    return False


class DepartmentControllerTest(TestCase):
    "Department Controller"
    url = Test.url
    dalgaardsholmstuen = 'Dalgaardsholmstuen{0}'.format(str(time.time()))

    lee = None
    graatand = None
    kurt = None

    @test()
    def logins(self, check):
        "Login as Lee, Graatand and Kurt"
        response = requests.post(Test.url + 'account/login', json={"username": "Lee", "password": "password"}).json()
        ensureSuccess(response, check)
        self.lee = response['data']

        response = requests.post(Test.url + 'account/login',
                                 json={"username": "Graatand", "password": "password"}).json()
        ensureSuccess(response, check)
        self.graatand = response['data']

        response = requests.post(Test.url + 'account/login', json={"username": "Kurt", "password": "password"}).json()
        ensureSuccess(response, check)
        self.kurt = response['data']

    numberOfDepartments = None

    @test()
    def departmentList(self, check):
        'Get list of departments'
        response = requests.get(Test.url + 'Department').json()
        if ensureSuccess(response, check):
            self.numberOfDepartments = len(response['data'])
        else:
            self.numberOfDepartments = 0

    @test(skip_if_failed=['logins', 'departmentList'])
    def unauthorizedDepartmentCreation0(self, check):
        'Graatand tries to create department'

        response = requests.post(Test.url + 'Department', json={
            "id": 0,
            "name": self.dalgaardsholmstuen,
            "members": [],
            "resources": []
        }, headers=auth(DepartmentControllerTest.graatand)).json()

        ensureError(response, check)

    @test(skip_if_failed=['logins', 'departmentList'])
    def unauthorizedDepartmentCreation1(self, check):
        'Kurt tries to create department'
        response = requests.post(Test.url + 'Department', json={
            "id": 0,
            "name": self.dalgaardsholmstuen,
            "members": [],
            "resources": []
        }, headers=auth(self.kurt)).json()

        ensureError(response, check)

    @test(skip_if_failed=['unauthorizedDepartmentCreation0', 'unauthorizedDepartmentCreation1'])
    def checkDepartmentCount(self, check):
        'Make sure number of departments has remained the same'
        response = requests.get(Test.url + 'Department').json()

        ensureSuccess(response, check)
        check.equal(self.numberOfDepartments, len(response['data']), 'Number of departments.\n')

    dalgardsholmstuenId = None
    dalgaardsholmstuenToken = None

    @test(skip_if_failed=['checkDepartmentCount'])
    def newDepartment(self, check):
        'Lee creates department'
        response = requests.post(Test.url + 'Department', json={
            "id": 0,
            "name": self.dalgaardsholmstuen,
            "members": [],
            "resources": []
        }, headers=auth(self.lee)).json()

        ensureSuccess(response, check)

        self.dalgardsholmstuenId = response.get('data').get('id')

        check.is_not_none(self.dalgardsholmstuenId, message='Could not get ID of Dalgaardsholmstuen')

    @test(skip_if_failed=['newDepartment'])
    def departmentLogin(self, check):
        response = requests.post(Test.url + 'account/login',
                                 json={"username": self.dalgaardsholmstuen, "password": "0000"}).json()
        ensureSuccess(response, check)
        self.dalgaardsholmstuenToken = response['data']

    @test(skip_if_failed=['newDepartment'])
    def getDepartment(self, check):
        'Get the newly created Dalgaardsholmstuen'
        response = requests.get(Test.url + 'Department/{0}'.format(self.dalgardsholmstuenId)).json()
        ensureSuccess(response, check)
        check.equal(response.get('data').get('name'), self.dalgaardsholmstuen,
                    message='Name should\'ve been {0} but was {1}'.format(self.dalgaardsholmstuen,
                                                                          response.get('name')))

    @test()
    def nullDepartment(self, check):
        'Get nonexistent department'
        response = requests.get(Test.url + 'Department/-1').json()
        ensureError(response, check)

    gunnarUsername = None
    gunnarID = None
    gunnarBody = None
    gunnar = None

    @test(skip_if_failed=['newDepartment'])
    def newGunnar(self, check):
        'Register Gunnar to that department'
        self.gunnarUsername = 'Gunnar{0}'.format(str(time.time()))

        response = requests.post(Test.url + 'account/register', json={
            "username": self.gunnarUsername,
            "password": "password",
            "departmentId": self.dalgardsholmstuenId
        }).json()

        ensureSuccess(response, check)

        response = requests.post(Test.url + 'account/login',
                                 json={"username": self.gunnarUsername, "password": "password"}).json()
        ensureSuccess(response, check)
        self.gunnar = response['data']

        response = requests.get(Test.url + 'User', headers=auth(self.gunnar)).json()

        self.gunnarID = response.get('data').get('id')

        check.is_not_none(self.gunnarID, 'Gunnar\'s ID.\n')

        # Data that has Gunnars ID in it.
        self.gunnarBody = {
            "role": -1,
            "guardians": None,
            "id": self.gunnarID,
            "username": self.gunnarUsername,
            "screenName": None,
            "userIcon": None,
            "department": -1,
            "weekScheduleIds": []
        }

        # Find Gunnar among users
        response = requests.get(Test.url + 'Department/{0}'.format(self.dalgardsholmstuenId)).json()
        ensureSuccess(response, check)

        gunnarFound = False
        for member in response['data']['members']:
            if member['userName'] == self.gunnarUsername and member['userId'] == self.gunnarID:
                gunnarFound = True

        check.is_true(gunnarFound, message='Gunnar was not included in list of department members.')

    cyclopianBody = None

    @test(skip_if_failed=['newDepartment'])
    def newPictogram(self, check):
        'Department posts private Cyclopian pictogram'
        body = {
            "accessLevel": 3,
            "title": "Cyclopian",
            "id": -1,
            "lastEdit": "2018-03-19T10:40:26.587Z"
        }
        response = requests.post(Test.url + 'pictogram', json=body, headers={"Authorization": "Bearer {0}".format(
            self.dalgaardsholmstuenToken)}).json()
        ensureSuccess(response, check)
        self.cyclopianBody = response['data']

    @test(skip_if_failed=['logins', 'newPictogram'])
    def unauthorizedPictogramAdd(self, check):
        'Kurt tries to add Cyclopian to Dalgaardsholmstuen'
        response = requests.post(Test.url + 'Department/{0}/resource/{1}'
                                 .format(self.dalgardsholmstuenId, self.cyclopianBody['id']),
                                 headers=auth(self.kurt)).json()
        ensureError(response, check)

        # Check that nothing's changed in database
        response = requests.get(Test.url + 'Department/{0}'.format(self.dalgardsholmstuenId)).json()
        check.is_false(pictogramIsInList(self.cyclopianBody['id'], response['data']['resources']),
                       message='Pictogram was found in department resources, but should not have been added')

    @test(skip_if_failed=['unauthorizedPictogramAdd'])
    def unauthorizedPictogramAdd1(self, check):
        'Gunnar tries to add Cyclopian to Dalgaardsholmstuen'
        response = requests.post(Test.url + 'Department/{0}/resource/{1}'
                                 .format(self.dalgardsholmstuenId, self.cyclopianBody['id']),
                                 json=self.cyclopianBody, headers=auth(self.gunnar)).json()
        ensureError(response, check)

        # Check that nothing's changed in database
        response = requests.get(Test.url + 'Department/{0}'.format(self.dalgardsholmstuenId)).json()
        check.is_false(pictogramIsInList(self.cyclopianBody['id'], response['data']['resources']),
                       message='Pictogram was found in department resources, but should not have been added')

    @test(skip_if_failed=['logins', 'newDepartment'])
    def unauthorizedPictogramAdd2(self, check):
        'Kurt tries to add his own pictogram to Dalgaardsholmstuen'
        body = {
            "accessLevel": 1,
            "title": "Squirmy and Rugose",
            "id": -1,
            "lastEdit": "2018-03-19T10:40:26.587Z"
        }
        response = requests.post(Test.url + 'pictogram', json=body, headers={"Authorization": "Bearer {0}".format(
            self.dalgaardsholmstuenToken)}).json()
        ensureSuccess(response, check)
        pictogram = response['data']

        response = requests.post(Test.url + 'Department/{0}/resource/{1}'
                                 .format(self.dalgardsholmstuenId, pictogram['id']),
                                 json=pictogram,
                                 headers=auth(self.gunnar)).json()
        ensureError(response, check)

        # Check that nothing's changed in database
        response = requests.get(Test.url + 'Department/{0}'.format(self.dalgardsholmstuenId)).json()
        check.is_false(pictogramIsInList(pictogram['id'], response['data']['resources']),
                       message='Pictogram was found in department resources, but should not have been added')

    @test(skip_if_failed=['logins', 'newDepartment'])
    def unauthorizedPictogramAdd3(self, check):
        'Gunnar tries to add his own pictogram to Dalgaardsholmstuen'
        body = {
            "accessLevel": 1,
            "title": "The End of the World as We Know It",
            "id": -1,
            "lastEdit": "2018-03-19T10:40:26.587Z"
        }
        response = requests.post(Test.url + 'pictogram', json=body, headers={"Authorization": "Bearer {0}".format(
            self.gunnar)}).json()
        ensureSuccess(response, check)
        pictogram = response['data']

        response = requests.post(Test.url + 'Department/{0}/resource/{1}'
                                 .format(self.dalgardsholmstuenId, pictogram['id']),
                                 json=pictogram,
                                 headers=auth(self.gunnar)).json()
        ensureError(response, check)

    @test(skip_if_failed=['logins', 'newDepartment'], expect_fail=True)
    def unauthorizedPictogramAdd4(self, check):
        'Gunnar tries to add a pictogram directly through pictogram controller to Dalgaardsholmstuen'
        body = {
            "accessLevel": 2,
            "title": "The End of the World as We Know It",
            "id": -1,
            "lastEdit": "2018-03-19T10:40:26.587Z"
        }
        response = requests.post(Test.url + 'pictogram', json=body, headers={"Authorization": "Bearer {0}".format(
            self.gunnar)}).json()
        ensureSuccess(response, check)
        pictogram = response['data']

        # Check that nothing's changed in database
        response = requests.get(Test.url + 'Department/{0}'.format(self.dalgardsholmstuenId)).json()
        check.is_false(pictogramIsInList(pictogram['id'], response['data']['resources']),
                       message='Pictogram was found in department resources, but should not have been added')

    @test(skip_if_failed=['unauthorizedPictogramAdd', 'unauthorizedPictogramAdd1'])
    def pictogramAdd(self, check):
        'Add Cyclopian to Dalgaardsholmstuen'
        response = requests.post(Test.url + 'Department/{0}/resource/{1}'
                                 .format(self.dalgardsholmstuenId, self.cyclopianBody['id']),
                                 headers=auth(self.dalgaardsholmstuenToken)).json()
        ensureSuccess(response, check)

        # Check that something's changed in database
        response = requests.get(Test.url + 'Department/{0}'.format(self.dalgardsholmstuenId)).json()
        check.is_true(pictogramIsInList(self.cyclopianBody['id'], response['data']['resources']),
                      message='Pictogram was not found in department resources')

    @test(skip_if_failed=['pictogramAdd'])
    def unauthorizedPictogramRemove(self, check):
        'Gunnar tries to remove Cyclopian from Dalgaardsholmstuen'
        response = requests.delete(Test.url + 'Department/resource'.format(self.dalgardsholmstuenId),
                                   json=self.cyclopianBody,
                                   headers=auth(self.gunnar)).json()
        ensureError(response, check)

        # Check that nothing's changed in database
        response = requests.get(Test.url + 'Department/{0}'.format(self.dalgardsholmstuenId)).json()
        check.is_true(pictogramIsInList(self.cyclopianBody['id'], response['data']['resources']),
                      message='Pictogram was not found in department resources')

    @test(skip_if_failed=['unauthorizedPictogramRemove'])
    def pictogramRemove(self, check):
        'Remove Cyclopian from Dalgaardsholmstuen'
        response = requests.delete(Test.url + 'Department/resource/{0}'.format(self.cyclopianBody['id']),
                                   headers=auth(self.dalgaardsholmstuenToken)).json()
        ensureSuccess(response, check)
