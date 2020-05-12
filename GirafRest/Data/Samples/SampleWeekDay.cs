using GirafRest.Models;
using System.Collections.Generic;

namespace GirafRest.Setup
{
    ///
    public class SampleWeekday
    {
        ///
        public Days Day { get; set; }
        ///
        public List<string> ActivityIconTitles { get; }
        ///
        public List<string> ActivityStates { get; }
        ///
        public SampleWeekday(Days day, List<string> actIconTitles, List<string> actStates)
        {
            Day = day;
            ActivityIconTitles = actIconTitles;
            ActivityStates = actStates;
        }
    }
}