using System;

namespace Elixir.BusinessLogic.Exceptions
{
    public class ModelValidationException : Exception
    {
        public ModelValidationException(string msg) : base(msg) { }
        public ModelValidationException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}
