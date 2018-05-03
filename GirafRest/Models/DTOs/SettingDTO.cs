using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GirafRest.Models;

namespace GirafRest.Models.DTOs
{
    public enum Orientation { portrait = 1, landscape = 2}
    public enum CompleteMark { Removed = 1, Checkmark = 2, MovedRight = 3}
    public enum CancelMark { Removed = 1, Cross = 2}
    public enum DefaultTimer { hourglass = 1, analogClock = 2 }
    public enum Theme { girafYellow = 1, girafGreen = 2, girafRed = 3, androidBlue = 4}
    public enum ColorThemeWeekSchedules {standard = 1, yellowAndWhite = 2}
    /// <summary>
    /// A Data Transfer Object for the user settings used by the launcher
    /// </summary>
    public class SettingDTO
    {
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
        /// Preferred appearence of cancelled resources
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
        /// Property for setting the color theme of weekschedules
        /// </summary>
        [Required]
        public ColorThemeWeekSchedules ColorThemeWeekSchedules { get; set; }
        /// <summary>
        /// defines the number of days to display for a user in a weekschedule
        /// </summary>
        public int? NrOfDaysToDisplay { get; set; }
        /// <summary>
        /// Flag for indicating whether or not greyscale is enabled
        /// </summary>
        public bool GreyScale { get; set; }

        public List<WeekDayColorDTO>  WeekDayColors {get; set;}
        /// <summary>
        /// Constructor to create a DTO based on the actual object
        /// </summary>
        /// <param name="options">The launcher options in need of transfer</param>
        public SettingDTO(Setting options)
        {
            this.Orientation = options.Orientation;
            this.CompleteMark = options.CompleteMark;
            this.CancelMark = options.CancelMark;
            this.DefaultTimer = options.DefaultTimer;
            this.TimerSeconds = options.TimerSeconds;
            this.ActivitiesCount = options.ActivitiesCount;
            this.Theme = options.Theme;
            this.NrOfDaysToDisplay = options.NrOfDaysToDisplay;
            this.ColorThemeWeekSchedules = options.ColorThemeWeekSchedules;
            this.GreyScale = options.GreyScale;
            this.WeekDayColors = SetWeekDayColorsFromModel(options.WeekDayColors);
        }

        public SettingDTO()
        {
            Orientation = Orientation.portrait;
            CompleteMark = CompleteMark.Checkmark;
            CancelMark = CancelMark.Cross;
            DefaultTimer = DefaultTimer.analogClock;
            Theme = Theme.girafYellow;
            ColorThemeWeekSchedules = ColorThemeWeekSchedules.yellowAndWhite;
        }

        private List<WeekDayColorDTO> SetWeekDayColorsFromModel(List<WeekDayColor> weekDayColors){
            if (weekDayColors != null)
            {
                var WeekDayColorDTOs = new List<WeekDayColorDTO>();
                foreach (var weekDayColor in weekDayColors)
                {
                    WeekDayColorDTOs.Add(new WeekDayColorDTO()
                    {
                        Day = weekDayColor.Day,
                        HexColor = weekDayColor.HexColor
                    });
                }

                return WeekDayColorDTOs;
            }
            return null;
        }
    }
}