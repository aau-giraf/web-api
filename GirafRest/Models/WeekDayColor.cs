using GirafRest.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest
{
    /// <summary>
    /// The entity for enabling a day to have a specific Color
    /// </summary>
    public class WeekDayColor
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Actual color, at the format of #AABBCC for RGB HEX, #000000 to #FFFFFF
        /// </summary>
        public string HexColor { get; set; }

        /// <summary>
        /// Actual day
        /// </summary>
        public Days Day { get; set; }

        /// <summary>
        /// Belonging setting id
        /// </summary>
        [ForeignKey("Setting")]
        public long SettingId { get; set; }

        /// <summary>
        /// Belonging setting
        /// </summary>
        public Setting Setting { get; set; }

        /// <summary>
        /// DO NOT DELETE
        /// </summary>
        public WeekDayColor()
        {

        }
    }
}
