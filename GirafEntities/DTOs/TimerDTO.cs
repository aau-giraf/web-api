namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for <see cref="Timer"/>
    /// </summary>
    public class TimerDTO
    {
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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(TimerDTO))
                return false;

            TimerDTO dto = (TimerDTO)obj;
            return this.StartTime == dto.StartTime && this.Progress == dto.Progress && this.FullLength == dto.FullLength && this.Paused == dto.Paused && this.Key == dto.Key;
        }
    }
}
