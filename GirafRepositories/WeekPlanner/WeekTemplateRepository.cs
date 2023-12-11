using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafEntities.WeekPlanner.DTOs;
using GirafRepositories.Persistence;
using GirafRepositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GirafRepositories.WeekPlanner
{
    public class WeekTemplateRepository : Repository<WeekTemplate>, IWeekTemplateRepository
    {
        public WeekTemplateRepository(GirafDbContext context) : base(context)
        {

        }

        public WeekTemplateNameDTO[] GetWeekTemplatesForUser(GirafUser user)
        {
            return Context.WeekTemplates
                .Where(t => t.DepartmentKey == user.DepartmentKey)
                .Select(w => new WeekTemplateNameDTO(w.Name, w.Id)).ToArray();
        }

        public async Task<WeekTemplate> GetWeekTemplateFromIdAndUser(long id, GirafUser user)
        {
            return await Context.WeekTemplates
                .Include(w => w.Thumbnail)
                .Include(u => u.Weekdays)
                .ThenInclude(wd => wd.Activities)
                .ThenInclude(pa => pa.Pictograms)
                .ThenInclude(p => p.Pictogram)
                .Where(t => t.DepartmentKey == user.DepartmentKey)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public void AddWeekTemplate(WeekTemplate newTemplate)
        {
            Context.WeekTemplates.Add(newTemplate);
        }

        public void UpdateWeekTemplate(WeekTemplate template)
        {
            Context.WeekTemplates.Update(template);
        }

        public void RemoveTemplate(WeekTemplate template)
        {
            Context.WeekTemplates.Remove(template);
        }

        public WeekTemplate GetWeekTemplateFromId(long id)
        {
            return Context.WeekTemplates
                .Include(w => w.Thumbnail)
                .Include(u => u.Weekdays)
                .ThenInclude(wd => wd.Activities)
                .ThenInclude(e => e.Pictograms)
                .FirstOrDefault(t => id == t.Id);
        }

        /// <summary>
        /// Saves changes.
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }
    }
}
