using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Application.Exceptions
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException()
        {
        }

        public LoginFailedException(string? message) : base(message)
        {
        }

        public LoginFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
