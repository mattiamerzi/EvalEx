using System;
using System.Collections.Generic;
using System.Text;

namespace EvalEx.Lib.Tokenizer
{
    internal enum TokenType
    {
        VARIABLE, FUNCTION, LITERAL, OPERATOR, UNARY_OPERATOR, OPEN_PAREN, COMMA, CLOSE_PAREN, HEX_LITERAL, STRINGPARAM, OBJPATH
    }
}
