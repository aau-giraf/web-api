using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafServices.User;

namespace GirafServices.WeekPlanner;

public class PictogramService : IPictogramService
{
    private readonly IPictogramRepository _pictogramRepository;
    private readonly IUserService _userService;

    public PictogramService(IPictogramRepository pictogramRepository, IUserService userService)
    {
        _pictogramRepository = pictogramRepository;
        _userService = userService;
    }
    public IEnumerable<Pictogram> fetchingPictogramsFromDepartment(string query, GirafUser user)
    {
        return _pictogramRepository.fetchPictogramsFromDepartmentStartsWithQuery(query, user)
            .Union(_pictogramRepository.fetchPictogramsFromDepartmentsContainsQuery(query, user));
    }

    public IEnumerable<Pictogram> fetchingPictogramsUserNotInDepartment(string query, GirafUser user)
    {
        return _pictogramRepository.fetchPictogramsUserNotPartOfDepartmentStartsWithQuery(query, user)
            .Union(_pictogramRepository.fetchPictogramsUserNotPartOfDepartmentContainsQuery(query, user));
    }

    public IEnumerable<Pictogram> fetchPictogramsNoUserLoggedIn(string query)
    {
        return _pictogramRepository.fetchPictogramsNoUserLoggedInStartsWithQuery(query)
            .Union(_pictogramRepository.fetchPictogramsNoUserLoggedInContainsQuery(query));
    }

    /// <summary>
    /// Read all pictograms available to the current user (or only the PUBLIC ones if no user is authorized).
    /// </summary>
    /// <returns>A list of said pictograms.</returns>
    public async Task<IEnumerable<Pictogram>> ReadAllPictograms(string query, GirafUser user)
    {
        //In this method .AsNoTracking is used due to a bug in EntityFramework Core, where we are not allowed to call a constructor in .Select
        //i.e. convert the pictograms to PictogramDTOs.
        try
        {
            //Find the user and add his pictograms to the result
            if (query != null)
            {
                query = query.ToLower().Replace(" ", string.Empty);
            }
            if (user != null)
            {
                // User is a part of a department
                if (user.Department != null)
                {
                    return fetchingPictogramsFromDepartment(query, user).AsEnumerable();
                }
                // User is not part of a department
                return fetchingPictogramsUserNotInDepartment(query, user).AsEnumerable();
            }

            // Fetch all public pictograms as there is no user.
            return fetchPictogramsNoUserLoggedIn(query).AsEnumerable();
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// Checks if the user has some form of ownership of the pictogram.
    /// </summary>
    /// <param name="picto">The Pictogram in need of checking.</param>
    /// <param name="usr">The user in question.</param>
    /// <returns>A bool indicating whether the user owns the pictogram or not.</returns>
    public async Task<bool> CheckOwnership(Pictogram picto, GirafUser usr)
    {
        var ownsPictogram = false;
        switch (picto.AccessLevel)
        {
            case AccessLevel.PUBLIC:
                ownsPictogram = true;
                break;

            case AccessLevel.PROTECTED:
                ownsPictogram = await _userService.CheckProtectedOwnership(picto, usr);
                break;

            case AccessLevel.PRIVATE:
                ownsPictogram = await _userService.CheckPrivateOwnership(picto, usr);
                break;

            default:
                break;
        }

        return ownsPictogram;
    }
}