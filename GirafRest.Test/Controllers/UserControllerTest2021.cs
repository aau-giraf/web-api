
using Xunit;
using GirafRest.Models.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace GirafRest.Test.Controllers
{
    [TestClass]
    public class UserControllerTest2021
    {
        readonly GirafRoles citizen = GirafRoles.Citizen;
       
        [Fact]
        public void CompareValueAfterCurrent()
        {
            //Arrange 

            DisplayNameDTO Kenneth = new DisplayNameDTO("Kenneth", citizen, "1101");
            DisplayNameDTO Louise = new DisplayNameDTO("Louise", citizen, "1101");
            int expected = -1;
            //Act
            int actual = Kenneth.CompareTo(Louise);
            //Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual);
        }

        [Fact]
        public void CompareValueBeforeCurrent()
        {
            //Arrange 

            DisplayNameDTO Kenneth = new DisplayNameDTO("Kenneth", citizen, "1101");
            DisplayNameDTO Louise = new DisplayNameDTO("Louise", citizen, "1101");
            int expected = 1;
            //Act
            int actual = Louise.CompareTo(Kenneth);
            //Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual);
        }
        [Fact]
        public void CompareValueSameAsCurrent()
        {
            //Arrange 

            DisplayNameDTO Kenneth = new DisplayNameDTO("Louise", citizen, "1101");
            DisplayNameDTO Louise = new DisplayNameDTO("Louise", citizen, "1101");
            int expected = 0;
            //Act
            int actual = Louise.CompareTo(Kenneth);
            //Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual);
        }
    }

}
