using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

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
        /// A flag indicating whether to run applications in grayscale or not.
        /// </summary>
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
        /// Required empty constructor
        /// </summary>
        public LauncherOptions()
        {
        }
        /// <summary>
        /// Updates all settings based on a DTO
        /// </summary>
        /// <param name="newOptions">The DTO containing new settings</param>
        public void UpdateFrom(LauncherOptionsDTO newOptions)
        {
            this.appGridSizeColumns = newOptions.appGridSizeColumns;
            this.appGridSizeRows = newOptions.appGridSizeRows;
            this.DisplayLauncherAnimations = newOptions.DisplayLauncherAnimations;
            this.orientation = newOptions.orientation;
            this.checkResourceAppearence = newOptions.checkResourceAppearence;
            this.defaultTimer = newOptions.defaultTimer;
            this.timerSeconds = newOptions.timerSeconds;
            this.activitiesCount = newOptions.activitiesCount;
            this.theme = newOptions.theme;
        }
    }
}
