CREATE DATABASE giraf_old;
USE giraf_old;

SOURCE girafdump.sql

USE giraf;

INSERT INTO Departments SELECT * FROM giraf_old.Departments WHERE Name!="Test-Department";

INSERT INTO 
    giraf.LauncherOptions 
    SELECT 
        giraf_old.LauncherOptions.Key, 
        NULL,
        appGridSizeColumns, 
        appGridSizeRows, 
        1,
        2, 
        DisplayLauncherAnimations, 
        1, 
        1,
        NULL
    FROM 
        giraf_old.LauncherOptions
;

INSERT INTO AspNetUsers SELECT * FROM giraf_old.AspNetUsers WHERE DepartmentKey != 31;

INSERT INTO Pictograms(id, Discriminator, LastEdit, AccessLevel, Image, Title, Sound) SELECT * FROM giraf_old.Frames WHERE Discriminator = "Pictogram";

INSERT INTO Weeks SELECT id, Discriminator, GirafUserId, Name, ThumbnailKey, DepartmentKey FROM giraf_old.Weeks;
INSERT INTO Weekdays SELECT * FROM giraf_old.Weekdays;

DROP DATABASE giraf_old;