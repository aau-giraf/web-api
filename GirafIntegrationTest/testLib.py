# -*- coding: utf-8 -*-
import http
import inspect
import subprocess
import json
import datetime
import getpass
import requests
import logging

class Test:
    # This url should absolutely not point to anything persistent.
    # The database may well get wrecked by the tests.
    url = "http://127.0.0.1:5000/v1/"

def ensureValidResponse(response, check):
    return check.is_not_none(response, message='Invalid response. Likely a 404 or a stacktrace.')


def ensureSuccess(response, check):
    if not ensureValidResponse(response, check):
        return False

    errormessages = ''

    for message in response['errorProperties']:
        errormessages += '\nMessage:  ' + message

    return check.is_true(response['success'],
                         message='Request failed: {0}'.format(response['errorKey'] + errormessages))


def ensureError(response, check):
    if not ensureValidResponse(response, check):
        return False
    return check.is_false(response['success'],
                          message='Server responds success on illegal action')


def ensureErrorWithKey(response, check, errorKey):
    if not ensureValidResponse(response, check):
        return False
    return check.is_true(response['success'] is False and response['errorKey'] == errorKey,
                         message='Server did not respond correct errorKey.\n'
                                 'Expected: {0}\n'
                                 'Actual:   {1}\n'.format(errorKey, response[errorKey]))


def ensureNotAuthorized(response, check):
    if not ensureValidResponse(response, check):
        return False
    return check.is_true(response['success'] is False and response['errorKey'] == 'NotFound',
                         message='Server did not respond with NotFound. Got "' + response['errorKey'] + '"\n')


def ensureNoData(response, check):
    if not ensureValidResponse(response, check):
        return False
    return check.is_true('data' not in response or response['data'] is None,
                         message='Data was returned when it should not have been.\n')


def ensureSomeData(response, check):
    if not ensureValidResponse(response, check):
        return False
    return check.is_true('data' in response and response['data'] is not None,
                         message='Data expected, but none returned\n')


def login(username, check, password='password'):
    response = requests.post(Test.url + 'account/login', json={"username": username, "password": password}).json()
    ensureSuccess(response, check)
    return response['data']

def auth(token):
    return {"Authorization": "Bearer {0}".format(token)}