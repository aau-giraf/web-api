using GirafEntities.User;

namespace GirafRepositories.Interfaces
{
    public interface IGirafRoleRepository : IRepository<GirafRole>
    {
        string GetGuardianRoleId();
        string GetCitizenRoleId();
        IEnumerable<string> GetUsersWithRole(string role);
        IEnumerable<string> GetAllGuardians();
        IEnumerable<string> GetAllCitizens();
    }
}