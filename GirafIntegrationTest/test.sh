#!/usr/bin/env bash

# pretty message
echo -e '\033[33m''Running integration tests for the GIRAF web API''\033[0m'
echo '----------------------------------------------------------------------'
echo ''

# source virtual environment
source venv/bin/activate

# run tests verbosely
python -m unittest discover tests -v

# deactivate virtual environment
deactivate