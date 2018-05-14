# Integration tests
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
## Contributors

SW613f18 & SW615f17

## License

Copyright [2018] [Aalborg University]
