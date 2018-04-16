using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
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
        /// A collection of all the user's applications.
        /// </summary>
        public virtual ICollection<ApplicationOption> appsUserCanAccess { get; set; }

        /// <summary>
        /// A field for storing how many rows to display in the GirafLauncher application.
        /// </summary>
        public int appGridSizeRows { get; set; }
        /// <summary>
        /// A field for storing how many columns to display in the GirafLauncher application.
        /// </summary>
        public int appGridSizeColumns { get; set; }
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
        [Required]
        public int timerSeconds { get; set; }
        /// <summary>
        /// Number of activities
        /// </summary>
        [Required]
        public int activitiesCount { get; set; }
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
            appsUserCanAccess = new List<ApplicationOption>();
        }
        /// <summary>
        /// Updates all settings based on a DTO
        /// </summary>
        /// <param name="newOptions">The DTO containing new settings</param>
        public void UpdateFrom (LauncherOptionsDTO newOptions) {
            this.appGridSizeColumns = newOptions.appGridSizeColumns;
            this.appGridSizeRows = newOptions.appGridSizeRows;
            this.appsUserCanAccess = newOptions.appsUserCanAccess;
            this.DisplayLauncherAnimations = newOptions.DisplayLauncherAnimations;
            this.orientation = newOptions.orientation;
            this.checkResourceAppearence = newOptions.checkResourceAppearence;
            this.defaultTimer = newOptions.defaultTimer;
            this.timerSeconds = newOptions.timerSeconds;
            this.activitiesCount = newOptions.activitiesCount;
            this.theme = newOptions.theme;
        }
    }

    /// <summary>
    /// Used to indicate that the user is allowed to use a given application.
    /// </summary>
    [ComplexType]
    public class ApplicationOption
    {
        /// <summary>
        /// The id of the ApplicationOption entity.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// The name of the application that the user is allowed to use.
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }
        /// <summary>
        /// The package in which the given application is located.
        /// </summary>
        [Required]
        public string ApplicationPackage { get; set; }

        /// <summary>
        /// Creates a new application option, that may be added to a given user.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <param name="applicationPackage">The package of the application.</param>
        public ApplicationOption(string applicationName, string applicationPackage)
        {
            ApplicationName = applicationName;
            ApplicationPackage = applicationPackage;
        }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public ApplicationOption()
        {

        }
    }
}
