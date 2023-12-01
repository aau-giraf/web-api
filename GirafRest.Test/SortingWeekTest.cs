using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace GirafRest.Test
{
    //This test should be moved to an apropiate location when one is defined
    public class SortingWeekTest
    {
       

        [Fact]
        public void testsorting()
        {
            WeekNameDTO Week1 = new WeekNameDTO(2000, 2, "five");
            WeekNameDTO Week2 = new WeekNameDTO(1999, 51, "six");
            WeekNameDTO Week3 = new WeekNameDTO(3000, 1000, "one");
            WeekNameDTO Week4 = new WeekNameDTO(2000, 4, "four");
            WeekNameDTO Week5 = new WeekNameDTO(2000, 40, "two");
            WeekNameDTO Week6 = new WeekNameDTO(2000, 7, "three");
            
            List<WeekNameDTO> weekList = new List<WeekNameDTO>() {Week1,Week2,Week3,Week4,Week5,Week6 };
            weekList.Sort();
            Assert.Collection(weekList,
                item => Assert.Equal("one",item.Name),
                item => Assert.Equal("two", item.Name),
                item => Assert.Equal("three", item.Name),
                item => Assert.Equal("four", item.Name),
                item => Assert.Equal("five", item.Name),
                item => Assert.Equal("six",item.Name)
                );
            

        }

    }
}
