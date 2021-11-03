using System.Diagnostics;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for <see cref="AlternateName"/>
    /// </summary>
    public class AlternateNameDTO
    {
        /// <summary>
        /// The unique id of the alternatename
        /// </summary>
        public long Id { get; set; }
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
        /// Empty constructor of HTTP bdody serialization
        /// </summary>
        public AlternateNameDTO() { }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="alternameName">The AlternateName object to create the DTO from</param>
        public AlternateNameDTO(AlternateName alternameName)
        {
            if (alternameName == default) {
                throw new System.ArgumentException("");
            }

            Id = alternameName.Id;
            Citizen = alternameName.CitizenId;
            Pictogram = alternameName.PictogramId;
            Name = alternameName.Name;
        }
    }
}