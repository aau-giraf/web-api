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

if(datetime.datetime.today().weekday() > 4):
	# Send en mail til anton om at stræberne kører integrationtests i weekenderne:
	message = getpass.getuser() + ' is a nerd and runs integration-tests on the weekends!'
	comm = ['curl', '-s', 'https://antonchristensen.net', '-c' ,'cookie']
	process = subprocess.Popen(comm, stdout=subprocess.PIPE)
	output, error = process.communicate()
	tokenIndex = output.find(br'<input name="anti_pihilili_token" type="hidden" value="')
	comm = ['curl', '-sX', 'POST', 'https://antonchristensen.net', '-b', 'cookie','-H', 'Cache-Control: no-cache','-H', 'content-type: multipart/form-data', '-F', 'name=spam', '-F', 'anti_pihilili_token=' + output[tokenIndex+55:tokenIndex+55+10].decode("utf-8"), '-F', 'email=spam', '-F', 'message='+message]
	process = subprocess.Popen(comm, stdout=subprocess.PIPE)
	print("lolnerd")