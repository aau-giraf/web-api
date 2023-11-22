using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafEntities.Samples
{
    public class SampleData
    {
        public List<SampleGirafUser> UserList { get; set; }
        public List<SampleDepartment> DepartmentList { get; set; }
        public List<SampleWeekday> WeekdayList { get; set; }
        public List<SampleWeek> WeekList { get; set; }
        public List<SampleWeekTemplate> WeekTemplateList { get; set; }

        public SampleData()
        {
            UserList = new List<SampleGirafUser>();
            DepartmentList = new List<SampleDepartment>();
            WeekdayList = new List<SampleWeekday>();
            WeekList = new List<SampleWeek>();
            WeekTemplateList = new List<SampleWeekTemplate>();
        }
    }
}
