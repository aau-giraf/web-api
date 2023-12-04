using GirafEntities.User;
using GirafEntities.WeekPlanner;

namespace GirafServices.WeekPlanner;

public interface IPictogramService
{
    IEnumerable<Pictogram> fetchingPictogramsFromDepartment(string query, GirafUser user);
    IEnumerable<Pictogram> fetchingPictogramsUserNotInDepartment(string query, GirafUser user);
    IEnumerable<Pictogram> fetchPictogramsNoUserLoggedIn(string query);

    Task<IEnumerable<Pictogram>> ReadAllPictograms(string query, GirafUser user);
    Task<bool> CheckOwnership(Pictogram picto, GirafUser usr);
}