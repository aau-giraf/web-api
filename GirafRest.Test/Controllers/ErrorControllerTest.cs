using GirafRest.Controllers;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

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
            mockFeatureCollection.Setup(feature => feature.Get<IStatusCodeReExecuteFeature>()) //feature is a parameter from IFeatureCollection
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
            mockFeatureCollection.Setup(feature => feature.Get<IStatusCodeReExecuteFeature>())
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
            int status = StatusCodes.Status400BadRequest;
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(feature => feature.Get<IStatusCodeReExecuteFeature>())
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
        public void UnknownError_Test()
        {
            // Arrange
            int status = 300;
            var mockFeatureCollection = new Mock<IFeatureCollection>();
            mockFeatureCollection.Setup(feature => feature.Get<IStatusCodeReExecuteFeature>())
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

        //[Fact]
        //public void NotFoundError_Test()
        //{
        //    // Arrange
        //    int status = 404;
        //    var mockFeatureCollection = new Mock<IFeatureCollection>();
        //    mockFeatureCollection.Setup(feature => feature.Get<IStatusCodeReExecuteFeature>())
        //        .Returns
        //        (
        //            new StatusCodeReExecuteFeature()
        //            {
        //                OriginalPath = "Path",
        //                OriginalPathBase = "Base",
        //                OriginalQueryString = "String"
        //            }
        //        );

        //    var httpContext = new DefaultHttpContext(mockFeatureCollection.Object);
        //    var controllerContext = new ControllerContext()
        //    {
        //        HttpContext = httpContext,
        //    };

        //    var controller = new MockErrorController()
        //    {
        //        ControllerContext = controllerContext
        //    };

        //    // Act
        //    var response = controller.StatusCodeEndpoint(status);
        //    var statusCodeResult = response.GetType().GetProperty("StatusCode").GetValue(response, null);

        //    // Assert
        //    Assert.Equal(statusCodeResult, status);
        //}

        [Fact]
        public void NotFoundErrorTest_Test()
        {
            // Arrange
            int status = 404;
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));
            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Request == request.Object
            );

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