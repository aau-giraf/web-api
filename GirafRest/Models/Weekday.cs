using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models
{
    public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };
    public class Weekday
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime LastEdit { get; set; }
        public long ThumbnailKey { get; set; }
        public Pictogram Thumbnail { get; set; }
        public bool ElementsSet { get; set; }
        public Days Day { get; set; }
        public ICollection<WeekdayResource> Elements { get; set; }
        public Weekday()
        {
            this.Elements = new List<WeekdayResource>();
        }
        public Weekday(Days day, Pictogram thumbnail, ICollection<Frame> elements)
        {
            this.Day = day;
            this.Thumbnail = thumbnail;
            this.ThumbnailKey = thumbnail.Id;
            this.Elements = new List<WeekdayResource>();
            foreach(var elem in elements) {
                this.Elements.Add(new WeekdayResource(this, elem));
            }
        }
    }
}