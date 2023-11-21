using GirafRest.Models;
using System.Collections.Generic;

namespace GirafEntities.Samples
{
    public class SampleWeekday
    {
        public Days Day { get; set; }
        public List<string> ActivityIconTitles { get; set; }
        public List<string> ActivityStates { get; set; }

        public SampleWeekday(Days day, List<string> actIconTitles, List<string> actStates)
        {
            Day = day;
            ActivityIconTitles = actIconTitles;
            ActivityStates = actStates;
        }
    }
}