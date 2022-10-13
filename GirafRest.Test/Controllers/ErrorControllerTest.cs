using Xunit;
using GirafRest.Controllers;
using Moq;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using GirafRest.Models.Responses;
using System;


namespace GirafRest.Test.Controllers
{
    
    public class ErrorControllerTest
    {
        [Fact]
       public void Unauthorized401_Test()
       {
            // Arrange
            int status = StatusCodes.Status401Unauthorized;
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(f => f.Get<IStatusCodeReExecuteFeature>())
                .Returns
                (
                    new StatusCodeReExecuteFeature()
                    {
                        OriginalPath = "Path",
                        OriginalPathBase = "Base",
                        OriginalQueryString = "String"
                    }
                );

            var httpContext = new DefaultHttpContext(mockFeatureCollection.Object);
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            
            var controller = new ErrorController()
            {
                ControllerContext = controllerContext
            };


            // Act
            var response = controller.StatusCodeEndpoint(status);
            var result = response as UnauthorizedObjectResult;
            var StatusCodeResult = result.StatusCode;

            // Assert
            Assert.Equal(response is UnauthorizedObjectResult, true);
            Assert.Equal(StatusCodeResult, status);
       }
       //Copy the method above until all Error handling actions are testable.

        [Fact]
       public void Forbidden403_Test()
       {
            // Arrange
            int status = StatusCodes.Status403Forbidden;
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(f => f.Get<IStatusCodeReExecuteFeature>())
                .Returns
                (
                    new StatusCodeReExecuteFeature()
                    {
                        OriginalPath = "Hej",
                        OriginalPathBase = "Base",
                        OriginalQueryString = "Query"
                    }
                );

            var httpContext = new DefaultHttpContext(mockFeatureCollection.Object);
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            
            var controller = new ErrorController()
            {
                ControllerContext = controllerContext
            };

            // Act
            var response = controller.StatusCodeEndpoint(status);
            var statusCodeResult = response.GetType().GetProperty("StatusCode").GetValue(response, null);

            // Assert
            Assert.Equal(statusCodeResult, status);
        
       }

        [Fact]
       public void NotFound404_Test() //FAILED test
       {
            // Arrange
            
            var status = ((int)ErrorCode.NotFound);
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(f => f.Get<IStatusCodeReExecuteFeature>())
                .Returns
                (
                    new StatusCodeReExecuteFeature()
                    {
                        OriginalPath = "",
                        OriginalPathBase = "",
                        OriginalQueryString = ""
                    }
                );

            var httpContext = new DefaultHttpContext(mockFeatureCollection.Object);
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            
            var controller = new ErrorController()
            {
                ControllerContext = controllerContext
            };


            // Act
            var response = controller.StatusCodeEndpoint(status);
            var result = response as NotFoundObjectResult;
            // var result = JObject.Parse(response.ToString());
            var statusCodeResult = result.StatusCode;

            // var response = controller.StatusCodeEndpoint(status);
            // var statusCodeResult = response.GetType().GetProperty("StatusCode").GetValue(response, null);
            // var message = Assert.Throws<InvalidOperationException>(() => controller.NotFound());

            // Assert

            Assert.Equal(status, statusCodeResult);
            // Assert.Equal("Not found", message.Message);
            // Assert.Equal("UnknownError", result["errorKey"]);
            // Assert.Null(result);
            
            
       }

        [Fact]
       public void BadRequest400_Test()
       {
            // Arrange
            int status = 0;
            int actual = StatusCodes.Status400BadRequest;
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(f => f.Get<IStatusCodeReExecuteFeature>())
                .Returns
                (
                    new StatusCodeReExecuteFeature()
                    {
                        OriginalPath = "Path",
                        OriginalPathBase = "Base",
                        OriginalQueryString = "String"
                    }
                );

            var httpContext = new DefaultHttpContext(mockFeatureCollection.Object);
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            
            var controller = new ErrorController()
            {
                ControllerContext = controllerContext
            };


            // Act
            var response = controller.StatusCodeEndpoint(status);
            var result = response as BadRequestObjectResult;
            var statusCodeResult = result.StatusCode;

            // Assert
            // Assert.Equal(response is NotFoundObjectResult, true);
            Assert.Equal(statusCodeResult, actual);
       }

         [Fact]
       public void UnknownError_Test()
       {
            // Arrange
            int status = 300;
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(f => f.Get<IStatusCodeReExecuteFeature>())
                .Returns
                (
                    new StatusCodeReExecuteFeature()
                    {
                        OriginalPath = "Hej",
                        OriginalPathBase = "Base",
                        OriginalQueryString = "Query"
                    }
                );

            var httpContext = new DefaultHttpContext(mockFeatureCollection.Object);
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            
            var controller = new ErrorController()
            {
                ControllerContext = controllerContext
            };

            // Act
            var response = controller.StatusCodeEndpoint(status);
            var statusCodeResult = response.GetType().GetProperty("StatusCode").GetValue(response, null);

            // Assert
            Assert.Equal(statusCodeResult, status);
        
       }
    }
}