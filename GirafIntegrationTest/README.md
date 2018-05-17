# Integration tests

The section on running the tests has been bumped to the top for your convenience. Read on for tips and insights on maintenance and trouble-shooting of the tests.

See also [The wiki page](http://web.giraf.cs.aau.dk/w/rest_api_-_development/integration_test/)

## File structure
`⋯/test.py` is the main executable.

`⋯/tests` contains the tests themselves, with one file/class for each controller in the backend.

`⋯/testLib.py` contains methods for frequently used actions such as logging in and ensureing the success of a request.

## Running the tests
The main project must be running in order for the integration tests to have a server to communicate with. See `README.md` in root folder for that.

All tests are run by executing `test.py` which is located in the same folder as this readme.
From a shell, navigate to this folder, then do `python test.py`. This requires _Python 3.6 or later_ to be installed. See section on installation below.

### Installing python
You'll want to install your Python IDE of choice. Note that many IDEs are able to handle packages and python installation automatically, so start by installing the IDE itself to see if you need to proceed.

#### Windows
Download and run installer from [https://www.python.org/downloads/]. Follow the instructions, being sure to add Python3 to your `PATH`.

If pip was not properly installed along with python, see [this video](https://www.youtube.com/watch?v=mFqdeX1C-8M).

#### Mac
Download and run installer from [https://www.python.org/downloads/]. Follow the instructions.

If pip was not properly installed along with python, see [this video](https://www.youtube.com/watch?v=j3yH6FfD_Wk).

#### Linux
From a shell, do `sudo apt install python3.6`. Then do `sudo apt install python3-pip`.

###Required packages
Similar on all systems. From a shell, do `python`. From the python prompt do `pip install integrate` then do `pip install requests`.


## Purpose
The tests within this folder are concerned with how all components of the backend work together when integrated. Calls are made to the API endpoints against a (locally) running backend server, and the responses received are checked.

This allows testing for things like access rights and correct storage and subsequent retrieval from the database. The flipside is that some tests may depend on others(logging in before checking access rights) and that the reason for the failure of a particular test may lie in multiple different components.

## Testing framework
Full documentation such as it is can be found at [@test(skip_if_failed=['loginAsKurt'])]. 

From `userControllerTest.py`, consider
        class UserControllerTest(TestCase):
        'User Controller'
        
          kurt = None
          @test()
          def loginAsKurt(self, check):
              'Log in as Kurt'
              self.kurt = login('Kurt', check)

          kurtId = None
          @test(skip_if_failed=['loginAsKurt'])
          def GetKurtID(self, check):
              'Get User info for kurt'
              response = requests.get(Test.url + 'User', headers=auth(self.kurt)).json()
              ensureSuccess(response, check)
              check.equal(response['data']['username'], 'Kurt')
              self.kurtId = response['data']['id']

This should result in

        * Running test suite 'User Controller'
           - Running Log in as Kurt                               : [  OK  ]
           - Running Get User info for kurt                       : [  OK  ]

The test names printed when run come from the docstrings below each method definition. E.g. `'Log in as Kurt'`.

The tests are run in undefined order, often on multiple threads. To enforce sequence, tests are marked as either `@test(skip_if_failed=['loginAsKurt'])` or `@test(depends=['loginAsKurt'])`. The `depends` property will make sure that all dependencies are run before the test in question. The `skip_if_failed` does the same thing, and in addition to this, will not run the dependencies if they did not pass.

Assertions can either be made directly from the given `check` object, or from common `ensureSomething(response, check)` style functions in `testLib.py`.

### Explanation of common but mystifying errors
`false is false` is caused by a failed `check.is_true`. We assume the first `false` was originally supposed to be the expression checked, but becomes its value due to an error in the framework.

`JSON decode error` is caused when the `.json()` function which must come after each request is given something that is not valid JSON. This indicates that the response was a `Not Found`, an `Internal Server Error` or anything else which is not a giraf `Response<T>` object.

`index exception` or `key exception` is thrown when trying to access some field in the `response` which does not exist.

## Contributors

SW613f18

## License

Copyright [2018] [Aalborg University]
