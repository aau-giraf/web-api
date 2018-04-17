using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs
{
    public enum orientation_enum { portrait = 1, landscape = 2 }
    public enum resourceAppearence_enum { normal = 1, checkmark = 2, removed = 3, movedToRight = 4, greyedOut = 5 }
    public enum defaultTimer_enum { hourglass = 1, analogClock = 2 }
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
        public int? appGridSizeRows { get; set; }
        /// <summary>
        /// A field for storing how many columns to display in the GirafLauncher application.
        /// </summary>
        public int? appGridSizeColumns { get; set; }
        /// <summary>
        /// Preferred orientation of device/screen
        /// </summary>
        [Required]
        public orientation_enum orientation { get; set; }
        /// <summary>
        /// Preferred appearence of checked resources
        /// </summary>
        [Required]
        public resourceAppearence_enum checkResourceAppearence { get; set; }
        /// <summary>
        /// Preferred appearence of timer
        /// </summary>
        [Required]
        public defaultTimer_enum defaultTimer { get; set; }
        /// <summary>
        /// Number of seconds for timer
        /// </summary>
        public int? timerSeconds { get; set; }
        /// <summary>
        /// Number of activities
        /// </summary>
        public int? activitiesCount { get; set; }
        /// <summary>
        /// The preferred theme
        /// </summary>
        [Required]
        public theme_enum theme { get; set; }
        /// <summary>
        /// Constructor to create a DTO based on the actual object
        /// </summary>
        /// <param name="options">The launcher options in need of transfer</param>
        public LauncherOptionsDTO(LauncherOptions options)
        {
            this.appGridSizeColumns = options.appGridSizeColumns;
            this.appGridSizeRows = options.appGridSizeRows;
            this.DisplayLauncherAnimations = options.DisplayLauncherAnimations;
            this.orientation = options.orientation;
            this.checkResourceAppearence = options.checkResourceAppearence;
            this.defaultTimer = options.defaultTimer;
            this.timerSeconds = options.timerSeconds;
            this.activitiesCount = options.activitiesCount;
            this.theme = options.theme;
        }
        /// <summary>
        /// Simple constructor ensuring that appsUserCanAccess isn't null
        /// </summary>
        public LauncherOptionsDTO()
        {
            theme = theme_enum.girafYellow;
            defaultTimer = defaultTimer_enum.hourglass;
            checkResourceAppearence = resourceAppearence_enum.checkmark;
            orientation = orientation_enum.portrait;
        }
    }
}