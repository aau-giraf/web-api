using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IntegrationTest.Setup
{
    public class PictogramFixture
    {
        public string LAST_EDIT_TIMESTAMP;

        public string Pictograms;
        public string Fish;
        public string FishName;
        public string FishRename;
        public string RawImage;
        public string Citizen1Username;
        public string Password;

        public PictogramFixture()
        {
            LAST_EDIT_TIMESTAMP = "2020-03-26T09:22:28.252106Z";
            Pictograms = @$"
            {{
                'data': [
                    {{'id': 1, 'lastEdit': '{LAST_EDIT_TIMESTAMP}', 'title': 'som', 'accessLevel': 1, 'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/1/image/raw'}},
                    {{'id': 2, 'lastEdit': '{LAST_EDIT_TIMESTAMP}', 'title': 'alfabet', 'accessLevel': 1, 'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/2/image/raw'}},
                    {{'id': 5, 'lastEdit': '{LAST_EDIT_TIMESTAMP}', 'title': 'antal', 'accessLevel': 1,'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/5/image/raw'}},
                    {{'id': 8, 'lastEdit': '{LAST_EDIT_TIMESTAMP}', 'title': 'delmængde', 'accessLevel': 1,'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/8/image/raw'}}
                ]
            }}";
            FishName = $"fish{DateTime.Now.Ticks}";
            Fish = $"{{ 'accessLevel': 0, 'title': '{FishName}', 'id': -1, 'lastEdit': '2099-03-19T10:40:26.587Z'}}";
            FishRename = $"cursed_{FishName}";
            RawImage = "ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿñÇÿÿõ×ÿÿñÇÿÿÿÿÿÿÿÿÿÿÿÿÿÿøÿÿüÿÿþ?ÿÿü?ÿÿøÿÿøÿÿà?ÿÿÿ";
            Citizen1Username = "Citizen-dev";
            Password = "password";
        }
    }
}
