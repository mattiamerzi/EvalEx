using System;
using System.Runtime.Serialization;

namespace EvalEx.Lib
{
    [Serializable]
    public class UnknownFunctionInExpressionException : Exception
    {
        public string Fun { get; private set; }
        public UnknownFunctionInExpressionException()
        {
        }

        public UnknownFunctionInExpressionException(string message, string var) : base(message)
        {
            this.Fun = var;
        }

        public UnknownFunctionInExpressionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownFunctionInExpressionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}