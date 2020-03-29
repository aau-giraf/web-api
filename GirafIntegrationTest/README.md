# Integration tests

The section on running the tests has been bumped to the top for your convenience. Read on for tips and insights on maintenance and trouble-shooting of the tests.

See also [the wiki page](https://aau-giraf.github.io/wiki/development/rest_api_development/IntegrationTest/)

## File structure
`test.py` is the main executable.

`tests/` contains the test classes, one class for each controller in the backend.

`testlib.py` contains frequently used functions and extensions to the `unittest` module.

## Running the tests
#### API
Run the web API locally, confer the setup instructions in the `README.md` in the root folder. This must be running in order for the tests to work.

#### Python
Ensure you have installed at least Python 3.8 and that it is your current version. To check, run
```
python --version
```
If you don't have Python installed, see [installing Python](#installing-python)

#### Virtual environment and dependencies
It is common practice to setup a virtual environment and install dependencies there, rather than in your global installation.
```
# navigate to the integration tests dir
cd /path/to/web-api/GirafIntegrationTest

# install virtual environment
python3.8 -m venv venv

# source the virtual environment
source venv/bin/activate  # Unix/MacOS
venv\Scripts\activate.bat  # Windows

# install dependencies
pip install -r requirements.txt
```

#### Run
All tests are run by executing `test.py`.

```
python test.py
```

### Installing Python
You'll want to install your Python IDE of choice. Note that many IDEs are able to handle packages and python installation automatically, so start by installing the IDE itself to see if you need to proceed.

#### Windows
Download and run installer from [https://www.python.org/downloads/]. Follow the instructions, being sure to add Python3 to your `PATH`.

If pip was not properly installed along with python, see [this video](https://www.youtube.com/watch?v=mFqdeX1C-8M).

#### Mac
Download and run installer from [https://www.python.org/downloads/]. Follow the instructions.

If pip was not properly installed along with python, see [this video](https://www.youtube.com/watch?v=j3yH6FfD_Wk).

#### Linux
Install via your package manager, e.g. for Ubuntu run `sudo apt install python3.8`, then do `sudo apt install python3-pip`.


## Purpose
The tests within this folder are concerned with how all components of the backend work together when integrated. Calls are made to the API endpoints against a (locally) running backend server, and the responses received are checked.

This allows testing for things like access rights and correct storage and subsequent retrieval from the database. The flipside is that some tests may depend on others(logging in before checking access rights) and that the reason for the failure of a particular test may lie in multiple different components.

## Testing framework
Full documentation can be seen [here](https://docs.python.org/3/library/unittest.html). 

### Naming
Generally all names should both be in `lower_snake_case`. Exceptions are global constants in `UPPER_SNAKE_CASE` and class names in `UpperCamelCase`.

**All** test methods should be prefixed by `test_` (a method is **only** recognized as a test if it has this prefix) and `domain_`, and decorated by the `order` function, e.g. 

```py
@order
def test_account_login_or_something(self):
    # do your tests
```

assuming the test is in the `test_account_controller` file. You should also include what your test is doing and which endpoint it is testing in the doc string, e.g.

```py
@order
def test_account_login_or_something(self):
    """
    Testing <what your method is testing in active tense>  # e.g. "Testing logging in as <user>"
    
    Endpoint: <method>:<endpoint>  # e.g. POST:/v1/Account/login
    """
    # do your tests
```
The first line of the doc string will be printed to terminal when the test is run, everything else is documentation.

### @order
By default `unittest` runs the test in alphabetical order. To ensure tests are run in the order in which they are written in the code, the default sorter is set to `compare` from `testlib.py`.
To enforce this, the `@order` decorator **must be appended to all test methods**.

### File/Class setup
Should you create a new file, you should make sure that it/they follow the same format as the remaining classes, i.e. follow the same naming scheme, `test_domain_....py` for file name and `TestDomain` for the class. 

The class should extend the `GIRAFTestCase` class from `testlib.py`.
The class should also override the same methods: `setUpClass`, `tearDownClass`, `setUp`, `tearDown`.

For example, a file setup could look like so

```py
# imports

# global vars


class TestExampleController(GIRAFTestCase):
    """
    Testing API requests on Example endpoints
    """

    @classmethod
    def setUpClass(cls) -> None:
        """
        Setup necessary data when class is loaded
        """
        super(TestExampleController, cls).setUpClass()
        print(f'file:/{__file__}\n')

    @classmethod
    def tearDownClass(cls) -> None:
        """
        Remove or resolve necessary data and states after class tests are done
        """
        super(TestWeekTemplateController, cls).tearDownClass()

    def setUp(self) -> None:
        """
        Setup necessary data and states before each test
        """
        Pass

    def tearDown(self) -> None:
        """
        Remove or resolve necessary data and states after each test
        """
        pass
    
    # test methods
```

#### Global variables
Any variable that is set in an arbitrary test method `T` and should be accessible in all the remaining with the value set in `T` should be defined globally.

To modify a global variable, source it first, then change it.
```py
some_var = 0

class TestExampleController(GIRAFTestCase):
    # setup
    
    @order
    def test_method(self):
        global some_var
        some_var = 10
```

#### setUpClass
This method should include a call to the super method and print the current file name. This is purely for textual information in the terminal when running the tests.

Any consts should be defined here, e.g. static test data.

#### tearDownClass
This method should include a call to the super method. This is purely for textual information in the terminal when running the tests.

If any data or states need to be resolved when the class tests are finished running, this should be done here, e.g. closing of files, etc.

#### setUp
Any data that should be available in each test with the same start state should be defined here.

E.g., if you define `self.some_var = 5` in this method, `some_var` will be available in each test with the value `5`, despite changing it in some arbitrary test method.

#### tearDown
If any data or states need to be resolved when each test is finished running, this should be done here, e.g. closing of files, etc.

### Assertions
All classes extend `GIRAFTestCase`, which extends `TestCase`. In the latter are assertion methods that can be called through `self`, e.g. `self.assertEqual` and `self.assertTrue`.

### testlib.py
This file contains commonly used functions and extensions to the base `unittest` classes. If you want to add more functions that are used often, or make more extensions to base classes, do it here.

### Output
Output wise each class will be separate, e.g.

```
(venv) [usr@host GirafIntegrationTest]$ python test.py
Running integration tests for the GIRAF web API
----------------------------------------------------------------------

Testing API requests on Example endpoints
file://path/to/web-api/GirafIntegrationTest/tests/test_example_controller.py

Testing logging in as User ...                                                                                  OK
Testing getting User's id ...                                                                                   OK
Testing registering New User ...                                                                                OK

----------------------------------------------------------------------

Testing API requests on Other Example endpoints
file://path/to/web-api/GirafIntegrationTest/tests/test_other_example_controller.py

...

----------------------------------------------------------------------

Ran 148 tests in 4.231s

148 passed (0 unexpected), 0 errors, 0 failed (0 expected), 0 skipped

OK
```

### Errors and failures
On errors and failures, a stacktrace will be printed to the terminal, including any local variables and their values for debugging purposes.

The tests will continue running despite an error or failure.

## Contributors

SW613f18 & SW607f20

## License

Copyright [2018] [Aalborg University]
