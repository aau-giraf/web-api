using GirafRest.IntegrationTest.Extensions;
using System;

namespace GirafRest.IntegrationTest.Setup
{
    public class UserFixture : IDisposable
    {
        public string GuardianUsername;
        public string GuardianDisplayName;
        public string NewGuardianUsername;
        public string Citizen1Username;
        public string Citizen2Username;
        public string Citizen3Username;
        public string Password;

        public string GrayscaleTheme;
        public string TimerOneHour;
        public string MultipleSettings;
        public string[] NewSetting = {"{'orientation': 1, 'completeMark': 2, 'cancelMark': 1, 'defaultTimer': 2, 'theme': 1}",
                            "{ 'orientation': 1, 'completeMark': 2, 'cancelMark': 1, 'defaultTimer': 2, 'theme': 2}"};
        public string RawImage;
        public string PictogramTitle;
        
        public UserFixture()
        {
            GuardianUsername = "Guardian-dev";
            GuardianDisplayName = "dev-guardian";
            NewGuardianUsername = $"Testguardian{DateTime.Now.Ticks}";
            Citizen1Username = "Citizen-dev";
            Citizen2Username = $"Gunnar{DateTime.Now.Ticks}";
            Citizen3Username = $"Charlie{DateTime.Now.Ticks}";
            Password = "password";

            GrayscaleTheme = $@"{{'orientation': 1, 'completeMark': 2, 'cancelMark': 2, 'defaultTimer': 2,
                               'timerSeconds': 900, 'activitiesCount': '{null}', 'theme': 2, 'nrOfDaysToDisplay': 7,
                               'greyScale': '{true}', 'lockTimerControl': '{true}', 'pictogramText': '{true}', 'showPopup': '{true}', 'weekDayColors':
                                   [{{ 'hexColor': '#08a045', 'day': 1}}, {{ 'hexColor': '#540d6e', 'day': 2}},
                                    {{ 'hexColor': '#f77f00', 'day': 3}}, {{ 'hexColor': '#004777', 'day': 4}},
                                    {{ 'hexColor': '#f9c80e', 'day': 5}}, {{ 'hexColor': '#db2b39', 'day': 6}},
                                    {{ 'hexColor': '#ffffff', 'day': 7}}]}}";
            TimerOneHour = $@"{{'orientation': 1, 'completeMark': 2, 'cancelMark': 2, 'defaultTimer': 2,
                              'timerSeconds': 3600, 'activitiesCount': '{null}', 'theme': 1, 'colorThemeWeekSchedules': 1,
                              'nrOfDaysToDisplay': 4, 'greyScale': '{true}', 'lockTimerControl': '{true}',
                              'pictogramText': '{true}', 'weekDayColors':
                                  [{{ 'hexColor': '#08A045', 'day': 1}}, {{ 'hexColor': '#540D6E', 'day': 2}},
                                   {{ 'hexColor': '#F77F00', 'day': 3}}, {{ 'hexColor': '#004777', 'day': 4}},
                                   {{ 'hexColor': '#F9C80E', 'day': 5}}, {{ 'hexColor': '#DB2B39', 'day': 6}},
                                   {{ 'hexColor': '#FFFFFF', 'day': 7}}]}}";
            MultipleSettings = $@"{{'orientation': 2, 'completeMark': 2, 'cancelMark': 1, 'defaultTimer': 2,
                                 'timerSeconds': 60, 'activitiesCount': 3, 'theme': 3, 'nrOfDaysToDisplay': 2,
                                 'greyScale': '{true}', 'lockTimerControl': '{true}', 'pictogramText': '{true}',
                                 'weekDayColors':
                                     [{{ 'hexColor': '#FF00FF', 'day': 1}}, {{ 'hexColor': '#540D6E', 'day': 2}},
                                      {{ 'hexColor': '#F77F00', 'day': 3}}, {{ 'hexColor': '#004777', 'day': 4}},
                                      {{ 'hexColor': '#F9C80E', 'day': 5}}, {{ 'hexColor': '#DB2B39', 'day': 6}},
                                      {{ 'hexColor': '#FFFFFF', 'day': 7}}]}}";
            RawImage = "ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿñÇÿÿõ×ÿÿñÇÿÿÿÿÿÿÿ" +
                        "ÿÿÿÿÿÿÿøÿÿüÿÿþ?ÿÿü?ÿÿøÿÿøÿÿà?ÿÿÿ";
            PictogramTitle = $"wednesday{DateTime.Now.Ticks}";
        }
        
        public void Dispose()
        {
            CustomWebApplicationFactory customWebApplicationFactory = new CustomWebApplicationFactory();
            
            // delete pictogram
            var pictogramId = TestExtension.GetPictogramIdAsync(customWebApplicationFactory, PictogramTitle, Citizen2Username, Password).Result;
            TestExtension.DeletePictogramAsync(customWebApplicationFactory, pictogramId, Citizen2Username, Password).Wait();
            
            // delete accounts
            TestExtension.DeleteAccountAsync(customWebApplicationFactory, Citizen2Username, Password, GuardianUsername, Password).Wait();
            TestExtension.DeleteAccountAsync(customWebApplicationFactory, Citizen3Username, Password, GuardianUsername, Password).Wait();
        }
    }
}
