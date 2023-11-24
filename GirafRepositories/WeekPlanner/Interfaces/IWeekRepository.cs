using GirafEntities.Responses;
using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafEntities.WeekPlanner.DTOs;

namespace GirafRepositories.Interfaces
{
    public interface IWeekRepository : IRepository<Week>
    {
        public Task<GirafUser> getAllWeeksOfUser(string userId);

        Task<GirafUser> LoadUserWithWeekSchedules(string userId);
        public Task<int> DeleteSpecificWeek(GirafUser user, Week week);
        public Task<int> UpdateSpecificWeek(Week week);
    }
}