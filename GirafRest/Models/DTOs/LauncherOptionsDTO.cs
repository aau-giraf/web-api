using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// A Data Transfer Object for the user settings used by the launcher
    /// </summary>
    public class LauncherOptionsDTO
    {
        /// <summary>
        /// A flag indicating whether to display animations in the launcher or not.
        /// </summary>
        [Required]
        public bool DisplayLauncherAnimations { get; set; }
        /// <summary>
        /// A field for storing how many rows to display in the GirafLauncher application.
        /// </summary>
        public int? AppGridSizeRows { get; set; }
        /// <summary>
        /// A field for storing how many columns to display in the GirafLauncher application.
        /// </summary>
        public int? AppGridSizeColumns { get; set; }
        /// <summary>
        /// Preferred orientation of device/screen
        /// </summary>
        [Required]
        public Orientation Orientation { get; set; }
        /// <summary>
        /// Preferred appearence of checked resources
        /// </summary>
        [Required]
        public ResourceAppearence CheckResourceAppearence { get; set; }
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
        /// Constructor to create a DTO based on the actual object
        /// </summary>
        /// <param name="options">The launcher options in need of transfer</param>
        public LauncherOptionsDTO(LauncherOptions options)
        {
            this.AppGridSizeColumns = options.AppGridSizeColumns;
            this.AppGridSizeRows = options.AppGridSizeRows;
            this.DisplayLauncherAnimations = options.DisplayLauncherAnimations;
            this.Orientation = options.Orientation;
            this.CheckResourceAppearence = options.CheckResourceAppearence;
            this.DefaultTimer = options.DefaultTimer;
            this.TimerSeconds = options.TimerSeconds;
            this.ActivitiesCount = options.ActivitiesCount;
            this.Theme = options.Theme;
            this.NrOfDaysToDisplay = options.NrOfDaysToDisplay;
        }

        public LauncherOptionsDTO()
        {
            Theme = Theme.girafYellow;
            DefaultTimer = DefaultTimer.hourglass;
            CheckResourceAppearence = ResourceAppearence.checkmark;
            Orientation = Orientation.portrait;
        }
    }
}