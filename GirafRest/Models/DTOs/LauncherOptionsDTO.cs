using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs
{
    public enum orientation_enum { portrait = 1, landscape = 2 }
    public enum resourceAppearence_enum { normal = 1, checkmark = 2, removed = 3, movedToRight = 4, greyedOut = 5 }
    public enum defaultTimer_enum { hourglass = 1,analogClock = 2 }
    public enum theme_enum { girafYellow = 1, girafGreen = 2, greyscale = 3 }

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
        public orientation_enum Orientation { get; set; }
        /// <summary>
        /// Preferred appearence of checked resources
        /// </summary>
        [Required]
        public resourceAppearence_enum CheckResourceAppearence { get; set; }
        /// <summary>
        /// Preferred appearence of timer
        /// </summary>
        [Required]
        public defaultTimer_enum DefaultTimer { get; set; }
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
        public theme_enum Theme { get; set; }

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
        /// <summary>
        /// Simple constructor ensuring that appsUserCanAccess isn't null
        /// </summary>
        public LauncherOptionsDTO()
        {
            Theme = theme_enum.girafYellow;
            DefaultTimer = defaultTimer_enum.hourglass;
            CheckResourceAppearence = resourceAppearence_enum.checkmark;
            Orientation = orientation_enum.portrait;
        }
    }
}