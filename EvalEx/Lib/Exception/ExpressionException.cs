using System;
using System.Collections.Generic;
using System.Text;

namespace EvalEx.Lib
{
    public class ExpressionException:Exception
    {
        public ExpressionException(string message) : base(message)
        {
        }

    }
}
