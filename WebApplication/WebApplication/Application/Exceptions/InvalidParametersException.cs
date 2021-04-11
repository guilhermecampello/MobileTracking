using System;

namespace WebApplication.Application
{
    public class InvalidParametersException : Exception
    {
        public InvalidParametersException(string parameterName, object? value, string? motive)
            : base($"Invalid parameter: {parameterName} Value: {value} \n {motive}")
        {
        }
    }
}
