using System;
using System.Net;

namespace Kmd.Momentum.Mea.Common.Exceptions
{
    /// <remarks>
    /// This class is made available to provide backward compatibility for functionality which relies on StatusCode.
    /// </remarks>
    /// <typeparam name="TResult">The result</typeparam>
    /// <typeparam name="TError">The error</typeparam>
    public class ResultOrHttpError<TResult, TError>
    {
        public ResultOrHttpError(TResult result)
        {
            this.Result = result;
            this.IsError = false;
        }

        public ResultOrHttpError(TError error)
        {
            this.Error = error;
            this.IsError = true;
        }
        public ResultOrHttpError(TError error, HttpStatusCode status)
        {
            this.Error = error;
            this.IsError = true;
            this.StatusCode = status;
        }

        public TResult Result { get; }
        public TError Error { get; }
        public bool IsError { get; }
        public HttpStatusCode? StatusCode { get; }       
    }
}
