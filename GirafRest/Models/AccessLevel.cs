namespace GirafRest.Models
{
    /// <summary>
    /// Defines access levels of resources in the system.
    /// </summary>
    public enum AccessLevel
    {
        //Accessible by everyone, even non-users.
        PUBLIC = 0,

        //Accessible to all users associated with the owning department.
        PROTECTED = 1,


        //Accessible only to the owning user.
        PRIVATE = 2
    }
}