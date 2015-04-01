using System;
using KaVE.Commons.Utils.Exceptions;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public class InvalidResponseException : KaVEException
    {
        public InvalidResponseException(string message, Exception error) : base(message, error) {}
    }
}