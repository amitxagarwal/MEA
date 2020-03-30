using System.Net;

namespace Kmd.Momentum.Mea.Common.Exceptions
{
    public class Error
    {
        public string Message { get; }
        public HttpStatusCode? StatusCode { get;}

        public Error(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}

