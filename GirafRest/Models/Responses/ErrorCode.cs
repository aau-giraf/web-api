﻿namespace GirafRest.Models.Responses
{
#pragma warning disable 1591
    public enum ErrorCode
    {
        Error = 1,
        FormatError,
        NoError,
        NotAuthorized,
        NotFound,
        ApplicationNotFound,
        ChoiceContainsInvalidPictogramId,
        CitizenAlreadyHasGuardian,
        CitizenNotFound,
        DepartmentAlreadyOwnsResource,
        DepartmentNotFound,
        EmailServiceUnavailable,
        ImageAlreadyExistOnPictogram,
        ImageNotContainedInRequest,
        InvalidCredentials,
        InvalidProperties,
        MissingProperties,
        NoWeekScheduleFound,
        PasswordNotUpdated,
        PictogramHasNoImage,
        PictogramNotFound,
        QueryFailed,
        ResourceMustBePrivate,
        ResourceNotFound,
        ResourceNotOwnedByDepartment,
        ResourceIDUnreadable,
        RoleMustBeCitizien,
        RoleNotFound,
        ThumbnailDoesNotExist,
        UserAlreadyExists,
        UserNameAlreadyTakenWithinDepartment,
        UserAlreadyHasAccess,
        UserAlreadyHasIconUsePut,
        UserAlreadyOwnsResource,
        UserAndCitizenMustBeInSameDepartment,
        UserCannotBeGuardianOfYourself,
        UserDoesNotOwnResource,
        UserHasNoIcon,
        UserHasNoIconUsePost,
        UserMustBeGuardian,
        UserNotFound,
        WeekScheduleNotFound,
        Forbidden,
        PasswordMissMatch,
        TwoDaysCannotHaveSameDayProperty,
        UserHasNoCitizens,
        UserHasNoGuardians,
        DepartmentHasNoCitizens,
        UnknownError,
        CouldNotCreateDepartmentUser,
        UserNotFoundInDepartment,
        NoWeekTemplateFound,
        UserAlreadyHasDepartment,
        MissingSettings,
        InvalidAmountOfWeekdays,
        WeekAlreadyExists,
        InvalidDay,
        DuplicateWeekScheduleName,
        ColorMustHaveUniqueDay,
        InvalidHexValues,
        WeekTemplateNotFound,
        NotImplemented,
        UserMustBeInDepartment,
        WeekNotFound,
        ActivityNotFound
    }
#pragma warning restore 1591
}
