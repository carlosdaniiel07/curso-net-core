using System;
using System.Net;

namespace CursoNetCore.Service.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; private set; }

        public ApiException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = (int)statusCode;
        }
    }
}
