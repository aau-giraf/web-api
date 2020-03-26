import unittest

BASE_URL = 'http://127.0.0.1:5000/'


def order_handler():
    ordered = {}

    def ordered_handler(f):
        ordered[f.__name__] = len(ordered)
        return f

    def compare_handler(a, b):
        return [1, -1][ordered[a] < ordered[b]]

    return ordered_handler, compare_handler


order, compare = order_handler()
unittest.defaultTestLoader.sortTestMethodsUsing = compare

