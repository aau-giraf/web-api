namespace GirafRest.Models
{
    /// <summary>
    /// Defines access levels of resources in the system.
    /// </summary>
    public enum AccessLevel
    {
        //Accessible by everyone, even non-users.
        PUBLIC = 1,

        //Accessible to all users associated with the owning department.
        PROTECTED = 2,

        //Accessible only to the owning user.
        PRIVATE = 3
    }
}