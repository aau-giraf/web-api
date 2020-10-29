using GirafRest.Utilities;
using Xunit;

namespace GirafRest.Test.Utilities
{
    public class PictogramUtilTest
    {
        [Fact]
        public void Search_IdenticalStrings_0Distance()
        {
            const int expected = 0;
            const string firstString = "A string";
            const string secondString = "A string";
            
            int actual = PictogramUtil.IbsenDistance(firstString, secondString);

            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Search_StringsWithDifferentLength_DistanceEqualToDifference()
        {
            const int expected = 7;
            const string firstString = "A string";
            // secondString is 7 characters longer
            const string secondString = "A longer string";
            
            int actual = PictogramUtil.IbsenDistance(firstString, secondString);

            Assert.Equal(expected, actual);
        }
        
    }
}