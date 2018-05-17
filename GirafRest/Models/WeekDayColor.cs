using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models;

namespace GirafRest
{
    /// <summary>
    /// The entity for enabling a day to have a specific Color
    /// </summary>
    public class WeekDayColor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string HexColor { get; set; }

        public Days Day { get; set; }

        [ForeignKey("Setting")]
        public long SettingId { get; set; }

        public Setting Setting { get; set; }

        /// <summary>
        /// DO NOT DELETE
        /// </summary>
        public WeekDayColor()
        {
            
        }
    }
}
