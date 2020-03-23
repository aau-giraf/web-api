using GirafRest.Models;
using System.Collections.Generic;

namespace GirafRest.Setup
{
    public class SampleWeekday
    {
        public Days Day;
        public List<string> ActivityIconTitles;
        public List<string> ActivityStates;

        public SampleWeekday(Days day, List<string> actIconTitles, List<string> actStates)
        {
            Day = day;
            ActivityIconTitles = actIconTitles;
            ActivityStates = actStates;
        }
    }
}