using ArsalanAssesment.Web.DTOs;
using System.Net;

namespace ArsalanAssesment.Web.Helper
{
    public static class ResponseHelper
    {

        /// <summary>
        /// Creates the response for CRUD operations in the repository.
        /// </summary>
        /// <param name="isSuccess">Indicates whether the operation was successful or not.</param>
        /// <param name="isException">Indicates whether the exception was caught or not.</param>
        /// <param name="message">A message providing details about the result of the CRUD operation.</param>
        /// <param name="result">Optional result object, containing the data returned from the operation if successful, Null for failure.</param>
        /// <returns>Returns a <see cref="ResponseDTO"/> containing the success status, message, and result.</returns>
        public static ResponseDTO CreateResponse(bool isSuccess, bool isException, string message, object? result = null)
        {
            return new ResponseDTO
            {
                IsException = isException,
                IsSuccess = isSuccess,
                Message = message,
                Result = result
            };
        }

        /// <summary>
        /// Creates a Fake Response For Unit Testing
        /// </summary>
        /// <param name="message">The message which is equal for the action in Repository</param>
        /// <param name="httpStatusCode">The Status which is equal for the action in Repository</param>
        /// <param name="result">The Status Code which is equal for the action in Repository</param>
        /// <returns>Returns a <see cref="ResponseDTO"/> containing the success status, message, and result.</returns>
        public static ResponseDTO CreateFakeResponse(string message, HttpStatusCode httpStatusCode, object result)
        {
            return new ResponseDTO
            {
                IsException = false,
                IsSuccess = true,
                Message = message,
                Result = result,
                StatusCode = (int)httpStatusCode
            };
        }
    }

}
