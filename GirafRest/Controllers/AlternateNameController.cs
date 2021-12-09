using GirafRest.Models;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Http;
using GirafRest.IRepositories;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Controller for managing <see cref="AlternateName"/>
    /// </summary>
    [Authorize]
    [Route("v2/[Controller]")]
    public class AlternateNameController : Controller
    {
        private readonly IGirafService _giraf;
        private readonly IAuthenticationService _authentication;
        private readonly IPictogramRepository _pictogramRepository;
        private readonly IGirafUserRepository _girafUserRepository;
        private readonly IAlternateNameRepository _alternateNameRepository;

        /// <summary>
        /// Constructor for controller
        /// </summary>
        /// <param name="girafService">Service Injection</param>
        /// <param name="lFactory">Service Injection</param>
        /// <param name="pictogramRepository">The <see cref="IPictogramRepository"/> used to query Pictograms</param>
        /// <param name="girafUserRepository">The <see cref="IGirafUserRepository"/> used to query Users</param>
        /// <param name="alternateNameRepository">The <see cref="IAlternateNameRepository"/> used to query alternate names</param>
        public AlternateNameController(
            IGirafService girafService,
            ILoggerFactory lFactory,
            IPictogramRepository pictogramRepository,
            IGirafUserRepository girafUserRepository,
            IAlternateNameRepository alternateNameRepository
        ) {
            _giraf = girafService;
            _pictogramRepository = pictogramRepository;
            _girafUserRepository = girafUserRepository;
            _alternateNameRepository = alternateNameRepository;
        }


        /// <summary>
        /// Get AlternateName for specified <see cref="GirafUser"/> and <see cref="Pictogram"/>
        /// </summary>
        /// <param name="userId">The id of the related user</param>
        /// <param name="picId">The id of the related pictogram</param>
        /// <returns></returns>
        [HttpGet("{userId}/{picId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType((StatusCodes.Status200OK))]
        public ActionResult Get(string userId, long picId) {
            GirafUser user = _girafUserRepository.GetByID(userId);
            if (user == default) {
                return this.ResourceNotFound(nameof(GirafUser), userId);
            }

            Pictogram pictogram = _pictogramRepository.GetByID(picId);
            if (pictogram == default) {
                return this.ResourceNotFound(nameof(Pictogram), picId);
            }

            AlternateName alternateName = _alternateNameRepository.GetForUser(user.Id, pictogram.Id);
            if (alternateName == default) {
                return this.ResourceNotFound(nameof(AlternateName), user.Id, pictogram.Id);
            }

            return this.RequestSucceeded(new AlternateNameDTO(alternateName));
        }

        /// <summary>
        /// Create a new AlternateName from AlternateNameDTO
        /// </summary>
        /// <param name="alternateName">The <see cref="AlternateName"/> to be created</param>
        /// <returns>StatusCode containing relevant error or success with the created <see cref="AlternateName"/></returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Create([FromBody] AlternateNameDTO alternateName) {
            if (alternateName == default) {
                return this.MissingObjectFromBody(nameof(AlternateName));
            }

            if (string.IsNullOrEmpty(alternateName.Citizen)) {
                return this.MissingPropertyFromRequest(nameof(alternateName.Citizen));
            }

            if (string.IsNullOrEmpty(alternateName.Name)) {
                return this.MissingPropertyFromRequest(nameof(alternateName.Name));
            }

            GirafUser user = _girafUserRepository.GetByID(alternateName.Citizen);
            if (user == null) {
                return this.ResourceNotFound(nameof(GirafUser), alternateName.Name);
            }

            Pictogram pictogram = _pictogramRepository.GetByID(alternateName.Pictogram);
            if (pictogram == null) {
                return this.ResourceNotFound(nameof(Pictogram), alternateName.Pictogram);
            }

            if (_alternateNameRepository.UserAlreadyHas(user.Id, pictogram.Id)) {
                return this.ResourceConflict(nameof(AlternateName), user.Id, pictogram.Id);
            }


            AlternateName newAlternateName = new AlternateName(user, pictogram, alternateName.Name);
            _alternateNameRepository.Add(newAlternateName);
            _alternateNameRepository.Save();

            return this.ResourceCreated(new AlternateNameDTO(newAlternateName));
        }

        /// <summary>
        /// Edit an AlternateName
        /// </summary>
        /// <param name="id">The id of the AlternateName to edit</param>
        /// <param name="newAlternateName">The AlternateNameDTO containing changes</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Edit(long id, [FromBody] AlternateNameDTO newAlternateName) {
            if (newAlternateName == default) {
                return this.MissingObjectFromBody(nameof(AlternateNameDTO));
            }

            if (string.IsNullOrEmpty(newAlternateName.Name)) {
                return this.MissingPropertyFromRequest(nameof(newAlternateName.Name));
            }

            if (string.IsNullOrEmpty(newAlternateName.Citizen)) {
                return this.MissingPropertyFromRequest(nameof(newAlternateName.Citizen));
            }

            GirafUser user = _girafUserRepository.Get(newAlternateName.Citizen);
            if (user == default) {
                return this.ResourceNotFound(nameof(GirafUser), newAlternateName.Citizen);
            }

            AlternateName oldAlternateName = _alternateNameRepository.Get(id);
            if (oldAlternateName == default) {
                return this.ResourceNotFound(nameof(AlternateName), id);
            }

            Pictogram pictogram = _pictogramRepository.GetByID(newAlternateName.Pictogram);
            if (pictogram == default) {
                return this.ResourceNotFound(nameof(Pictogram), newAlternateName.Pictogram);
            }

            // This is a weird artefact caused by the same DTOs being used for in/out going requests
            if (oldAlternateName.PictogramId != pictogram.Id ||
                oldAlternateName.CitizenId != user.Id) {
                return this.InvalidPropertiesFromRequest(nameof(oldAlternateName.PictogramId), nameof(oldAlternateName.CitizenId));
            }

            oldAlternateName.Name = newAlternateName.Name;
            _alternateNameRepository.Update(oldAlternateName);
            _alternateNameRepository.Save();

            return this.RequestSucceeded(newAlternateName);
        }
    }
}