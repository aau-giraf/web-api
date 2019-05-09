using System;
namespace GirafRest.Models.DTOs
{
    public class TimerDTO
    {
        public TimerDTO(string startTime, long progress, long fullLength, bool paused)
        {
            StartTime = startTime;
            Progress = progress;
            FullLength = fullLength;
            Paused = paused;
        }

        public TimerDTO(Timer timer) { 
            StartTime = timer.StartTime;
            Progress = timer.Progress;
            FullLength = timer.FullLength;
            Paused = timer.Paused;
        }

        public TimerDTO() { }

        /// <summary>
        /// Start time of the timers
        /// </summary>
        public string StartTime {get; set;}

        /// <summary>
        /// The progress of the timer in miliseconds.
        /// </summary>
        public long Progress {get; set;}

        /// <summary>
        /// The full length of the timer.
        /// </summary>
        public long FullLength {get; set;}

        /// <summary>
        /// Boolean for setting whether the timer is paused.
        /// </summary>
        public bool Paused {get; set;}
    }
}