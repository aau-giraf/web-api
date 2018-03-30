#*BUG:* Gunnar should be citizen again
Når man opretter en ny bruger, "ophæver" den til Værge, og så "degraderer" den til Borger igen, så fremgår degraderingen ikke i endpointet
GET user
Her ser det ud som om brugeren stadig er Værge.
```ENSURE failed in Role Controller for test `Gunnar should be admin`
Role was Citizen

  File:     /home/rgc/GitvAErk/API/GirafIntegrationTest/roleControllerTest.py
  Function: testRoleController
  Line:     77
======================```

#*BUG* Seems anyone can move anyone to a new department...
Både Graatand og Kurt havde lov til at flytte nyoprettede Gunnar til en ny afdeling
```ENSURE failed in Department Controller for test `Kurt tries to add Gunnar to Dalgaardsholmstuen`
Response says success

  File:     /home/rgc/GitvAErk/API/GirafIntegrationTest/departmentControllerTest.py
  Function: testDepartmentController
  Line:     104
======================

ENSURE failed in Department Controller for test `Graatand tries to add Gunnar to Dalgaardsholmstuen`
Response says success

  File:     /home/rgc/GitvAErk/API/GirafIntegrationTest/departmentControllerTest.py
  Function: testDepartmentController
  Line:     109
======================

ENSURE failed in Department Controller for test `Check that Gunnar has not changed departments yet.`
Gunnar was moved to new department!

  File:     /home/rgc/GitvAErk/API/GirafIntegrationTest/departmentControllerTest.py
  Function: testDepartmentController
  Line:     115
======================```

#*BUG:* Department ID persists in userinfo after user is removed
Når man fjerner en bruger fra en afdeling, så fremgår fjernelsen ikke af GET .../user
```ENSURE failed in Department Controller for test `Check that Gunnar was removed`
Gunnar was moved to new department!

  File:     /home/rgc/GitvAErk/API/GirafIntegrationTest/departmentControllerTest.py
  Function: testDepartmentController
  Line:     154
======================```