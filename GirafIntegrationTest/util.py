from tests import *

TEST_CLASSES = [TestAccountController, TestAuthorization, TestDepartmentController,
                TestPictogramController, TestUserController, TestWeekController,
                TestWeekTemplateController]
ENDPOINTS = {'Account': {'/v1/Account/login': ['POST'], '/v1/Account/register': ['POST'],
                         '/v1/Account/user/{id}': ['DELETE'],
                         '/v1/User/{id}/Account/password': ['PUT', 'POST'],
                         '/v1/User/{id}/Account/password-reset-token': ['GET']},
             'Activity': {'/v2/Activity/{user_id}/{weekplan_name}/{week_year}/{week_number}'
                          '/{week_day_number}': ['POST'],
                          '/v2/Activity/{user_id}/delete/{activity_id}': ['DELETE'],
                          '/v2/Activity/{user_id}/update': ['PATCH']},
             'Department': {'/v1/Department': ['GET', 'POST'],
                            '/v1/Department/{id}': ['GET', 'DELETE'],
                            '/v1/Department/{id}/citizens': ['GET'],
                            '/v1/Department/{departmentId}/user/{userId}': ['POST'],
                            '/v1/Department/{id}/name': ['PUT']},
             'Error': {'/v1/Error': ['GET', 'PUT', 'POST', 'DELETE']},
             'Pictogram': {'/v1/Pictogram': ['GET', 'POST'],
                           '/v1/Pictogram/{id}': ['GET', 'PUT', 'DELETE'],
                           '/v1/Pictogram/{id}/image': ['GET', 'PUT'],
                           '/v1/Pictogram/{id}/image/raw': ['GET']},
             'Status': {'/v1/Status': ['GET'], '/v1/Status/database': ['GET'],
                        '/v1/Status/version-info': ['GET']},
             'User': {'/v1/User': ['GET'], '/v1/User/{id}': ['GET', 'PUT'],
                      '/v1/User/{id}/settings': ['GET', 'PUT'],
                      '/v1/User/{id}/icon': ['GET', 'PUT', 'DELETE'],
                      '/v1/User/{id}/icon/raw': ['GET'],
                      '/v1/User/{userId}/citizens': ['GET'],
                      '/v1/User/{userId}/guardians': ['GET'],
                      '/v1/User/{userId}/citizens/{citizenId}': ['POST']},
             'Week': {'/v2/User/{userId}/week': ['GET'], '/v1/User/{userId}/week': ['GET'],
                      '/v1/User/{userId}/week/{weekYear}/{weekNumber}': ['GET', 'PUT', 'DELETE']},
             'WeekTemplate': {'/v1/WeekTemplate': ['GET', 'POST'],
                              '/v1/WeekTemplate/{id}': ['GET', 'PUT', 'DELETE']}}


def missing_endpoints():
    """
    Prints missing endpoints based on globally defined dict 'ENDPOINTS'
    """
    missing = _missing()
    print('#' * 50)
    print(f'\t\tMissing tests')
    for k, v in missing.items():
        print()
        print(f'{k} endpoints')
        print('-' * 50)
        for end, meths in v.items():
            if meths:
                print(f'Methods {meths} in endpoint {end} are NOT tested')


def tested_endpoints():
    """
    Prints tested endpoints based on globally defined dict 'ENDPOINTS'
    """
    tested = _tested()
    print(f'\t\tAlready tested')
    for k, v in tested.items():
        print()
        print(f'{k} endpoints')
        print('-' * 50)
        for end, meths in v.items():
            print(f'Methods {meths} in endpoint {end} are tested')


def _missing() -> dict:
    """
    Collects missing endpoints
    :return: missing endpoints as dict
    """
    missing = {'Authorization': {}}
    tested = _tested()
    for k, v in ENDPOINTS.items():
        missing[k] = {}

        # if no endpoints in this controller is tested, simply copy all endpoints
        if k not in tested.keys():
            missing[k] = v.copy()

        for end, meths in v.items():
            if end not in tested['Authorization'].keys():
                # if endpoint is not tested, simply copy it with all methods
                missing['Authorization'][end] = meths.copy()
            else:
                # if it is, append all tested methods
                missing['Authorization'][end] = []
                for meth in meths:
                    try:
                        if meth not in missing['Authorization'][end] and meth not in \
                                tested['Authorization'][end]:
                            missing['Authorization'][end].append(meth)
                    except KeyError:
                        missing['Authorization'][end].append(meth)

            # if any endpoint in this controller are tested
            if k in tested.keys():
                if end not in tested[k].keys():
                    # if this endpoint is not tested, simply copy it with all methods
                    missing[k][end] = meths.copy()
                else:
                    # if it is, append all tested methods
                    missing[k][end] = []
                    for meth in meths:
                        if meth not in missing[k][end] and meth not in tested[k][end]:
                            missing[k][end].append(meth)

    return missing


def _tested() -> dict:
    """
    Collects tested endpoints
    :return: tested endpoints as dict
    """
    tested = {'Authorization': {}}
    cls_meth = _methods()

    for k, v in ENDPOINTS.items():
        tested[k] = {}

        for end, meths in v.items():
            # if any endpoint in this controller has been tested
            if k in cls_meth.keys():
                # if this endpoint has been tested
                if end in cls_meth[k].keys():
                    tested[k][end] = []
                    for m in meths:
                        # if this method has been tested, append
                        if m in cls_meth[k][end]:
                            tested[k][end].append(m)

            # if any endpoint in this controller has been tested in test_authorization.py
            if end in cls_meth['Authorization'].keys():
                tested['Authorization'][end] = []
                for m in meths:
                    # if this method has been tested, append
                    if m in cls_meth['Authorization'][end]:
                        tested['Authorization'][end].append(m)
    return tested


def _methods() -> dict:
    """
    Extracts methods from test classes and puts them into a dict by their request method
    and endpoint
    :return: methods as dict
    """
    temp = {}
    for cls in TEST_CLASSES:
        # for each object in each test class, append as an object if
        # it is callable, i.e. a method, and
        # it is a test method, i.e. prefixed by 'test'
        temp[_slice(cls.__name__)] = \
            [getattr(cls(), meth) for meth in dir(cls()) if callable(getattr(cls(), meth))
             and meth[:4] == 'test']
    cls_meth = {}
    # convert to dict of endpoints and request methods
    for k, v in temp.items():
        cls_meth[k] = {}
        for func in v:
            # get request method and endpoint and add to dict
            meth, end = _endpoint(func.__doc__)
            try:
                cls_meth[k][end].append(meth)
            except KeyError:
                cls_meth[k][end] = [meth]
    return cls_meth


def _slice(name: str) -> str:
    """
    Extracts controller name
    :param name: test class name
    :return: controller name
    """
    endpoint_end = name.index('Controller') if 'Controller' in name else None
    return name[4:endpoint_end]


def _endpoint(doc: str) -> tuple:
    """
    Extracts request method and endpoint from method doc string
    :param doc: doc string
    :return: request method, endpoint
    """
    docs = [x.strip() for x in doc.strip().split('\n') if x]
    endpoint = [x.strip() for x in docs[1].split(':')]
    return endpoint[1], endpoint[2]
