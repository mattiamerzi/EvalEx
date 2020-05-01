using System;
using System.Runtime.Serialization;

namespace EvalEx.Lib
{
    [Serializable]
    public class UnknownVariableInExpressionException : Exception
    {
        public string Var { get; private set; }
        public UnknownVariableInExpressionException()
        {
        }

        public UnknownVariableInExpressionException(string message, string var) : base(message)
        {
            this.Var = var;
        }

        public UnknownVariableInExpressionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownVariableInExpressionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}