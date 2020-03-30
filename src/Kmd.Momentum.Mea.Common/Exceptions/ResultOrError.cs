using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Kmd.Momentum.Mea.Common.Exceptions
{
    public class ResultOrError<TResult, TError>
    {
        public ResultOrError(TResult result)
        {
            this.Result = result;
            this.IsError = false;
        }

        public ResultOrError(TError error)
        {
            this.Error = error;
            this.IsError = true;
        }

        public TResult Result { get; }
        public TError Error { get; }
        public bool IsError { get; }
               
#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator ResultOrError<TResult, TError>(TResult result) => FromResult(result);
        public static implicit operator ResultOrError<TResult, TError>(TError error) => FromError(error);
#pragma warning restore CA2225 // Operator overloads have named alternates

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static ResultOrError<TResult, TError> FromResult(TResult result)
        {
            return new ResultOrError<TResult, TError>(result);
        }

        public static ResultOrError<TResult, TError> FromError(TError error)
        {
            return new ResultOrError<TResult, TError>(error);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }   
}
