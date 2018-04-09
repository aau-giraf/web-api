# -*- coding: utf-8 -*-
from testLib import *
import time


def testAuthorization():
    test = controllerTest("Authorization Test")
    
    ###
    test.newTest('Testing authorization of POST /v1/Account/set-password');
    response = test.request('POST',  'Account/set-password');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/Account/change-password');
    response = test.request('POST',  'Account/change-password');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Account/reset-password');
    response = test.request('GET',  'Account/reset-password');
    test.ensure(response is None) # Aka. responded with HTML
    
    ###
    test.newTest('Testing authorization of GET /v1/Account/reset-password-confirmation');
    response = test.request('GET',  'Account/reset-password-confirmation');
    test.ensure(response is None) 
    # Aka. reponse is not valid JSON Aka. HTML
    
    ###
    test.newTest('Testing authorization of GET /v1/Account/access-denied');
    response = test.request('GET',  'Account/access-denied');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Choice/{id}');
    response = test.request('GET',  'Choice/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of PUT /v1/Choice/{id}');
    response = test.request('PUT',  'Choice/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of DELETE /v1/Choice/{id}');
    response = test.request('DELETE',  'Choice/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/Choice');
    response = test.request('POST',  'Choice');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of PUT /v1/Day/{id}');
    response = test.request('PUT',  'Day/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Department');
    response = test.request('GET',  'Department');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/Department');
    response = test.request('POST',  'Department');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Department/{id}/citizens');
    response = test.request('GET',  'Department/0/citizens');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/Department/user/{departmentID}');
    response = test.request('POST',  'Department/user/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of DELETE /v1/Department/user/{departmentID}');
    response = test.request('DELETE',  'Department/user/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/Department/resource/{departmentID}');
    response = test.request('POST',  'Department/resource/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of DELETE /v1/Department/resource');
    response = test.request('DELETE',  'Department/resource');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Error');
    response = test.request('GET',  'Error');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/Error');
    response = test.request('POST',  'Error');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Pictogram');
    response = test.request('GET',  'Pictogram');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/Pictogram');
    response = test.request('POST',  'Pictogram');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Pictogram/{id}');
    response = test.request('GET',  'Pictogram/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of PUT /v1/Pictogram/{id}');
    response = test.request('PUT',  'Pictogram/0');
    test.ensureNotAuthorized(response)

    ###
    test.newTest('Testing authorization of DELETE /v1/Pictogram/{id}');
    response = test.request('DELETE',  'Pictogram/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Pictogram/{id}/image');
    response = test.request('GET',  'Pictogram/0/image');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of PUT /v1/Pictogram/{id}/image');
    response = test.request('PUT',  'Pictogram/0/image');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Pictogram/{id}/image/raw');
    response = test.request('GET',  'Pictogram/0/image/raw');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Role');
    response = test.request('GET',  'Role');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/Role/guardian/{username}');
    response = test.request('POST',  'Role/guardian/graatand');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of DELETE /v1/Role/guardian/{username}');
    response = test.request('DELETE',  'Role/guardian/graatand');
    test.ensureNotAuthorized(response)

    ###
    test.newTest('Testing authorization of POST /v1/Role/admin/{username}');
    response = test.request('POST',  'Role/admin/graatand');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of DELETE /v1/Role/admin/{username}');
    response = test.request('DELETE',  'Role/admin/graatand');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Status');
    response = test.request('GET',  '/Status');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/Status/database');
    response = test.request('GET',  '/Status/database');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/User/username');
    response = test.request('GET',  'User/username');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/User/{username}');
    response = test.request('GET',  'User/{username}');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/User');
    response = test.request('GET',  'User');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of PUT /v1/User');
    response = test.request('PUT',  'User');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of PUT /v1/User/{id}');
    response = test.request('PUT',  'User/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/User/{id}/icon');
    response = test.request('GET',  'User/0/icon');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/User/{id}/icon/raw');
    response = test.request('GET',  'User/0/icon/raw');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of PUT /v1/User/icon');
    response = test.request('PUT',  'User/icon');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of DELETE /v1/User/icon');
    response = test.request('DELETE',  'User/icon');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/User/applications/{username}');
    response = test.request('POST',  'User/applications/graatand');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of DELETE /v1/User/applications/{username}');
    response = test.request('DELETE',  'User/applications/graatand');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of PUT /v1/User/display-name');
    response = test.request('PUT',  'User/display-name');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/User/resource/{username}');
    response = test.request('POST',  'User/resource/graatand');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of DELETE /v1/User/resource');
    response = test.request('DELETE',  'User/resource');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/User/grayscale/{enabled}');
    response = test.request('POST',  'User/grayscale/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of POST /v1/User/launcher_animations/{enabled}');
    response = test.request('POST',  'User/launcher_animations/0');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/User/getCitizens/{username}');
    response = test.request('GET',  'User/getCitizens/graatand');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/User/getGuardians/{username}');
    response = test.request('GET',  'User/getGuardians/graatand');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of GET /v1/User/settings');
    response = test.request('GET',  'User/settings');
    test.ensureNotAuthorized(response)
    
    ###
    test.newTest('Testing authorization of PUT /v1/User/settings');
    response = test.request('PUT',  'User/settings');
    test.ensureNotAuthorized(response)


def testExpiredAuthorization():
    ### Same as above, but will use an outdated token
    # Will expire 5. April 2018 @ 11:23 UTC (01:23 Copenhagen): eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1YmM0YzAzMC1mOGQxLTRhYTAtOTBlOC05MTNhMDYwOTA4YWIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ijg0MTJkOTk1LWIzODEtNGY4My1iZDI1LWU5ODY2NzBiNTdkOSIsImV4cCI6MTUyNTMwMzQyNSwiaXNzIjoibm90bWUiLCJhdWQiOiJub3RtZSJ9.8KXRRqF3B5s8tUki7u5j0TqK-189QIpApdOC6aSxOms

    test = controllerTest("Expired authorization Test")
    
    test.newTest('Testing expired authorization of GET /v1/User');
    response = test.request('GET',  'User', auth='eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1YmM0YzAzMC1mOGQxLTRhYTAtOTBlOC05MTNhMDYwOTA4YWIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ijg0MTJkOTk1LWIzODEtNGY4My1iZDI1LWU5ODY2NzBiNTdkOSIsImV4cCI6MTUyNTMwMzQyNSwiaXNzIjoibm90bWUiLCJhdWQiOiJub3RtZSJ9.8KXRRqF3B5s8tUki7u5j0TqK-189QIpApdOC6aSxOms');
    test.ensureNotAuthorized(response)