using System;
using System.Collections.Generic;
using System.Text;

namespace EvalEx.Lib.Tokenizer
{
    internal class Token
    {
        public string surface = "";
        public TokenType type;
        public int pos;

        public void Append(char c)
        {
            surface += c;
        }

        public void Append(string s)
        {
            surface += s;
        }

        public char this[int i] => surface[i];

        public char CharAt(int pos)
        {
            return surface[pos];
        }

        public int Length
        {
            get
            {
                return surface.Length;
            }
        }

        public override string ToString()
        {
            return surface;
        }
    }
}
