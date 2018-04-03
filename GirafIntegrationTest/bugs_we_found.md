Choice Controller
=====================
##*BUG:* GET Choice/0 returns NotFound when no authorization is supplied, should be Unauthorized 
##*BUG:* PUT Choice/0 returns FormatError when no authorization is supplied, should be Unauthorized 
##*BUG:* DELETE Choice/0 returns NotFound when no authorization is supplied, should be Unauthorized 
##*BUG:* POST Choice returns FormatError when no authorization is supplied, should be Unauthorized 


Department Controller
=====================

##*BUG* DELETE department/resource returns 404 when no authorization is supplied
##*BUG?:* GET department returns a lot of information to anyone who calls it... Maybe limit if not authorized?
##*BUG:* POST /v1/Department/user/0 returns MissingProperties when no authorization is supplied, should be Unauthorized 
##*BUG:* DELETE /v1/Department/user/0 returns MissingProperties when no authorization is supplied, should be Unauthorized 
##*BUG:* DELETE /v1/Department/resource does not return valid JSON when no authorization is supplied, should be Unauthorized 


##*BUG:* Seems any user can move anyone to a new department...
Both Graatand and Kurt were allowed to moove Gunnar to a new department, although either have any relation to him.

##*BUG?:* Seems any user is able to add/remove pictograms to any department
Kurt was able to add the newly created Cyclopian pictogram from the newly created Dalgaardsholmstuen.
He had no relation to either.
What is worse is he was able to remove it again afterwards.


Pictogram Controller
====================

##*BUG:* GET Pictogram returns NoError when no authorization supplied. Should be UnAuthorized. 
##*BUG:* GET Pictogram/0 returns PictogramNotFound when no authorization supplied. Should be UnAuthorized. 
##*BUG:* DELETE Pictogram/0 returns UserNotFound when no authorization supplied. Should be UnAuthorized. 
##*BUG:* PUT Pictogram/0 does not return valid JSON when no authorization supplied. Should be UnAuthorized.
##*BUG:* PUT Pictogram/0/image does not return valid JSON when no authorization supplied. Should be UnAuthorized.

##*BUG:* People can easily GET other peoples' private pictograms if they correctly guess the ID
Obviously this should not be the case.
No authorization also returns a "PictogramNotFound", when should return "NotAuthorized"

##*BUG:* For some reason public pictograms' images are not publicly accessible. But the raw version is.
NotAuthorized for `GET .../Pictogram/2/image` but not for `GET .../Pictogram/2/image/raw`
I should have access to both.

##*BUG:* Department ID persists in userinfo after user is removed
When a user is removed from a department, this is not visible in `GET .../user`

##Min fejl: Hvordan håndterer jeg de rå billeder i integrationstests?
Jeg mangler en 3-5 tests her.

##*BUG:* Seems anyone is able to get anyone's User Ifo
This won't do, as Asger stated on Slack that one time.

Role Controller
===============

##*BUG:* DELETE Role/Admin/graatand does not return valid JSON when no authorization supplied. Should be UnAuthorized.

##*BUG:* Userinfo not updated when made citizen
If a new user is created, made *guardian* and then made *citizen* again, in the endpoint

`GET .../user`

It looks as if the user is still *guardian*.

##*BUG:* Userinfo not updated when made admin
Related to previous bug; if you make a user admin, no error is returned, but the update is not reflected in  `GET .../user`

User Controller
===============

##*BUG:* PUT User does not return valid JSON when no authorization supplied. Should be UnAuthorized.
##*BUG:* PUT User/0 does not return valid JSON when no authorization supplied. Should be UnAuthorized.
##*BUG:* PUT User/Icon does not return valid JSON when no authorization supplied. Should be UnAuthorized.
##*BUG:* DELETE User/Icon does not return valid JSON when no authorization supplied. Should be UnAuthorized.
##*BUG:* DELETE User/applications/graatand does not return valid JSON when no authorization supplied. Should be UnAuthorized.
##*BUG:* PUT User/display-name does not return valid JSON when no authorization supplied. Should be UnAuthorized.
##*BUG:* DELETE User/resource does not return valid JSON when no authorization supplied. Should be UnAuthorized.
##*BUG:* PUT User/settings does not return valid JSON when no authorization supplied. Should be UnAuthorized.

##*BUG:* Anyone is able to get anyone's user info
Gunnar can read both Kurt's and Graatand's. This is a right mess.

##*BUG:* Several problems with POST .../User
 - Able to set username(causing errors).
 - Somehow, whatever username is set to, screen name also becomes.
 - Able to change department(Surely you shouldn't be allowed to do that from there).
 - Gunnar(a citizen) is able to change Graatand's(an unrelated guardian) User Info.

##*BUG:* POSTing settings without sending data does not result in error
In fact, that sets grid sizes to 0. That's... stragne.

##*BUG? Help?:* Unable to access GET citizens/guardians endpoint
I get NotAuthorized, and I can't readily determine why. Doesn't happen from postman.

##*BUG?:* If no weeks exist for user, error is returned
Seems a bit excessive. An empty list should have been enough.

##*BUG:* `POST .../Week` endpoint is a pile of garbage
The code seems in part to rely on specially formatted data from the client.
It expects 7 days with correct enum value for the day's name, along with some kind of boolean flag set that I don't quite understand.
The client cannot be trusted; this is not acceptable.

Status Controller
===============

##*BUG:* Endpoints do not start with /v1/ like all other endpoints
##*RELATED BUG:* GET /v1/Status returns NotFound.
##*RELATED BUG:* GET /v1/Status/database returns NotFound.

Account Controller
===============

##*BUG:* get Account/reset-password with no data responds with bad request. Should respond with missingProperties


Day Controller
===============

##*BUG:* PUT Day/0 returns 404 when no authorization is supplied

##*BUG:* Checks if day is valid before authorization and returns 404 if day is not valid. Have not tested other values than 0, 1, and 2 though....

