using Xunit;
using GirafRest.Controllers;
using Moq;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;


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
            mockFeatureCollection.Setup(FeatureCollection => FeatureCollection.Get<IStatusCodeReExecuteFeature>())
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

        [Fact]
       public void Forbidden403_Test()
       {
            // Arrange
            int status = StatusCodes.Status403Forbidden;
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(FeatureCollection => FeatureCollection.Get<IStatusCodeReExecuteFeature>())
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
            var statusCodeResult = response.GetType().GetProperty("StatusCode").GetValue(response, null);

            // Assert
            Assert.Equal(statusCodeResult, status);
       }

        [Fact]
       public void BadRequest400_Test()
       {
            // Arrange
            int status = 0;
            int actual = StatusCodes.Status400BadRequest;
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(FeatureCollection => FeatureCollection.Get<IStatusCodeReExecuteFeature>())
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
            Assert.Equal(statusCodeResult, actual);
       }

         [Fact]
       public void UnknownError_Test()
       {
            // Arrange
            int status = 300;
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(FeatureCollection => FeatureCollection.Get<IStatusCodeReExecuteFeature>())
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
            var statusCodeResult = response.GetType().GetProperty("StatusCode").GetValue(response, null);

            // Assert
            Assert.Equal(statusCodeResult, status);
       }
    }
}