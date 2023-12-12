using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafEntities.WeekPlanner.DTOs;
using GirafRepositories.Interfaces;

namespace GirafRepositories.Interfaces
{
    public interface IWeekTemplateRepository : IRepository<WeekTemplate>
    {
        WeekTemplateNameDTO[] GetWeekTemplatesForUser(GirafUser user);
        Task<WeekTemplate> GetWeekTemplateFromIdAndUser(long id, GirafUser user);
        Task AddWeekTemplate(WeekTemplate newTemplate);
        Task UpdateWeekTemplate(WeekTemplate template);
        Task RemoveTemplate(WeekTemplate template);
        WeekTemplate GetWeekTemplateFromId(long id);
    }
}