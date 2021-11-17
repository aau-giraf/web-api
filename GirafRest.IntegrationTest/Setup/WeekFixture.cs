using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace GirafRest.IntegrationTest.Setup
{
    public class WeekFixture
    {
        public string CitizenUsername;
        public string SuperUserUsername;
        public string Password;
        public int WeekYear;
        public int Week1Number;
        public int Week2Number;
        public int Week3Number;
        public string CorrectWeek;
        public string DifferentCorrectWeek;
        public string TooManyDaysWeek;
        public string BadEnumValueWeek;

        public WeekFixture()
        {
            CitizenUsername = $"Gunnar{DateTime.Now.Ticks}";
            Password = "password";
            SuperUserUsername = "Lee";
            WeekYear = 2018;
            Week1Number = 11;
            Week2Number = 12;
            Week3Number = 13;
            CorrectWeek = Week($"[{ Enumerable.Range(1, 7).Aggregate("", (p, c) => $"{p + Day(c)},")}]");
            DifferentCorrectWeek = Week($"[{ Enumerable.Range(1, 7).Aggregate("", (p, c) => $"{p + DifferentDay(c)},")}]");
            TooManyDaysWeek = Week($"[{ Enumerable.Range(1, 7).Aggregate("", (p, c) => $"{p + Day(c)},")}, {Day(3)}]");
            BadEnumValueWeek = Week($"[{ Enumerable.Range(1, 7).Aggregate("", (p, c) => $"{p + Day(c*10)},")}]");
        }

        private string Day(int dayNumber)
        {
            return $@"{{
                'day': {dayNumber},
                'activities': [
                    {{
                        'pictograms': [
                            {{
                                'title': 'sig',
                                'id': 4,
                                'state': 1,
                                'lastEdit': '2018-03-28T10:47:51.628333',
                                'accessLevel': 0
                            }}
                        ],
                        'order': 0,
                    }}
                ]
            }}";
        }

        public string DifferentDay(int dayNumber)
        {
            return $@"{{
                'day': {dayNumber},
                'activities': [
                    {{
                        'pictograms': [
                            {{
                                'title': 'JUNK',
                                'id': 2,
                                'state': 3,
                                'lastEdit': '2017-03-28T10:47:51.628333',
                                'accessLevel': 0
                            }}
                        ],
                        'order': 0
                    }}
                ]
            }}";
        }

        private string Week(string days)
        {
            return $@"{{
                'thumbnail': {{
                    'title': 'simpelt',
                    'id': 5,
                    'lastEdit': '2018-04-20T13:17:51.033Z',
                    'accessLevel': 0
                }},
                'name': 'Coronabots roll out',
                'id': 0, 'days': {days}
            }}";
        }
    }
}