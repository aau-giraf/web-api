#*BUG:* Gunnar should be citizen again
Når man opretter en ny bruger, "ophæver" den til Værge, og så "degraderer" den til Borger igen, så fremgår degraderingen ikke i endpointet
GET .../user
Her ser det ud som om brugeren stadig er Værge.
 - `Gunnar should be admin`

#*BUG* Seems anyone can move anyone to a new department...
Både Graatand og Kurt havde lov til at flytte nyoprettede Gunnar til en ny afdeling
 - `Kurt tries to add Gunnar to Dalgaardsholmstuen`
 - `Graatand tries to add Gunnar to Dalgaardsholmstuen`
 - `Check that Gunnar has not changed departments yet.`

#*BUG:* Department ID persists in userinfo after user is removed
Når man fjerner en bruger fra en afdeling, så fremgår fjernelsen ikke af GET .../user
 - `Check that Gunnar was removed`

#*BUG:* For some reason public pictograms' images are not publicly accessible. But the raw version is.
Jeg får NotAuthorized for GET .../Pictogram/2/image men ikke for GET .../Pictogram/2/image/raw
Jeg burde ikke få NotAuthorized for nogen af dem.
 - `Get image of public pictogram`

#Min fejl: Hvordan håndterer jeg de rå billeder i integrationstests?