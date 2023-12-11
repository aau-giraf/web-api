using Giraf.IntegrationTest.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Giraf.IntegrationTest.Tests
{
    [TestCaseOrderer("Giraf.IntegrationTest.Order.PriorityOrderer", "Giraf.IntegrationTest")]
    [Collection("Integration test")]
    public class ActivityControllerTest
    {
        /*
         ActivityControllerTest unimplemented; requires fix!

            1. Testing creation of user specific activity

               Endpoint: POST:/v2/Activity/{userId}/{weekplanName}/{weekYear}/{weekNumber}/{weekDayNmb}

            2. Testing PUT update to activity for a specific user

               Endpoint: PUT:/v2/Activity/{userId}/update

            3. Testing DELETE on user specific activity
        
               Endpoint: DELETE:/v2/Activity/{userId}/delete/{activityId}
            
            4. Testing GET users activity from an activityId

               Endpoint: GET:/v2/Activity/{userId}/{activityId}

            5. Testing PUT on update timer with userId

               Endpoint: PUT:/v2/Activity/{userId}/updatetimer
    */
    }
}
