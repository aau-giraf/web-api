using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafEntities.WeekPlanner.DTOs;
using GirafRepositories.Interfaces;

namespace GirafServices.WeekPlanner;

public class WeekTemplateService : IWeekTemplateService
{
    private readonly IWeekTemplateRepository _weekTemplateRepository;

    public WeekTemplateService(IWeekTemplateRepository weekTemplateRepository)
    {
        _weekTemplateRepository = weekTemplateRepository;
    }
    
    public WeekTemplateNameDTO[] GetWeekTemplatesForUser(GirafUser user)
    {
        return _weekTemplateRepository.GetWeekTemplatesForUser(user);
    }

    public Task<WeekTemplate> GetWeekTemplateFromIdAndUser(long id, GirafUser user)
    {
        return _weekTemplateRepository.GetWeekTemplateFromIdAndUser(id, user);
    }

    public void AddWeekTemplate(WeekTemplate newTemplate)
    {
        _weekTemplateRepository.AddWeekTemplate(newTemplate);
    }

    public void UpdateWeekTemplate(WeekTemplate template)
    {
        _weekTemplateRepository.UpdateWeekTemplate(template);
    }

    public void RemoveTemplate(WeekTemplate template)
    {
        _weekTemplateRepository.RemoveTemplate(template);
    }

    public WeekTemplate GetWeekTemplateFromId(long id)
    {
        return _weekTemplateRepository.GetWeekTemplateFromId(id);
    }

    public Task<int> SaveChangesAsync()
    {
        return _weekTemplateRepository.SaveChangesAsync();
    }
}