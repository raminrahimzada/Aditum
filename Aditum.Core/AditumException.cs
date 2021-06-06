using System;

namespace Aditum.Core
{
    [Serializable]
    public sealed class AditumException : Exception
    {
        private AditumException(string message):base(message)
        {
        }
        public static AditumException ParameterNeeded(string parameterName)
        {
            return new AditumException($"Parameter {parameterName} needed to be non null for that operation");
        }

        public static AditumException NoMatchFound(string parameterName, object parameterValue)
        {
            return new AditumException($"No {parameterName} found for given parameter:{parameterValue}");
        }

        public static Exception InvalidState(object state)
        {
            return new AditumException($"Invalid State : {state}");
        }
    }
}