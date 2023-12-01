using GirafRest.IntegrationTest.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GirafRest.IntegrationTest.Tests
{
    [TestCaseOrderer("GirafRest.IntegrationTest.Order.PriorityOrderer", "GirafRest.IntegrationTest")]
    [Collection("Integration test")]
    public class ActivityControllerTest
    {
        // The below is taken from the python integration tests and havent been implemented since they are skipped
        /*
        @order
        @skip("Skipping since endpoint is broken")
        def test_activity_set_new_user_activity(self):
            """
            Testing creation of user specific activity

            Endpoint: POST:/v2/Activity/{user_id}/{weekplan_name}/{week_year}/{week_number}/{week_day_number}
            """
            global activity_id
            data = {"pictogram": {"id": 1}}
            response = post(f'{BASE_URL}v2/Activity/{user_id}/{self.weekplan_name}/{self.week_year}/{self.week_number}/{self.week_day_number}', headers=auth(guardian_token), json=data,)
            response_body = response.json()

            self.assertEqual(response.status_code, HTTPStatus.CREATED)
            self.assertIsNotNone(response_body['data'])
            self.assertIsNotNone(response_body['data']['id'])
            activity_id = response_body['data']['id']


        @order
        @skip("Skipping since test is broken")
        def test_activity_update_user_activity(self):
            """
            Testing PATCH update to activity for a specific user

            Endpoint: PATCH:/v2/Activity/{user_id}/update
            """

            data = {'pictogram': {'id': 6}, 'id': activity_id}
            response = patch(f'{BASE_URL}v2/Activity/{user_id}/update', json=data,
                             headers=auth(guardian_token))

            self.assertEqual(response.status_code, HTTPStatus.OK)

        @order
        @skip("Skipping since test is broken")
        def test_activity_delete_user_activity(self):
            """
            Testing DELETE on user specific activity

            Endpoint: DELETE:/v2/Activity/{user_id}/delete/{activity_id}
            """

            response = delete(f'{BASE_URL}v2/Activity/{user_id}/delete/{activity_id}', headers=auth(guardian_token))

            self.assertEqual(response.status_code, HTTPStatus.OK)
    */
    }
}
