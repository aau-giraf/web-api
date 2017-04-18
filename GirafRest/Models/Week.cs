using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GirafRest.Models.DTOs;

namespace GirafRest.Models
{
    public class Week
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; private set; }
        public ICollection<Weekday> Weekdays { get; set; }
        public Week()
        {
            this.Weekdays = new List<Weekday>();
        }
        public Week(WeekDTO weekDTO)
        {
            Merge(weekDTO);
        }
        public Week Merge(WeekDTO weekDTO)
        {
            this.Weekdays = new List<Weekday>();
            foreach (var day in weekDTO.Days)
            {
                this.Weekdays.Add(new Weekday(day));
            }
            this.Id = weekDTO.Id;
            return this;
        }
        public void InitWeek() 
        {
            if(!Weekdays.Any())
            {
                this.Weekdays.Add(new Weekday(Days.Monday, new Pictogram(), new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Tuesday, new Pictogram(), new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Wednesday, new Pictogram(), new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Thursday, new Pictogram(), new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Friday, new Pictogram(), new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Saturday, new Pictogram(), new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Sunday, new Pictogram(), new List<Frame>()));
            }
        }
    }
}