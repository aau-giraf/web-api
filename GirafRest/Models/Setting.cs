using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;

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
        /// defines the number of days to display for a user in a weekschedule
        /// </summary>
        public int? NrOfDaysToDisplay { get; set; }

        /// <summary>
        /// Flag for indicating whether or not greyscale is enabled
        /// </summary>
        public bool GreyScale { get; set; }

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
            this.NrOfDaysToDisplay = newOptions?.NrOfDaysToDisplay ?? this.NrOfDaysToDisplay;
            this.GreyScale = newOptions?.GreyScale ?? this.GreyScale;
            if(newOptions.WeekDayColors != null)
                updateWeekDayColors(newOptions.WeekDayColors);
        }

        private void updateWeekDayColors(List<WeekDayColorDTO> weekDayColors){
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
        public void InitialiseWeekDayColors(){
            this.WeekDayColors = new List<WeekDayColor>(){
                new WeekDayColor(){Day = Days.Monday, HexColor = "#067700", SettingId = Key},
                new WeekDayColor(){Day = Days.Tuesday, HexColor = "#8c1086", SettingId = Key},
                new WeekDayColor(){Day = Days.Wednesday, HexColor = "#ff7f00", SettingId = Key},
                new WeekDayColor(){Day = Days.Thursday, HexColor = "#0017ff", SettingId = Key},
                new WeekDayColor(){Day = Days.Friday, HexColor = "#ffdd00", SettingId = Key},
                new WeekDayColor(){Day = Days.Saturday, HexColor = "#ff0102", SettingId = Key},
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
            DefaultTimer = DefaultTimer.analogClock;
            Theme = Theme.girafYellow;
            NrOfDaysToDisplay = 7;
            TimerSeconds = 900;
            GreyScale = false;

        }
    }
}
