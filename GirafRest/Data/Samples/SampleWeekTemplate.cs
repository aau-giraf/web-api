namespace GirafRest.Setup
{
    public class SampleWeekTemplate
    {
        public string Name { get; set; }
        public string ImageTitle { get; set; }
        public string DepartmentName { get; set; }

        public SampleWeekTemplate(string name, string imageTitle, string departmentName)
        {
            Name = name;
            ImageTitle = imageTitle;
            DepartmentName = departmentName;
        }
    }  
}