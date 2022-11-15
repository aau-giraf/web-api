using GirafRest.Models.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GirafRest.Models
{
    /// <summary>
    /// The LauncherOptions, which is the various settings the users can add to customize the Launcher App.
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// Settingskey
        /// </summary>
        [Key]
        public long Key { get; private set; }

        /// <summary>
        /// Preferred appearence of phone; Portrait or Landscape mode.
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
        /// Defines the number of days to display in portrait mode for a user in a weekplan
        /// </summary>
        public int? NrOfDaysToDisplayPortrait { get; set; }

        /// <summary>
        /// Defines the number of days to display in landscape mode for a user in a weekplan
        /// </summary>
        public int? NrOfDaysToDisplayLandscape { get; set; }

        /// <summary>
        /// true: if the first day shown in the weekplanner in landscape mode
        /// should be today
        /// false: if the first day shown in the weekplanner in landscape mode
        /// should be monday
        /// </summary>
        public bool DisplayDaysRelativeLandscape { get; set; }

        /// <summary>
        /// true: if the first day shown in the weekplanner in portrait mode
        /// should be today
        /// false: if the first day shown in the weekplanner in portrait mode
        /// should be monday
        /// </summary>
        public bool DisplayDaysRelativePortrait { get; set; }

        /// <summary>
        /// Flag for indicating whether or not greyscale is enabled
        /// </summary>
        public bool GreyScale { get; set; }

        /// <summary>
        /// Flag for indicating if pictogram text should be enabled or not
        /// </summary>
        public bool PictogramText { get; set; }

        /// <summary>
        /// Flag for indicating if popups should be enabled or not
        /// </summary>
        public bool ShowPopup { get; set; }

        /// <summary>
        /// Flag for indicating whether or not to show timer buttons
        /// </summary>
        public bool LockTimerControl { get; set; }

        /// <summary>
        /// Each day in a weekschedule has a hexcolor associated
        /// </summary>
        /// <value>The week day colors.</value>
        public List<WeekDayColor> WeekDayColors { get; set; }

        /// <summary>
        /// Updates all settings based on a DTO
        /// </summary>
        /// <param name="newOptions">The DTO containing new settings</param>
        public void UpdateFrom(SettingDTO newOptions)
        {
            this.Orientation = newOptions?.Orientation ?? this.Orientation;
            this.CompleteMark = newOptions?.CompleteMark ?? this.CompleteMark;
            this.CancelMark = newOptions?.CancelMark ?? this.CancelMark;
            this.DefaultTimer = newOptions?.DefaultTimer ?? this.DefaultTimer;
            this.TimerSeconds = newOptions?.TimerSeconds ?? this.TimerSeconds;
            this.ActivitiesCount = newOptions?.ActivitiesCount ?? this.ActivitiesCount;
            this.Theme = newOptions?.Theme ?? this.Theme;
            this.NrOfDaysToDisplayPortrait = newOptions?.NrOfDaysToDisplayPortrait ?? this.NrOfDaysToDisplayPortrait;
            this.DisplayDaysRelativePortrait = newOptions?.DisplayDaysRelativePortrait ?? this.DisplayDaysRelativePortrait;
            this.NrOfDaysToDisplayLandscape = newOptions?.NrOfDaysToDisplayLandscape ?? this.NrOfDaysToDisplayLandscape;
            this.DisplayDaysRelativeLandscape = newOptions?.DisplayDaysRelativeLandscape ?? this.DisplayDaysRelativeLandscape;
            this.GreyScale = newOptions?.GreyScale ?? this.GreyScale;
            this.LockTimerControl = newOptions?.LockTimerControl ?? this.LockTimerControl;
            this.PictogramText = newOptions?.PictogramText ?? this.PictogramText;
            this.ShowPopup = newOptions?.ShowPopup ?? this.ShowPopup;
            if (newOptions.WeekDayColors != null)
                updateWeekDayColors(newOptions.WeekDayColors);
        }

        private void updateWeekDayColors(List<WeekDayColorDTO> weekDayColors)
        {
            if (WeekDayColors != null)
            {
                foreach (var weekDayColor in weekDayColors)
                {
                    var weekDayColorToUpdate = this.WeekDayColors.FirstOrDefault(wdc => wdc.Day == weekDayColor.Day);
                    if (weekDayColorToUpdate != null)
                    {
                        weekDayColorToUpdate.HexColor = weekDayColor.HexColor;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes WeekDayColors.
        /// </summary>
        public void InitialiseWeekDayColors()
        {
            this.WeekDayColors = new List<WeekDayColor>(){
                new WeekDayColor(){Day = Days.Monday, HexColor = "#08a045", SettingId = Key},
                new WeekDayColor(){Day = Days.Tuesday, HexColor = "#540d6e", SettingId = Key},
                new WeekDayColor(){Day = Days.Wednesday, HexColor = "#f77f00", SettingId = Key},
                new WeekDayColor(){Day = Days.Thursday, HexColor = "#004777", SettingId = Key},
                new WeekDayColor(){Day = Days.Friday, HexColor = "#f9c80e", SettingId = Key},
                new WeekDayColor(){Day = Days.Saturday, HexColor = "#db2b39", SettingId = Key},
                new WeekDayColor(){Day = Days.Sunday, HexColor = "#ffffff", SettingId = Key},

            };
        }

        /// <summary>
        /// DO NOT DELETE
        /// </summary>
        public Setting()
        {
            Orientation = Orientation.portrait;
            CompleteMark = CompleteMark.Checkmark;
            CancelMark = CancelMark.Cross;
            DefaultTimer = DefaultTimer.pieChart;
            Theme = Theme.girafYellow;
            NrOfDaysToDisplayPortrait = 1;
            DisplayDaysRelativePortrait = true;
            NrOfDaysToDisplayLandscape = 7;
            DisplayDaysRelativePortrait = false;
            TimerSeconds = 900;
            GreyScale = false;
            PictogramText = false;
            ShowPopup = false;
            LockTimerControl = true;

        }
    }
}
