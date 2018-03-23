import subprocess
import json
import datetime
import getpass

def call(*comm):
	process = subprocess.Popen(comm, stdout=subprocess.PIPE)
	output, error = process.communicate()
	return json.loads(output)
def login():
	assert(request('POST', 'account/login', '{"username": "Graatand", "password": "password"}')['success'])

def request(requestType, url, data='', auth='nokey'):
	return call('curl',  '-sX', requestType, 'http://localhost:5000/v1/'+url,'-H', 'accept: text/plain', '-H', "Authorization: bearer " + auth,'-H', 'Content-Type: application/json-patch+json' ,'-d',data)

def newTest():
	assert(request('POST', 'account/logout')['success'])

def isValidAuth(auth):
	return request('GET', 'user/username', auth=auth)['success']