using System;
using System.Collections.Generic;

namespace GirafRest.Models
{
    public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };
    public class Weekday
    {
        public bool ElementsSet { get; set; }
        public Days Day { get; set; }
        public DateTime lastEdit { get; set; }
        public ICollection<Frame> Elements { get; set; }
        public Weekday()
        {
            Elements = new List<Frame>();
        }
    }
}