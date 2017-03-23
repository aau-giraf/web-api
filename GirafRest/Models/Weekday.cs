using System;
using System.Collections.Generic;

namespace GirafRest.Models
{
    public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };
    public class Weekday
    {
        public Days Day { get; set; }
        public DateTime lastEdit { get; set; }
        public ICollection<PictoFrame> Pictograms { get; set; }
        public Weekday()
        {
            Pictograms = new List<PictoFrame>();
        }
    }
}