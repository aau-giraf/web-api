from integrate import TestCase, test
from testLib import *
import time


class Authorization(TestCase):
    "Authorization Test"

    expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1YmM0YzAzMC1mOGQxLTRhYTAtOTBlOC05MTNhMDYwOTA4YWIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ijg0MTJkOTk1LWIzODEtNGY4My1iZDI1LWU5ODY2NzBiNTdkOSIsImV4cCI6MTUyNTMwMzQyNSwiaXNzIjoibm90bWUiLCJhdWQiOiJub3RtZSJ9.8KXRRqF3B5s8tUki7u5j0TqK-189QIpApdOC6aSxOms"

    @test()
    def callPOST_Account_setpasswordNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Account/set-password"
        response = requests.post(Test.url + 'Account/set-password').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPOST_Account_changepasswordNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Account/change-password"
        response = requests.post(Test.url + 'Account/change-password').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_Account_resetpasswordNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Account/reset-password"
        response = requests.get(Test.url + 'Account/reset-password');
        check.equal(response.status_code, 200) # Aka. responded with HTML and 200
    
    @test()
    def callGET_Account_resetpasswordconfirmationNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Account/reset-password-confirmation"
        response = requests.get(Test.url + 'Account/reset-password-confirmation');
        check.equal(response.status_code, 200) # Aka. responded with HTML and 200    

    @test()
    def callGET_Account_accessdeniedNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Account/access-denied"
        response = requests.get(Test.url + 'Account/access-denied').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_Choice_idNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Choice/{id}"
        response = requests.get(Test.url + 'Choice/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPUT_Choice_idNoAuthNoBody(self, check):
        "Testing authorization of PUT /v1/Choice/{id}"
        response = requests.put(Test.url + 'Choice/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callDELETE_Choice_idNoAuthNoBody(self, check):
        "Testing authorization of DELETE /v1/Choice/{id}"
        response = requests.delete(Test.url + 'Choice/0');
        check.equal(response.status_code, 200)
        check.is_false(response.json()['success'])
        check.equal(response.json()['errorKey'], "NotFound")
    
    @test()
    def callPOST_ChoiceNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Choice"
        response = requests.post(Test.url + 'Choice').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPUT_Day_idNoAuthNoBody(self, check):
        "Testing authorization of PUT /v1/Day/{id}"
        response = requests.put(Test.url + 'Day/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPOST_DepartmentNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Department"
        response = requests.post(Test.url + 'Department').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_Department_id_citizensNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Department/{id}/citizens"
        response = requests.get(Test.url + 'Department/0/citizens').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPOST_Department_user_departmentIDNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Department/user/{departmentID}"
        response = requests.post(Test.url + 'Department/user/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callDELETE_Department_user_departmentIDNoAuthNoBody(self, check):
        "Testing authorization of DELETE /v1/Department/user/{departmentID}"
        response = requests.delete(Test.url + 'Department/user/0');
        check.equal(response.status_code, 200)
        check.is_false(response.json()['success'])
        check.equal(response.json()['errorKey'], "NotFound")
    
    @test()
    def callPOST_Department_resource_departmentIDNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Department/resource/{departmentID}"
        response = requests.post(Test.url + 'Department/resource/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callDELETE_Department_resourceNoAuthNoBody(self, check):
        "Testing authorization of DELETE /v1/Department/resource"
        response = requests.delete(Test.url + 'Department/resource');
        check.equal(response.status_code, 200)
        check.is_false(response.json()['success'])
        check.equal(response.json()['errorKey'], "NotFound")
    
    @test()
    def callGET_ErrorNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Error"
        response = requests.get(Test.url + 'Error').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPOST_ErrorNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Error"
        response = requests.post(Test.url + 'Error').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_PictogramNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Pictogram"
        response = requests.get(Test.url + 'Pictogram').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPOST_PictogramNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Pictogram"
        response = requests.post(Test.url + 'Pictogram').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_Pictogram_idNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Pictogram/{id}"
        response = requests.get(Test.url + 'Pictogram/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPUT_Pictogram_idNoAuthNoBody(self, check):
        "Testing authorization of PUT /v1/Pictogram/{id}"
        response = requests.put(Test.url + 'Pictogram/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")

    @test()
    def callDELETE_Pictogram_idNoAuthNoBody(self, check):
        "Testing authorization of DELETE /v1/Pictogram/{id}"
        response = requests.delete(Test.url + 'Pictogram/0');
        check.is_false(response.json()['success'])
        check.equal(response.json()['errorKey'], "NotFound")
    
    @test()
    def callGET_Pictogram_id_imageNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Pictogram/{id}/image"
        response = requests.get(Test.url + 'Pictogram/0/image').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPUT_Pictogram_id_imageNoAuthNoBody(self, check):
        "Testing authorization of PUT /v1/Pictogram/{id}/image"
        response = requests.put(Test.url + 'Pictogram/0/image').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_Pictogram_id_image_rawNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Pictogram/{id}/image/raw"
        response = requests.get(Test.url + 'Pictogram/0/image/raw').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_RoleNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Role"
        response = requests.get(Test.url + 'Role').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPOST_Role_guardian_usernameNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Role/guardian/{username}"
        response = requests.post(Test.url + 'Role/guardian/graatand').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callDELETE_Role_guardian_usernameNoAuthNoBody(self, check):
        "Testing authorization of DELETE /v1/Role/guardian/{username}"
        response = requests.delete(Test.url + 'Role/guardian/graatand');
        check.equal(response.status_code, 200)
        check.is_false(response.json()['success'])
        check.equal(response.json()['errorKey'], "NotFound")

    @test()
    def callPOST_Role_admin_usernameNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/Role/admin/{username}"
        response = requests.post(Test.url + 'Role/admin/graatand').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callDELETE_Role_admin_usernameNoAuthNoBody(self, check):
        "Testing authorization of DELETE /v1/Role/admin/{username}"
        response = requests.delete(Test.url + 'Role/admin/graatand');
        check.equal(response.status_code, 200)
        check.is_false(response.json()['success'])
        check.equal(response.json()['errorKey'], "NotFound")
    
    @test()
    def callGET_StatusNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Status"
        response = requests.get(Test.url + '/Status').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_Status_databaseNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/Status/database"
        response = requests.get(Test.url + '/Status/database').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_User_usernameNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/User/username"
        response = requests.get(Test.url + 'User/username').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_User_usernameNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/User/{username}"
        response = requests.get(Test.url + 'User/{username}').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_UserNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/User"
        response = requests.get(Test.url + 'User').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPUT_UserNoAuthNoBody(self, check):
        "Testing authorization of PUT /v1/User"
        response = requests.put(Test.url + 'User').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPUT_User_idNoAuthNoBody(self, check):
        "Testing authorization of PUT /v1/User/{id}"
        response = requests.put(Test.url + 'User/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_User_id_iconNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/User/{id}/icon"
        response = requests.get(Test.url + 'User/0/icon').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_User_id_icon_rawNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/User/{id}/icon/raw"
        response = requests.get(Test.url + 'User/0/icon/raw').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPUT_User_iconNoAuthNoBody(self, check):
        "Testing authorization of PUT /v1/User/icon"
        response = requests.put(Test.url + 'User/icon').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callDELETE_User_iconNoAuthNoBody(self, check):
        "Testing authorization of DELETE /v1/User/icon"
        response = requests.delete(Test.url + 'User/icon');
        check.equal(response.status_code, 200)
        check.is_false(response.json()['success'])
        check.equal(response.json()['errorKey'], "NotFound")
    
    @test()
    def callPOST_User_applications_usernameNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/User/applications/{username}"
        response = requests.post(Test.url + 'User/applications/graatand').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callDELETE_User_applications_usernameNoAuthNoBody(self, check):
        "Testing authorization of DELETE /v1/User/applications/{username}"
        response = requests.delete(Test.url + 'User/applications/graatand');
        check.equal(response.status_code, 200)
        check.is_false(response.json()['success'])
        check.equal(response.json()['errorKey'], "NotFound")
    
    @test()
    def callPUT_User_displaynameNoAuthNoBody(self, check):
        "Testing authorization of PUT /v1/User/display-name"
        response = requests.put(Test.url + 'User/display-name').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPOST_User_resource_usernameNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/User/resource/{username}"
        response = requests.post(Test.url + 'User/resource/graatand').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callDELETE_User_resourceNoAuthNoBody(self, check):
        "Testing authorization of DELETE /v1/User/resource"
        response = requests.delete(Test.url + 'User/resource');
        check.equal(response.status_code, 200)
        check.is_false(response.json()['success'])
        check.equal(response.json()['errorKey'], "NotFound")
    
    @test()
    def callPOST_User_grayscale_enabledNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/User/grayscale/{enabled}"
        response = requests.post(Test.url + 'User/grayscale/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPOST_User_launcher_animations_enabledNoAuthNoBody(self, check):
        "Testing authorization of POST /v1/User/launcher_animations/{enabled}"
        response = requests.post(Test.url + 'User/launcher_animations/0').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_User_getCitizens_usernameNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/User/getCitizens/{username}"
        response = requests.get(Test.url + 'User/getCitizens/graatand').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_User_getGuardians_usernameNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/User/getGuardians/{username}"
        response = requests.get(Test.url + 'User/getGuardians/graatand').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callGET_User_settingsNoAuthNoBody(self, check):
        "Testing authorization of GET /v1/User/settings"
        response = requests.get(Test.url + 'User/settings').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")
    
    @test()
    def callPUT_User_settingsNoAuthNoBody(self, check):
        "Testing authorization of PUT /v1/User/settings"
        response = requests.put(Test.url + 'User/settings').json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")

    @test()
    def ExpiredAuthorization(self, check):
        "Testing expired authorization of GET /v1/User"
        ### Same as above, but will use an outdated token
        # Will expire 5. April 2018 @ 11:23 UTC (01:23 Copenhagen): 
        response = requests.get(Test.url + 'User', headers = {"Authorization":"Bearer {0}".format(Authorization.expiredToken)}).json();
        check.is_false(response['success'])
        check.equal(response['errorKey'], "NotFound")