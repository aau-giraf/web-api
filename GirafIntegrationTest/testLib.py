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
    check.is_not_none(response, message='Invalid response. Likely a 404 or a stacktrace.')

def ensureSuccess(response, check):
    check.is_true(response['success'])

def ensureError(response, check):
    check.is_false(response['success'])


def ensureErrorWithKey(response, check, errorKey):
    check.is_false(response['success'])
    check.equal(response['errorKey'], errorKey)


def ensureNotAuthorized(response, check):
    ensureErrorWithKey(response, 'NotFound')

def ensureNoData(response, check):
    check.is_none(response['data'])

def ensureSomeData(response, check):
    check.is_not_none(response['data'])

def login(username, check, password='password', fail=False):
    response = requests.post(Test.url + 'account/login', json={"username": username, "password": password}).json()
    if fail:
        check.is_false(response['success'])
        check.is_none(response['data'])
    else:
        ensureSuccess(response, check)
    return None if fail else response['data']

def auth(token):
    return {"Authorization": "Bearer {0}".format(token)}