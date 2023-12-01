using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    /// <summary>
    /// Model for holding a timer, start time, progress etc.
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// The key of the relationship entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        /// <summary>
        /// Start time of the timer
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// The progress of the timer in miliseconds.
        /// </summary>
        public long Progress { get; set; }

        /// <summary>
        /// The full length of the timer.
        /// </summary>
        public long FullLength { get; set; }

        /// <summary>
        /// Boolean for setting whether the timer is paused.
        /// </summary>
        public bool Paused { get; set; }
    }
}
