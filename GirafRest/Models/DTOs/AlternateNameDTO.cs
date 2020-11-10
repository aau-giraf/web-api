using System.Diagnostics;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for <see cref="AlternateName"/>
    /// </summary>
    public class AlternateNameDTO
    {
        /// <summary>
        /// Citizen
        /// </summary>
        public string Citizen { get; set; }
        /// <summary>
        /// pictogram
        /// </summary>
        public long Pictogram { get; set; }
        /// <summary>
        /// AlternateName
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Empty constructor for JSON reasons?
        /// </summary>
        public AlternateNameDTO()
        {
            
        }
        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="an">The AlternateName object to create the DTO from</param>
        public AlternateNameDTO(AlternateName an)
        {
            if (an == null)
            {
                return;
            }
            Citizen = an.CitizenId;
            Pictogram = an.PictogramId;
            Name = an.Name;
        }
    }
}