Department Controller
=====================

##*BUG:* Seems any user can move anyone to a new department...
Both Graatand and Kurt were allowed to moove Gunnar to a new department, although either have any relation to him.

##*BUG?:* Seems any user is able to add/remove pictograms to any department
Kurt was able to add the newly created Cyclopian pictogram from the newly created Dalgaardsholmstuen.
He had no relation to either.
What is worse is he was able to remove it again afterwards.


Pictogram Controller
====================

##*BUG:* People can easily GET other peoples' private pictograms if they correctly guess the ID
Obviously this should not be the case.

##*BUG:* For some reason public pictograms' images are not publicly accessible. But the raw version is.
NotAuthorized for `GET .../Pictogram/2/image` but not for `GET .../Pictogram/2/image/raw`
I should have access to both.

##*BUG:* Department ID persists in userinfo after user is removed
When a user is removed from a department, this is not visible in `GET .../user`

##*BUG:* Seems anyone is able to get anyone's User Ifo
This won't do, as Asger stated on Slack that one time.

Role Controller
===============

##*BUG:* Userinfo not updated when made citizen
If a new user is created, made *guardian* and then made *citizen* again, in the endpoint

`GET .../user`

It looks as if the user is still *guardian*.

##*BUG:* Userinfo not updated when made admin
Related to previous bug; if you make a user admin, no error is returned, but the update is not reflected in  `GET .../user`

User Controller
===============

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
