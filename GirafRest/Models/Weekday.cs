using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };
    public class Weekday : PictoFrame
    {
        public bool ElementsSet { get; set; }
        public Days Day { get; set; }
        public ICollection<WeekdayResource> Elements { get; set; }
        public Weekday() : base()
        {
            Elements = new List<WeekdayResource>();
        }
        public Weekday(string title, AccessLevel accessLevel/*, Pictogram thumbnail*/, ICollection<Frame> elements)
            : base(title, accessLevel)
        {
            this.Elements = new List<WeekdayResource>();
            foreach(var elem in elements) {
                this.Elements.Add(new WeekdayResource(this, elem));
            }
        }
    }
}