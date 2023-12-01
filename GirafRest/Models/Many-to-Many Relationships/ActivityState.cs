namespace GirafRest.Models
{
    /// <summary>
    /// State of Activities
    /// </summary>
    public enum ActivityState
    {
        /// <summary>
        /// Normal default state
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Active state
        /// </summary>
        Active,

        /// <summary>
        /// When cancelled
        /// </summary>
        Canceled,

        /// <summary>
        /// After completion
        /// </summary>
        Completed
    }
}
