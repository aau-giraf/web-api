using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafEntities.WeekPlanner.DTOs;

namespace GirafServices.WeekPlanner;

public interface IWeekTemplateService
{
    WeekTemplateNameDTO[] GetWeekTemplatesForUser(GirafUser user);
    Task<WeekTemplate> GetWeekTemplateFromIdAndUser(long id, GirafUser user);
    void AddWeekTemplate(WeekTemplate newTemplate);
    void UpdateWeekTemplate(WeekTemplate template);
    void RemoveTemplate(WeekTemplate template);
    WeekTemplate GetWeekTemplateFromId(long id);
    Task<int> SaveChangesAsync();
}