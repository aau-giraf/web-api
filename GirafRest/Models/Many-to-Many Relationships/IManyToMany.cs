namespace GirafRest.Models
{
    /// <summary>
    /// Defines what data should be present when defining a many-to-many relationship.
    /// </summary>
    /// <typeparam name="TOwnerKey">The type of the primary key of the owner.</typeparam>
    /// <typeparam name="TOwnerType">The type of the owner.</typeparam>
    public interface IManyToMany<TOwnerKey, TOwnerType>
        where TOwnerType : class
    {
        /// <summary>
        /// The key of the relationship entity.
        /// </summary>
        long Key {get; set;}

        /// <summary>
        /// The key of the involved resource.
        /// </summary>
        long ResourceKey { get; set; }
        /// <summary>
        /// A reference to the actual resource.
        /// </summary>
        Frame Resource { get; set; }

        /// <summary>
        /// The key of the involved owner.
        /// </summary>
        TOwnerKey OtherKey { get; set; }
        /// <summary>
        /// A reference to the actual owner.
        /// </summary>
        TOwnerType Other { get; set; }
    }
}