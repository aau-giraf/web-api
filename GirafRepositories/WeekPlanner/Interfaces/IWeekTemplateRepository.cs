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
        void AddWeekTemplate(WeekTemplate newTemplate);
        void UpdateWeekTemplate(WeekTemplate template);
        void RemoveTemplate(WeekTemplate template);
        WeekTemplate GetWeekTemplateFromId(long id);
        Task<int> SaveChangesAsync();
    }
}