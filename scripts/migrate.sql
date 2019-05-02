CREATE DATABASE giraf_old;

Use giraf_old;

SOURCE girafdump.sql

USE giraf;

INSERT INTO Departments SELECT id, Name FROM giraf_old.Departments WHERE Name!="Test-Department";
INSERT INTO Departments SELECT id, Name FROM giraf_old.Departments WHERE id=31;

INSERT INTO
    giraf.Setting
    SELECT
        giraf_old.LauncherOptions.Key,
        NULL,
        2,
        1,
        2,
        2,
        0,
        7,
        1,
        1,
        900
    FROM
        giraf_old.LauncherOptions
;

INSERT INTO AspNetUsers SELECT * FROM giraf_old.AspNetUsers;
INSERT INTO AspNetRoles SELECT * FROM giraf_old.AspNetRoles;
INSERT INTO AspNetUserRoles SELECT * FROM giraf_old.AspNetUserRoles;
INSERT INTO Pictograms SELECT id, AccessLevel, ImageHash, LastEdit, Sound, Title FROM giraf_old.Frames WHERE Discriminator = "Pictogram";


INSERT INTO Weeks SELECT id, GirafUserId, Name, ThumbnailKey, 1, 1864 FROM giraf_old.Weeks;
INSERT INTO Weekdays SELECT id, Day, WeekId, NULL FROM giraf_old.Weekdays;


Delete from Weekdays where WeekId in (select Id from Weeks where ThumbnailKey in (select Id from Pictograms WHERE Image is null));
Delete from Weeks where ThumbnailKey in (select Id from Pictograms WHERE Image is null);
Delete from Pictograms WHERE Image is null;

INSERT INTO Pictograms(id, Discriminator, LastEdit, AccessLevel, Image, Title, Sound) SELECT * FROM giraf_old.Frames WHERE Discriminator = "Pictogram";

SET @newId = 0;

INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 1, "#067700" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1) , SettingsKey, 2, "#8c1086" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 3, "#ff7f00" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 4, "#0017ff" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 5, "#ffdd00" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 6, "#ff0102" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 7, "#ffffff" From giraf_old.AspNetUsers;

DROP DATABASE giraf_old;

Delete from Weekdays where WeekId in (select Id from Weeks where ThumbnailKey in (select Id from Pictograms WHERE ImageHash is null));
Delete from Weeks where ThumbnailKey in (select Id from Pictograms WHERE ImageHash is null);
Delete from Pictograms WHERE ImageHash is null;

-- Update migrations-history from local db to prod-db
-- mysqldump -p12345678 giraf __EFMigrationsHistory | mysql --host web.giraf.cs.aau.dk --port 3333 -u <DB-USER> -p<DB-PASSWORD> giraf-release