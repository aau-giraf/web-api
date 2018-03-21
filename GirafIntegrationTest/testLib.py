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

def request(requestType, url, data=''):
	return call('curl',  '-sX', requestType, 'http://localhost:5000/v1/'+url,'-H', 'accept: text/plain','-H', 'Content-Type: application/json-patch+json' ,'-d',data)

def newTest():
	assert(request('POST', 'account/logout')['success'])

def isLoggedIn():
	return request('GET', 'user/username')['success']
