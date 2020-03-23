namespace GirafRest.Setup
{
    public class SampleWeekTemplate
    {
        public string Name { get; set; }
        public long PictKey { get; set; }
        public long DepartmentKey { get; set; }

        public SampleWeekTemplate(string name, long pictKey, long departmentKey)
        {
            Name = name;
            PictKey = pictKey;
            DepartmentKey = departmentKey;
        }
    }  
}