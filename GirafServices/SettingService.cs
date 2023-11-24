using GirafEntities.Settings.DTOs;
using GirafEntities.WeekPlanner.DTOs;
using GirafEntities.User;

namespace GirafServices
{
    public class SettingService
    {
        /// <summary>
        /// Updates all settings based on a DTO
        /// </summary>
        /// <param name="newOptions">The DTO containing new settings</param>
        public void UpdateFrom(SettingDTO newSetting, GirafUser user)
        {
            user.Settings.Orientation = newSetting?.Orientation ?? user.Settings.Orientation;
            user.Settings.CompleteMark = newSetting?.CompleteMark ?? user.Settings.CompleteMark;
            user.Settings.CancelMark = newSetting?.CancelMark ?? user.Settings.CancelMark;
            user.Settings.DefaultTimer = newSetting?.DefaultTimer ?? user.Settings.DefaultTimer;
            user.Settings.TimerSeconds = newSetting?.TimerSeconds ?? user.Settings.TimerSeconds;
            user.Settings.ActivitiesCount = newSetting?.ActivitiesCount ?? user.Settings.ActivitiesCount;
            user.Settings.Theme = newSetting?.Theme ?? user.Settings.Theme;
            user.Settings.NrOfDaysToDisplayPortrait = newSetting?.NrOfDaysToDisplayPortrait ?? user.Settings.NrOfDaysToDisplayPortrait;
            user.Settings.DisplayDaysRelativePortrait = newSetting?.DisplayDaysRelativePortrait ?? user.Settings.DisplayDaysRelativePortrait;
            user.Settings.NrOfDaysToDisplayLandscape = newSetting?.NrOfDaysToDisplayLandscape ?? user.Settings.NrOfDaysToDisplayLandscape;
            user.Settings.DisplayDaysRelativeLandscape = newSetting?.DisplayDaysRelativeLandscape ?? user.Settings.DisplayDaysRelativeLandscape;
            user.Settings.GreyScale = newSetting?.GreyScale ?? user.Settings.GreyScale;
            user.Settings.LockTimerControl = newSetting?.LockTimerControl ?? user.Settings.LockTimerControl;
            user.Settings.PictogramText = newSetting?.PictogramText ?? user.Settings.PictogramText;
            user.Settings.ShowPopup = newSetting?.ShowPopup ?? user.Settings.ShowPopup;
            user.Settings.NrOfActivitiesToDisplay = newSetting?.NrOfActivitiesToDisplay ?? user.Settings.NrOfActivitiesToDisplay;
            user.Settings.ShowOnlyActivities = newSetting?.ShowOnlyActivities ?? user.Settings.ShowOnlyActivities;
            user.Settings.ShowSettingsForCitizen = newSetting?.ShowSettingsForCitizen ?? user.Settings        .ShowSettingsForCitizen;
            if (newSetting.WeekDayColors != null)
                UpdateWeekDayColors(newSetting.WeekDayColors, user);
        }

        private void UpdateWeekDayColors(List<WeekDayColorDTO> weekDayColors, GirafUser user)
        {
            if (user.Settings.WeekDayColors != null)
            {
                foreach (var weekDayColor in weekDayColors)
                {
                    var weekDayColorToUpdate = user.Settings.WeekDayColors.FirstOrDefault(wdc => wdc.Day == weekDayColor.Day);
                    if (weekDayColorToUpdate != null)
                    {
                        weekDayColorToUpdate.HexColor = weekDayColor.HexColor;
                    }
                }
            }
        }
    }
}
