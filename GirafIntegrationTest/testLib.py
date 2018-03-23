import subprocess
import json
import datetime
import getpass

class controllerTest:
	name = "unkonwn controller"
	currentTest = "unknown test"
	
	def __init__(self, name):
		self.name = name
	
	def call(self, *comm):
		process = subprocess.Popen(comm, stdout=subprocess.PIPE)
		output, error = process.communicate()
		return json.loads(output)

	def login(self):
		assert(self.request('POST', 'account/login', '{"username": "Graatand", "password": "password"}')['success'])

	def request(self, requestType, url, data='', auth='nokey'):
		return self.call('curl',  '-sX', requestType, 'http://localhost:5000/v1/'+url,'-H', 'accept: text/plain', '-H', "Authorization: bearer " + auth,'-H', 'Content-Type: application/json-patch+json' ,'-d',data)

	def newTest(self, title):
		currentTest = title
		self.ensure(self.request('POST', 'account/logout')['success'])

	def isValidAuth(self, auth):
		return self.request('GET', 'user/username', auth=auth)['success']
		
	def ensure(self, fact, errormessage=""):
		if fact:
			return;
		else:
			print("ENSURE failed in " + name + " for test `" + currentTest + "`\n " + errorMessage + "\n======================\n\n")
