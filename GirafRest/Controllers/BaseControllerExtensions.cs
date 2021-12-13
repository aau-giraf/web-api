
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GirafRest.Controllers {
    /// <summary>
    /// Extensions for the base controller
    /// </summary>
    public static class BaseControllerExtensions {
        /// <summary>
        /// The request could not be process because a crucial object from the HTTP body
        /// </summary>
        /// <param name="controller">this reference</param>
        /// <param name="type">The <see cref="System.Type"/> name of the missing object</param>
        /// <returns>an <see cref="ObjectResult"/> with code <see cref="StatusCodes.Status400BadRequest"/> and <see cref="ErrorCode.MissingProperties"/></returns>
        public static ObjectResult MissingObjectFromBody(this ControllerBase controller, string type) {
            return controller.StatusCode(
                StatusCodes.Status400BadRequest,
                new ErrorResponse(
                    ErrorCode.MissingBodyObject,
                    $"{type} missing from request body"
                )
            );
        }

        /// <summary>
        /// The request could not be process because a crucial property is missing from the request
        /// </summary>
        /// <param name="controller">this reference</param>
        /// <param name="property">The name of the missing property</param>
        /// <returns>an <see cref="ObjectResult"/> with code <see cref="StatusCodes.Status400BadRequest"/> and <see cref="ErrorCode.MissingProperties"/></returns>
        public static ObjectResult MissingPropertyFromRequest(this ControllerBase controller, string property) {
            return controller.StatusCode(
                StatusCodes.Status400BadRequest,
                new ErrorResponse(
                    ErrorCode.MissingProperties,
                    $"{property} missing in request body"
                )
            );
        }

        /// <summary>
        /// The request body contains invalid propreties
        /// </summary>
        /// <param name="controller">this reference</param>
        /// <param name="properties">the invalid properties</param>
        /// <returns>an <see cref="ObjectResult"/> with code <see cref="StatusCodes.Status400BadRequest"/> and <see cref="ErrorCode.InvalidProperties"/></returns>
        public static ObjectResult InvalidPropertiesFromRequest(this ControllerBase controller, params object[] properties) {
            string message = "some properties are invalid";
            if (properties.Length > 0) {
                string property = ConvertToString(properties);
                message = $"the properties {property} are invalid";
            }
            return controller.StatusCode(
                StatusCodes.Status400BadRequest,
                new ErrorResponse(ErrorCode.InvalidProperties, message)
            );
        }

        /// <summary>
        /// A given resource could not be found given certain ids
        /// </summary>
        /// <param name="controller">this reference</param>
        /// <param name="resource">The <see cref="System.Type"/> name of the missing object</param>
        /// <param name="ids">the ids used to query the resource (eg. userId)</param>
        /// <returns>an <see cref="ObjectResult"/> with code <see cref="StatusCodes.Status404NotFound"/> and <see cref="ErrorCode.NotFound"/></returns>

        public static ObjectResult ResourceNotFound(this ControllerBase controller, string resource, params object[] ids) {
            string id = ConvertToString(ids);
            string message = $"could not find {resource}";
            if (!string.IsNullOrEmpty(id)) {
                message += $" with id(s): '{id}'";
            }
            return controller.StatusCode(
                StatusCodes.Status404NotFound,
                new ErrorResponse(ErrorCode.NotFound, message)
            );
        }

        /// <summary>
        /// A conflicting resource based on key constraints
        /// </summary>
        /// <param name="controller">this reference</param>
        /// <param name="resource">The <see cref="System.Type"/> name of the missing object</param>
        /// <param name="ids">the ids used to query the resource (eg. userId)</param>
        /// <returns>an <see cref="ObjectResult"/> with code <see cref="StatusCodes.Status409Conflict"/> and <see cref="ErrorCode.ConflictingResource"/></returns>
        public static ObjectResult ResourceConflict(this ControllerBase controller, string resource, params object[] ids) {
            string id = ConvertToString(ids);
            string message = $"Conflicting resource for {resource}";
            if (!string.IsNullOrEmpty(id)) {
                message += $" with id(s): '{id}'";
            }
            return controller.StatusCode(
                StatusCodes.Status409Conflict,
                new ErrorResponse(ErrorCode.ConflictingResource, message)
            );
        }

        /// <summary>
        /// A given resource has been created succesfully
        /// </summary>
        /// <remarks>
        /// After saving the DBContext the insertions have gotten their properties injected.
        /// Be casefull when copying data before if you want the data in the response (eg. ids).
        /// </remarks>
        /// <param name="controller">this reference</param>
        /// <param name="resource">The created resource</param>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <returns>an <see cref="ObjectResult"/> with code <see cref="StatusCodes.Status201Created"/></returns>
        public static ObjectResult ResourceCreated<T>(this ControllerBase controller, T resource) {
            return controller.StatusCode(
                StatusCodes.Status201Created,
                new SuccessResponse<T>(resource)
            );
        }

        /// <summary>
        /// The resource has been fetched and transmitted in the message body
        /// </summary>
        /// <remarks>
        /// After saving the DBContext the insertions have gotten their properties injected.
        /// Be casefull when copying data before if you want the data in the response (eg. ids).
        /// </remarks>
        /// <param name="controller">this reference</param>
        /// <param name="resource">The requested resource</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>an appropriate <see cref="ObjectResult"/> for HTTP code <see cref="StatusCodes.Status200OK"/></returns>
        public static ObjectResult RequestSucceeded<T>(this ControllerBase controller, T resource = default) {
            return controller.StatusCode(
                StatusCodes.Status200OK,
                new SuccessResponse<T>(resource)
            );
        }

        private static string ConvertToString(params object[] objs) {
            return string.Join(
                ", ",
                new List<object>(objs)
                    .ConvertAll<string>(elem => elem.ToString())
            );
        }
    }
}