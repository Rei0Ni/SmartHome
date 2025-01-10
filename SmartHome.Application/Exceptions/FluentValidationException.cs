using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace SmartHome.Application.Exceptions
{
    public class FluentValidationException : Exception
    {
        public string? Status { get; private set; }
        public new IEnumerable<ValidationFailure>? Data { get; private set; }

        public FluentValidationException()
        {
        }

        public FluentValidationException(string? message) : base(message)
        {
        }

        public FluentValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public FluentValidationException(string? status, string? message, IEnumerable<ValidationFailure> errors) : base(message)
        {
            Status = status;
            Data = errors;
        }
    }
}
