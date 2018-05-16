from testLib import *
from integrate import TestCase, test
import time


def auth(token):
    return {"Authorization": "Bearer {0}".format(token)}


class WeekTemplateControllerTest(TestCase):
    "Week Template Controller Test"
    graatand = None
    aliceUsername = None
    alice = None

    @test()
    def logins(self, check):
        "logins"
        response = login('Graatand', check)
        self.graatand = response

        self.aliceUsername = 'Alice{0}'.format(str(time.time()))

        response = requests.post(Test.url + 'account/register', headers=auth(self.graatand),
                                 json={"username": self.aliceUsername, "password": "password", "role": "Citizen",
                                       "departmentId": 2}).json()
        ensureSuccess(response, check)

        self.alice = login(self.aliceUsername, check)

        response = requests.get(Test.url + 'User', headers=auth(self.alice)).json()

    @test(skip_if_failed=['logins'])
    def getTemplates(self, check):
        'Get all available templates'
        response = requests.get(Test.url + 'WeekTemplate', headers=auth(self.graatand)).json()
        ensureSuccess(response, check)

        check.equal("SkabelonUge", response['data'][0]['name'])
        check.equal(1, response['data'][0]['templateId'])

    @test(skip_if_failed=['logins'])
    def getSpecificTemplate(self, check):
        'Get template by id'
        response = requests.get(Test.url + 'WeekTemplate/1', headers=auth(self.graatand)).json()
        ensureSuccess(response, check)

        check.equal("SkabelonUge", response['data']['name'])
        check.equal(1, response['data']['thumbnail']['id'])
        check.equal(1, response['data']['days'][0]['day'])
        check.equal(6, response['data']['days'][5]['day'])
        check.equal(70, response['data']['days'][4]['activities'][1]['pictogram']['id'])

    @test(skip_if_failed=['logins'])
    def getTemplateFromOutsideDepartment(self, check):
        'Try to get template outside department by id'
        response = requests.get(Test.url + 'WeekTemplate/1', headers=auth(self.alice)).json()

        ensureError(response, check)

    templateId = None

    @test(skip_if_failed=['logins'])
    def postNewTemplate(self, check):
        'Post a new template'
        template1DTO = {
            "thumbnail": {"id": 28},
            "name": "Template1",
            "days": [
                {
                    "day": "Monday",
                    "activities": [
                        {
                            "pictogram": {"id": 1},
                            "order": 0, "state": "Active"
                        },
                        {
                            "pictogram": {"id": 6},
                            "order": 0, "state": "Active"
                        },
                    ]
                },
                {
                    "day": "Friday",
                    "activities": [
                        {
                            "pictogram": {"id": 2},
                            "order": 0, "state": "Active"
                        },
                        {
                            "pictogram": {"id": 7},
                            "order": 0, "state": "Active"
                        },
                    ]
                },
            ]
        }

        response = requests.post(Test.url + 'WeekTemplate', headers=auth(self.graatand), json=template1DTO).json()

        ensureSuccess(response, check)
        self.templateId = response['data']['id']

        response = requests.get(Test.url + 'WeekTemplate/{0}'.format(self.templateId),
                                headers=auth(self.graatand)).json()
        ensureSuccess(response, check)

        check.equal("Template1", response['data']['name'])
        check.equal(28, response['data']['thumbnail']['id'])
        check.equal(6, response['data']['days'][0]['activities'][1]['pictogram']['id'])
        check.equal(7, response['data']['days'][1]['activities'][1]['pictogram']['id'])

    @test(skip_if_failed=['postNewTemplate'])
    def updateTemplate(self, check):
        'Put new stuff into the template'
        template2DTO = {
            "thumbnail": {"id": 29},
            "name": "Template2",
            "days": [
                {
                    "day": "Monday",
                    "activities": [
                        {
                            "pictogram": {"id": 2},
                            "order": 1, "state": "Active"
                        },
                        {
                            "pictogram": {"id": 7},
                            "order": 2, "state": "Active"
                        },
                    ]
                },
                {
                    "day": "Friday",
                    "activities": [
                        {
                            "pictogram": {"id": 3},
                            "order": 1, "state": "Active"
                        },
                        {
                            "pictogram": {"id": 8},
                            "order": 2, "state": "Active"
                        },
                    ]
                },
            ]
        }

        response = requests.put(Test.url + 'WeekTemplate/{0}'.format(self.templateId),
                                headers=auth(self.graatand),
                                json=template2DTO).json()

        ensureSuccess(response, check)

        response = requests.get(Test.url + 'WeekTemplate/{0}'.format(self.templateId),
                                headers=auth(self.graatand)).json()

        ensureSuccess(response, check)

        check.equal("Template2", response['data']['name'])
        check.equal(29, response['data']['thumbnail']['id'])
        check.equal(7, response['data']['days'][0]['activities'][1]['pictogram']['id'])
        check.equal(8, response['data']['days'][1]['activities'][1]['pictogram']['id'])

    @test(skip_if_failed=['postNewTemplate'], depends=['updateTemplate'])
    def deleteTemplate(self, check):
        'Delete the template again.'
        response = requests.delete(Test.url + 'WeekTemplate/{0}'.format(self.templateId),
                                   headers=auth(self.graatand)).json()
        ensureSuccess(response, check)

        response = requests.get(Test.url + 'WeekTemplate/{0}'.format(self.templateId),
                                headers=auth(self.graatand)).json()
        ensureErrorWithKey(response, check, "NoWeekTemplateFound")
