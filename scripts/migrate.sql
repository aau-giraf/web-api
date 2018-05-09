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
INSERT INTO Pictograms SELECT id, AccessLevel, TO_BASE64(Image), LastEdit, Sound, Title FROM giraf_old.Frames WHERE Discriminator = "Pictogram";


INSERT INTO Weeks SELECT id, GirafUserId, Name, ThumbnailKey, 1, 1864 FROM giraf_old.Weeks;
INSERT INTO Weekdays SELECT id, Day, WeekId, NULL FROM giraf_old.Weekdays;


Delete from Weekdays where WeekId in (select Id from Weeks where ThumbnailKey in (select Id from Pictograms WHERE Image is null));
Delete from Weeks where ThumbnailKey in (select Id from Pictograms WHERE Image is null);
Delete from Pictograms WHERE Image is null;

SET @newId = 0;

INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 1, "#067700" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1) , SettingsKey, 2, "#8c1086" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 3, "#ff7f00" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 4, "#0017ff" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 5, "#ffdd00" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 6, "#ff0102" From giraf_old.AspNetUsers;
INSERT INTO WeekDayColors (Id, SettingId, Day, HexColor) SELECT (@newId := @newId + 1), SettingsKey, 7, "#ffffff" From giraf_old.AspNetUsers;

DROP DATABASE giraf_old;

-- # Insert one guardian more, named Gorm
-- INSERT INTO `LauncherOptions` (`Key`, `activitiesCount`, `appGridSizeColumns`, `appGridSizeRows`, `checkResourceAppearence`, `defaultTimer`, `displayLauncherAnimations`, `orientation`, `theme`, `timerSeconds`) VALUES
-- (6, NULL,   NULL,   NULL,   1,  2,  CONV('0', 2, 10) + 0,   1,  1,  NULL);
-- INSERT INTO `AspNetUsers` (`Id`, `AccessFailedCount`, `ConcurrencyStamp`, `DepartmentKey`, `DisplayName`, `Email`, `EmailConfirmed`, `IsDepartment`, `LockoutEnabled`, `LockoutEnd`, `NormalizedEmail`, `NormalizedUserName`, `PasswordHash`, `PhoneNumber`, `PhoneNumberConfirmed`, `SecurityStamp`, `SettingsKey`, `TwoFactorEnabled`, `UserIcon`, `UserName`) VALUES
-- ('b2466bcd-14fd-4f56-a053-b133908fff44',    0,  '58765e22-fbec-45f8-bb54-0ad7f663dddc', 1,  NULL,   NULL,   CONV('0', 2, 10) + 0,   CONV('0', 2, 10) + 0,   CONV('1', 2, 10) + 0,   NULL,   NULL,   'GORM', 'AQAAAAEAACcQAAAAEOtxV3MLz7Pu6MPtE54xKvK+GuipWwm6RM14lPBa2Sm9UttwHIs4YWZ6GpyWPjhtww==', NULL,   CONV('0', 2, 10) + 0,   '8c31711a-a06d-4dca-9ff9-a7283e4f3ec9', 6,  CONV('0', 2, 10) + 0,   NULL,   'Gorm');
-- INSERT INTO AspNetUserRoles values("b2466bcd-14fd-4f56-a053-b133908fff44", "dce9d3c7-0ca1-4fb5-915d-c18cb902e12e");