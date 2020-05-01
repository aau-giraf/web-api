namespace GirafRest.Models
{
    /// <summary>
    /// Defines access levels of <see cref="Pictogram"/> in the system.
    /// </summary>
    public enum AccessLevel
    {
        /// <summary>Accessible by everyone, even non-users.</summary>
        PUBLIC = 1,

        /// <summary>Accessible to all users associated with the owning department.</summary>
        PROTECTED = 2,

        /// <summary>Accessible only to the owning user.</summary>
        PRIVATE = 3
    }
}