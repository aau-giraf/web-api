using System;
using System.Collections.Generic;
using GirafRest.Controllers;
using System.Text;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GirafRest.Extensions;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.InMemory;
using GirafRest.Data;
using GirafRest.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GirafRest.Test.Controllers
{
    [TestClass]
    public class UserControllerTest2021
    {
        readonly GirafRoles citizen = GirafRoles.Citizen;

        [TestMethod]
        public void GetSortedCitizens()
        {
            //Arrange 
            List<DisplayNameDTO> unsorted_test_list = new List<DisplayNameDTO>();
            List<DisplayNameDTO> sorted_test_list = new List<DisplayNameDTO>();
            DisplayNameDTO Kenneth = new DisplayNameDTO("Kenneth", citizen, "1101");
            DisplayNameDTO Louise = new DisplayNameDTO("Louise", citizen, "1101");
            DisplayNameDTO Josefine = new DisplayNameDTO("Josefine", citizen, "1101");
            DisplayNameDTO Søren = new DisplayNameDTO("Søren", citizen, "1101");
            DisplayNameDTO Fatima = new DisplayNameDTO("Fatima", citizen, "1101");
            DisplayNameDTO Christoffer = new DisplayNameDTO("Christoffer", citizen, "1101");
            //add users to list unsorted
            unsorted_test_list.Add(Josefine);
            unsorted_test_list.Add(Kenneth);
            unsorted_test_list.Add(Louise);
            unsorted_test_list.Add(Søren);
            unsorted_test_list.Add(Fatima);
            unsorted_test_list.Add(Christoffer);
            //Act
            var sorted = unsorted_test_list.OrderBy(person => person.DisplayName); //Make an sorted version of the unsorted list
             unsorted_test_list.Sort(); //sort the original list
            
            //Assert
            CollectionAssert.AreEqual(sorted.ToList(), unsorted_test_list.ToList()); //check is they are equal in order
        }

    }

}
