using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;

namespace GirafRest.Models
{
    /// <summary>
    /// The LauncherOptions, which is the various settings the users can add to customize the Launcher App.
    /// </summary>
    [ComplexType]
    public class LauncherOptions
    {
        /// <summary>
        /// Key for LauncherOptions
        /// </summary>
        [Key]
        public long Key { get; private set; }

        /// <summary>
        /// Preferred orientation of device/screen
        /// </summary>
        [Required]
        public Orientation Orientation { get; set; }
        /// <summary>
        /// Preferred appearence of checked resources
        /// </summary>
        [Required]
        public CompleteMark CompleteMark { get; set; }
        /// <summary>
        /// Preferred appearence of cancelled appearance    
        /// </summary>
        [Required]
        public CancelMark CancelMark { get; set; }
        /// <summary>
        /// Preferred appearence of timer
        /// </summary>
        [Required]
        public DefaultTimer DefaultTimer { get; set; }
        /// <summary>
        /// Number of seconds for timer
        /// </summary>
        public int? TimerSeconds { get; set; }
        /// <summary>
        /// Number of activities
        /// </summary>
        public int? ActivitiesCount { get; set; }
        /// <summary>
        /// The preferred theme
        /// </summary>
        [Required]
        public Theme Theme { get; set; }
        /// <summary>
        /// defines the number of days to display for a user in a weekschedule
        /// </summary>
        public int? NrOfDaysToDisplay { get; set; }

        /// <summary>
        /// Property for setting the color theme of weekschedules
        /// </summary>
        [Required]
        public ColorThemeWeekSchedules ColorThemeWeekSchedules { get; set; }

        /// <summary>
        /// Flag for indicating whether or not greyscale is enabled
        /// </summary>
        public bool GreyScale { get; set; }

        /// <summary>
        /// Required empty constructor
        /// </summary>
        public LauncherOptions()
        {
            Orientation = Orientation.portrait;
            CompleteMark = CompleteMark.Checkmark;
            CancelMark = CancelMark.Cross;
            DefaultTimer = DefaultTimer.analogClock;
            Theme = Theme.girafYellow;
            NrOfDaysToDisplay = 7;
            TimerSeconds = 900;
            ColorThemeWeekSchedules = ColorThemeWeekSchedules.standard;
            GreyScale = false;
        }
        /// <summary>
        /// Updates all settings based on a DTO
        /// </summary>
        /// <param name="newOptions">The DTO containing new settings</param>
        public void UpdateFrom(LauncherOptionsDTO newOptions)
        {
            this.Orientation = newOptions.Orientation;
            this.CompleteMark = newOptions.CompleteMark;
            this.CancelMark = newOptions.CancelMark;
            this.DefaultTimer = newOptions.DefaultTimer;
            this.TimerSeconds = newOptions.TimerSeconds;
            this.ActivitiesCount = newOptions.ActivitiesCount;
            this.Theme = newOptions.Theme;
            this.NrOfDaysToDisplay = newOptions.NrOfDaysToDisplay;
            this.ColorThemeWeekSchedules = newOptions.ColorThemeWeekSchedules;
            this.GreyScale = newOptions.GreyScale;
        }
    }
}
