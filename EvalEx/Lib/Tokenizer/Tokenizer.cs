using EvalEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EvalEx.Lib.Tokenizer
{
    internal class Tokenizer : IEnumerable
    {
        private readonly Expression expression;
        private readonly string str2tok;
        public Tokenizer(Expression expression, string str2tok)
        {
            this.expression = expression;
            this.str2tok = str2tok;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TokenEnumerator GetEnumerator()
        {
            return new TokenEnumerator(expression, str2tok);
        }
    }
}
