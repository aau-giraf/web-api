using GirafEntities.WeekPlanner;
using Timer = GirafEntities.WeekPlanner.Timer;
using GirafEntities.Responses;
using GirafEntities.WeekPlanner.DTOs;
using GirafRepositories.Interfaces;



namespace GirafServices.WeekPlanner
{
    /// <summary>
    /// From the given DTO, set the name, thumbnail and days of the given week object.
    /// </summary>
    /// <param name="weekDTO">The DTO from which values are read.</param>
    /// <param name="week">The week object to which values are written.</param>
    /// <param name="_giraf">An instance of the GirafService from which the database will be accessed when reading the DTO.</param>
    /// <returns>MissingProperties if thumbnail is missing.
    /// ResourceNotFound if any pictogram id is invalid.
    /// null otherwise.</returns>
    public class WeekService : IWeekService
    {
        private readonly IPictogramRepository _pictogramRepository;
        private readonly ITimerRepository _timerRepository;
        private readonly IWeekBaseService _weekBaseService;

        public WeekService(IPictogramRepository pictogramRepository, ITimerRepository timerRepository, IWeekBaseService weekBaseService)
        {
            _pictogramRepository = pictogramRepository;
            _timerRepository = timerRepository;
            _weekBaseService = weekBaseService;
        }

        public async Task<ErrorResponse> SetWeekFromDTO(WeekBaseDTO weekDTO, WeekBase week)
        {
            var modelErrorCode = weekDTO.ValidateModel();
            if (modelErrorCode.HasValue)
            {
                return new ErrorResponse(modelErrorCode.Value, "Invalid model");
            }

            week.Name = weekDTO.Name;

            Pictogram thumbnail = await _pictogramRepository.GetPictogramsById(weekDTO.Thumbnail.Id);

            if (thumbnail == null)
            {
                return new ErrorResponse(ErrorCode.MissingProperties, "Missing thumbnail");
            }

            week.Thumbnail = thumbnail;

            foreach (var day in weekDTO.Days)
            {
                var wkDay = new Weekday(day);
                if (!(await AddPictogramsToWeekday(wkDay, day)))
                {
                    return new ErrorResponse(ErrorCode.ResourceNotFound, "Missing pictogram");
                }

                _weekBaseService.UpdateDay(wkDay, week);
            }

            // All week days that were not specified in the new schedule, but existed before
            var toBeDeleted = week.Weekdays.Where(wd => !weekDTO.Days.Any(d => d.Day == wd.Day)).ToList();
            foreach (var deletedDay in toBeDeleted)
            {
                week.Weekdays.Remove(deletedDay);
            }

            return null;
        }

        /// <summary>
        /// Take pictograms and choices from DTO and add them to weekday object.
        /// </summary>
        /// <returns>True if all pictograms and choices were found and added, and false otherwise.</returns>
        /// <param name="to">Pictograms and choices will be added to this object.</param>
        /// <param name="from">Pictograms and choices will be read from this object.</param>
        public async Task<bool> AddPictogramsToWeekday(Weekday to, WeekdayDTO from)
        {
            if (from.Activities != null)
            {
                foreach (var activityDTO in from.Activities)
                {

                    List<Pictogram> pictograms = new List<Pictogram>();

                    foreach (var pictogram in activityDTO.Pictograms)
                    {
                        var picto = await _pictogramRepository.GetPictogramsById(pictogram.Id);

                        if (picto != null)
                        {
                            pictograms.Add(picto);
                        }
                        else
                        {
                            return false;
                        }
                    }

                    Timer timer = null;
                    if (activityDTO.Timer != null)
                    {
                        timer = await _timerRepository.getTimerWithKey(activityDTO.Timer.Key);
                    }

                    if (pictograms.Any())
                        to.Activities.Add(new Activity(to, pictograms, activityDTO.Order, activityDTO.State, timer, activityDTO.IsChoiceBoard, activityDTO.Title, activityDTO.ChoiceBoardName));
                }
            }
            return true;
        }
    }
}
