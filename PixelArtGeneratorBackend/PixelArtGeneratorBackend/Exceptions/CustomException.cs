using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourPetAPI.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {

        }
    }
}
