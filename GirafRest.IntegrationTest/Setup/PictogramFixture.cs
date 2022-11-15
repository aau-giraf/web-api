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
        public string Pictogram;
        public string PictogramTitle;
        public string PictogramRename;
        public string RawImage;
        public string Citizen1Username;
        public string Password;

        public PictogramFixture()
        {
            LAST_EDIT_TIMESTAMP = "2020-03-26T09:22:28.252106Z";
            Pictograms = @$"
            {{
                'data': [
                    {{'id': 1, 'lastEdit': '{LAST_EDIT_TIMESTAMP}', 'title': 'Epik', 'accessLevel': 1, 'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/1/image/raw'}},
                    {{'id': 2, 'lastEdit': '{LAST_EDIT_TIMESTAMP}', 'title': 'som', 'accessLevel': 1, 'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/2/image/raw'}},
                    {{'id': 5, 'lastEdit': '{LAST_EDIT_TIMESTAMP}', 'title': 'simpelt', 'accessLevel': 1,'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/5/image/raw'}},
                    {{'id': 8, 'lastEdit': '{LAST_EDIT_TIMESTAMP}', 'title': 'sejt', 'accessLevel': 1,'imageHash': 'secure hash', 'imageUrl': '/v1/pictogram/8/image/raw'}}
                ]
            }}";
            Citizen1Username = "Citizen-dev";
            Password = "password";

            ///Information about a new pictogram to be added in the integration PictogramControllerTest 
            PictogramTitle = $"New pictogram created: {DateTime.Now.Ticks}";
            Pictogram = $"{{ 'accessLevel': 0, 'title': '{PictogramTitle}', 'id': -1, 'lastEdit': '2099-03-19T10:40:26.587Z'}}";
            PictogramRename = $"Pictogram is now renamed: {PictogramTitle}";
            RawImage = "Some raw image"; 
        }
    }
}
