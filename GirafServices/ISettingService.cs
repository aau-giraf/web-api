using GirafEntities.Settings.DTOs;
using GirafEntities.User;
using GirafEntities.WeekPlanner.DTOs;

namespace GirafServices;

public interface ISettingService
{
    void UpdateFrom(SettingDTO newSetting, GirafUser user);
    void UpdateWeekDayColors(List<WeekDayColorDTO> weekDayColors, GirafUser user);
}