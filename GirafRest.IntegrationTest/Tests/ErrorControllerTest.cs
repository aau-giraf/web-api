﻿using GirafRest.IntegrationTest.Extensions;
using GirafRest.IntegrationTest.Order;
using GirafRest.IntegrationTest.Setup;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GirafRest.IntegrationTest.Tests
{
    public class ErrorControllerTest : IClassFixture<CustomWebApplicationFactory>, IClassFixture<ErrorFixture>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly ErrorFixture _errorFixture;
        private const string BASE_URL = "https://localhost:5000/";
        private readonly HttpClient client;
        public ErrorControllerTest(CustomWebApplicationFactory factory, ErrorFixture errorFixture)
        {
            _factory = factory;
            client = _factory.CreateClient();
            _errorFixture = errorFixture;
        }

        /// <summary>
        ///Testing GET error
        ///Endpoint: GET:/v1/Error
        /// </summary>
        [Fact, Priority(0)]
        public async void TestERRORCanGetError()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error"),
                Method = HttpMethod.Get
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("UnknownError", content["errorKey"]);
        }

        /// <summary>
        ///Testing PUT error
        ///Endpoint: PUT:/v1/Error
        /// </summary>
        [Fact, Priority(1)]
        public async void TestERRORCanPutError()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error"),
                Method = HttpMethod.Put
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("UnknownError", content["errorKey"]);
        }

        /// <summary>
        ///Testing POST error
        ///Endpoint: POST:/v1/Error
        /// </summary>
        [Fact, Priority(1)]
        public async void TestERRORCanPostError()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error"),
                Method = HttpMethod.Post
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("UnknownError", content["errorKey"]);
        }


        /// <summary>
        ///Testing DELETE error
        ///Endpoint: DELETE:/v1/Error
        /// </summary>
        [Fact, Priority(2)]
        public async void TestERRORCanDELETEError()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error"),
                Method = HttpMethod.Delete
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("UnknownError", content["errorKey"]);
        }


        /// <summary>
        ///Doing dumb shit here.This test should get redirected to the Error endpoint.
        ///Endpoint: DELETE:/v1/Error
        /// </summary>
        [Fact, Priority(3)]
        public async void TestERRORCanDELETEHaHaError()
        {
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}hahahaha"),
                Method = HttpMethod.Delete
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("NotFound", content["errorKey"]);
        }

        /// <summary>
        /// Testing statuscode 401.
        /// Endpoint: DELETE:/v1/Error
        /// </summary>

        [Fact, Priority(4)]
        public async void TestStatusCode401()
        {

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error?statusCode=401"),
                Method = HttpMethod.Delete,
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("NotAuthorized", content["errorKey"]);
        }

        /// <summary>
        /// Testing statuscode 403.
        /// Endpoint: DELETE:/v1/Error
        /// </summary>

        [Fact, Priority(5)]
        public async void TestStatusCode403()
        {

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error?statusCode=403"),
                Method = HttpMethod.Delete,
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("Forbidden", content["errorKey"]);
        }

        /// <summary>
        /// Testing statuscode 404.
        /// Endpoint: DELETE:/v1/Error
        /// </summary>

        [Fact, Priority(5)]
        public async void TestStatusCode404()
        {

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error?statusCode=404"),
                Method = HttpMethod.Delete,
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("NotFound", content["errorKey"]);
        }


        /// <summary>
        ///Testing whether the endpoint can brew coffee.
        ///(Testing the fallback in the error controller)
        ///Endpoint: DELETE:/v1/Error
        /// </summary>

        [Fact, Priority(5)]
        public async void TestStatusCodeImATeapot()
        {

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{BASE_URL}v1/Error?statusCode=418"),
                Method = HttpMethod.Delete,
            };

            var response = await client.SendAsync(request);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status418ImATeapot.ToString(), response.StatusCode.ToString());
            Assert.Equal("UnknownError", content["errorKey"]);
        }

        /*
    
    @order
    def test_error_lul(self):
        """
        Doing dumb shit here. This test should get redirected to the Error endpoint.

        Endpoint: DELETE:/v1/Error
        """
        response = delete(f'{BASE_URL}hahahaha/')
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)
        self.assertEqual(response_body['errorKey'], 'NotFound')

    @order
    def test_status_code_401(self):
        """
        Testing statuscode 401.

        Endpoint: DELETE:/v1/Error
        """
        params = {'statusCode': 401}
        response = get(f'{BASE_URL}v1/Error', params=params)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.UNAUTHORIZED)
        self.assertEqual(response_body['errorKey'], 'NotAuthorized')

    @order
    def test_status_code_403(self):
        """
        Testing statuscode 403.

        Endpoint: DELETE:/v1/Error
        """
        params = {'statusCode': 403}
        response = get(f'{BASE_URL}v1/Error', params=params)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.FORBIDDEN)
        self.assertEqual(response_body['errorKey'], 'Forbidden')

    @order
    def test_status_code_404(self):
        """
        Testing statuscode 404.

        Endpoint: DELETE:/v1/Error
        """
        params = {'statusCode': 404}
        response = get(f'{BASE_URL}v1/Error', params=params)
        response_body = response.json()
        self.assertEqual(response.status_code, HTTPStatus.NOT_FOUND)
        self.assertEqual(response_body['errorKey'], 'NotFound')

    @order
    def test_status_code_im_a_teapot(self):
        """
        Testing whether the endpoint can brew coffee.
        (Testing the fallback in the error controller)

        Endpoint: DELETE:/v1/Error
        """
        params = {'statusCode': 418}
        response = get(f'{BASE_URL}v1/Error', params=params)
        response_body = response.json()
        self.assertEqual(response.status_code, 418)
        self.assertEqual(response_body['errorKey'], 'UnknownError')

         * 
         */
    }
}
