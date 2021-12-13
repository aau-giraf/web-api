using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IntegrationTest.Setup
{
    public class WeekTemplateFixture
    {
        public string[] Templates = {$@"{{'thumbnail': {{'id': 28}}, 'name': 'Template1', 'days':
                         [{{'day': 'Monday', 'activities': [{{'pictograms': [{{'id': 1}}], 'order': 0, 'state': 'Active'}},
                                                           {{ 'pictograms': [{{ 'id': 6}}], 'order': 0, 'state': 'Active'}}]}},
                         {{
    'day': 'Friday', 'activities': [{{ 'pictograms': [{{ 'id': 2}}], 'order': 0, 'state': 'Active'}},
                                                          {{ 'pictograms': [{{ 'id': 7}}], 'order': 0, 'state': 'Active'}}]}}]}}",
                         $@"{{
    'thumbnail': {{ 'id': 29}}, 'name': 'Template2', 'days':
                         [{{
        'day': 'Monday', 'activities': [{{ 'pictograms': [{{ 'id': 2}}], 'order': 1, 'state': 'Active'}},
                                                           {{ 'pictograms': [{{ 'id': 7}}], 'order': 2, 'state': 'Active'}}]}},
                          {{
        'day': 'Friday', 'activities': [{{ 'pictograms': [{{ 'id': 3}}], 'order': 1, 'state': 'Active'}},
                                                           {{ 'pictograms': [{{ 'id': 8}}], 'order': 2, 'state': 'Active'}}]}}]}}"};
        public string[] templateName = { "Template1", "Template2" };
        public string GuardianUsername;
        public string Password;
        public int TemplateId;
        public string Citizen1Username;
        public string Citizen2Username;

        public WeekTemplateFixture()
        {
            Citizen1Username = "Citizen-dev";
            Citizen2Username = $"Alice{DateTime.Now.Ticks}";
            GuardianUsername = "Guardian-dev";
            Password = "password";
            TemplateId = 0;
        }
    }
}
