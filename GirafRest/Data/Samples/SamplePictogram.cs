namespace GirafRest.Setup
{
    ///
    public class SamplePictogram
    {
        ///
        public string Title { get; set; }
        ///
        public string AccessLevel { get; set; }
        ///
        public string ImageHash { get; set; }
        ///
        public SamplePictogram(string name, string accessLevel, string imageHash)
        {
            Title = name;
            AccessLevel = accessLevel;
            ImageHash = imageHash;
        }
    }
}