using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoApp.Exceptions
{
    public class UserRegistrationException : Exception
    {
        public UserRegistrationException()
        {
        }

        public UserRegistrationException(string message)
            : base(message)
        {
        }

        public UserRegistrationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
