namespace GirafRest.Models
{
    public enum AccessLevel {
    /**
     * Accessible by everyone, even non-users.
     */
    PUBLIC = 0,
    /**
     * Accessible to everyone associated with the owning department.
     */
    PROTECTED = 1, 
    /**
     * Accessible only to the owning user.
     */
    PRIVATE = 2
    }
}