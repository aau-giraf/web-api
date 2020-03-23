namespace GirafRest.Setup
{
    public class SamplePictogram
    {
        public string Title { get; set; }
        public string AccessLevel { get; set; }
        public string Hash { get; set; }

        public SamplePictogram(string name, string accessLevel, string hash)
        {
            Title = name;
            AccessLevel = accessLevel;
            Hash = hash;
        }
    }
}