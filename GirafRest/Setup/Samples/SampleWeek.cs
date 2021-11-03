namespace GirafRest.Setup
{
    public class SampleWeek
    {
        public string Name { get; set; }
        public string ImageTitle { get; set; }

        public SampleWeek(string name, string imageTitle)
        {
            Name = name;
            ImageTitle = imageTitle;
        }
    }
}