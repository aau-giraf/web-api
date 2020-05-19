namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for <see cref="Timer"/>
    /// </summary>
    public class TimerDTO
    {
        /// <summary>
        /// Constructor for DTO
        /// </summary>
        public TimerDTO(long startTime, long progress, long fullLength, bool paused, long key)
        {
            StartTime = startTime;
            Progress = progress;
            FullLength = fullLength;
            Paused = paused;
            Key = key;
        }

        /// <summary>
        /// Constructor given a Timer
        /// </summary>
        public TimerDTO(Timer timer)
        {
            StartTime = timer.StartTime;
            Progress = timer.Progress;
            FullLength = timer.FullLength;
            Paused = timer.Paused;
            Key = timer.Key;
        }

        /// <summary>
        /// Empty constructor for JSON Generation
        /// </summary>
        public TimerDTO() { }

        /// <summary>
        /// Start time of the timers
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

        /// <summary>
        /// Key for Timer
        /// </summary>
        public long? Key { get; set; }
    }
}