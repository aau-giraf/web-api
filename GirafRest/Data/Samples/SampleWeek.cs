namespace GirafRest.Setup
{
    public class SampleWeek
    {
        public string Name { get; set; }
        public long PictKey { get; set; }

        public SampleWeek(string name, long pictKey)
        {
            Name = name;
            PictKey = pictKey;
        }
    }
}