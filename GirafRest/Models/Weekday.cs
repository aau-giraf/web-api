using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };
    public class Weekday : PictoFrame
    {
        public long ThumbnailKey { get; set; }
        [ForeignKey("ThumbnailKey")]
        public bool ElementsSet { get; set; }
        public Days Day { get; set; }
        public ICollection<Frame> _elements { get; set; }
        public Weekday()
        {
            _elements = new List<Frame>();
        }
        public Weekday(string title, AccessLevel accessLevel, Pictogram thumbnail, ICollection<Frame> elements)
            : base(title, accessLevel)
        {
            this._elements = elements;
        }
    }
}