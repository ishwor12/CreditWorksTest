using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleManagement.Application.Exceptions
{
    public class ValidationFailedException:Exception
    {
        public List<string> Errors { get; }

        public ValidationFailedException(List<string> errors) : base("Validation failed.")
        {
            Errors = errors;
        }

        public ValidationFailedException(string error) : this(new List<string> { error })
        {
        }

    }
}
