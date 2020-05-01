using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static EvalEx.Lib.ExConst;

namespace EvalEx.Lib.Tokenizer
{
    internal class TokenEnumerator : IEnumerator
    {

        private int pos = 0;

        private readonly Expression expression;
        private readonly string input;

        private Token previousToken;

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public Token Current { get; private set; }


        public TokenEnumerator(Expression expression, string input)
        {
            this.expression = expression;
            this.input = input.Trim();
        }

        public void Reset()
        {
            pos = 0;
            previousToken = Current = null;
        }


        private bool HasNext()
        {
            return (pos < input.Length);
        }

        public bool MoveNext()
        {
            if (HasNext())
            {
                Current = Next();
                return true;
            }
            return false;
        }


        /**
         * Peek at the next character, without advancing the iterator.
         * 
         * @return The next character or character 0, if at end of string.
         */
        private char PeekNextChar()
        {
            if (pos < (input.Length - 1))
            {
                return input[pos + 1];
            }
            else
            {
                return NULL_CHAR;
            }
        }

        private bool IsHexDigit(char ch)
        {
            return ch == 'x' || ch == 'X' || (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
        }

        private Token Next()
        {
            Token token = new Token();

            if (pos >= input.Length)
            {
                return previousToken = null;
            }
            char ch = input[pos];
            while (Char.IsWhiteSpace(ch) && pos < input.Length)
            {
                ch = input[++pos];
            }
            token.pos = pos;

            bool isHex = false;

            if (Char.IsDigit(ch))
            {
                if (ch == '0' && (PeekNextChar() == 'x' || PeekNextChar() == 'X')) isHex = true;
                while ((isHex && IsHexDigit(ch)) || (Char.IsDigit(ch) || ch == DecimalSeparator
                                                || ch == 'e' || ch == 'E'
                                                || (ch == MinusSign && token.Length > 0
                                                    && ('e' == token[token.Length - 1] || 'E' == token[token.Length - 1]))
                                                || (ch == '+' && token.Length > 0
                                                    && ('e' == token[token.Length - 1] || 'E' == token[token.Length - 1]))
                                                ) && (pos < input.Length))
                {
                    token.Append(input[pos++]);
                    ch = pos == input.Length ? NULL_CHAR : input[pos];
                }
                token.type = isHex ? TokenType.HEX_LITERAL : TokenType.LITERAL;
            }
            else if (ch == '"')
            {
                pos++;
                while (input[pos] != '"')
                {
                    if (input[pos] == '\\' && input[pos + 1] == '"')
                    {
                        token.Append('"');
                        pos++;
                    }
                    else
                    {
                        token.Append(input[pos]);
                    }
                    pos++;
                }
                pos++;
                token.type = TokenType.STRINGPARAM;
                    /*
                    pos++;
                    if (previousToken == null || previousToken.type != TokenType.STRINGPARAM) // questo controllo non ricordo perche' e' cosi', ma e' cosi' ...

                    {
                        ch = input[pos];
                        while (ch != '"' && pos < input.Length)
                        {
                            token.Append(input[pos++]);
                            ch = pos == input.Length ? NULL_CHAR : input[pos]; // TODO controllo stupido?
                        }
                        if (ch != '"' && pos == input.Length)
                            throw new ExpressionException("Unterminated string constant");
                        pos++;
                        token.type = TokenType.STRINGPARAM;
                    }
                    else
                    {
                        return Next();
                    }
                    */
            }
            else if (ExConst.ValidVarStart(ch))
            {
                while ((
                    (token.Length == 0 && ExConst.ValidVarStart(ch)) ||
                    (token.Length > 0 && ExConst.ValidVarNameWith(token.ToString(), ch))
                    ) && (pos < input.Length))
                {
                    token.Append(input[pos++]);
                    ch = pos == input.Length ? NULL_CHAR : input[pos];
                }
                bool isMap = token[0] == ExConst.MapChar;
                bool isReduce = token[0] == ExConst.ReduceChar;
                bool isMapOrReduce = isMap || isReduce;
                if (token.Length == 1 && isMapOrReduce)
                    throw new ExpressionException("No function specified after Map or Reduce");
                //Remove optional white spaces after function or variable name
                if (char.IsWhiteSpace(ch))
                {
                    while (char.IsWhiteSpace(ch) && pos < input.Length)
                    {
                        ch = input[pos++];
                    }
                    pos--;
                }
                token.type = ch == '(' ? TokenType.FUNCTION : TokenType.VARIABLE;
                if (token.type != TokenType.FUNCTION && isMapOrReduce)
                {
                    throw new ExpressionException("No function specified after Map or Reduce");
                }
            }
            else if (ch == '(' || ch == ')' || ch == ',')
            {
                if (ch == '(')
                {
                    token.type = TokenType.OPEN_PAREN;
                }
                else if (ch == ')')
                {
                    token.type = TokenType.CLOSE_PAREN;
                }
                else
                {
                    token.type = TokenType.COMMA;
                }
                token.Append(ch);
                pos++;
            }
            else if (ch == '.' && previousToken != null && previousToken.type == TokenType.VARIABLE)
            {
                ch = input[++pos];
                if (ch == '"')
                {
                    pos++;
                    while (input[pos] != '"')
                    {
                        if (input[pos] == '\\' && input[pos+1] == '"')
                        {
                            token.Append('"');
                            pos++;
                        }
                        else
                        {
                            token.Append(input[pos]);
                        }
                        pos++;
                    }
                    pos++;
                }
                else
                {
                    while (ExConst.ValidPropertyWith(token.ToString(), ch) && (pos < input.Length))
                    {
                        token.Append(input[pos++]);
                        ch = pos == input.Length ? NULL_CHAR : input[pos];
                    }
                }
                //Remove optional white spaces after function or variable name
                if (char.IsWhiteSpace(ch))
                {
                    while (char.IsWhiteSpace(ch) && pos < input.Length)
                    {
                        ch = input[pos++];
                    }
                    pos--;
                }
                token.type = TokenType.OBJPATH;
            }
            else
            {
                bool if_op_then_unary = previousToken == null || previousToken.type == TokenType.OPERATOR || previousToken.type == TokenType.OPEN_PAREN || previousToken.type == TokenType.COMMA || previousToken.type == TokenType.UNARY_OPERATOR;
                string greedyMatch = "";
                string optest;
                int initialPos = pos;
                ch = input[pos];
                int validOperatorSeenUntil = -1;

                for (int i=0; i < ExConst.MAX_OPERATOR_LENGTH; i++)
                {
                    if (++pos == input.Length)
                        break;
                    greedyMatch += ch;
                    optest = if_op_then_unary ? greedyMatch + "u" : greedyMatch;
                    if (this.expression.OperatorExists(optest))
                    {
                        validOperatorSeenUntil = pos;
                    }
                    ch = input[pos];
                }
                if (validOperatorSeenUntil != -1)
                {
                    token.Append(input.Substring(initialPos, validOperatorSeenUntil - initialPos));
                    pos = validOperatorSeenUntil;
                }
                else
                {
                    token.Append(greedyMatch);
                }

                if (if_op_then_unary)
                {
                    token.surface += "u";
                    token.type = TokenType.UNARY_OPERATOR;
                }
                else
                {
                    token.type = TokenType.OPERATOR;
                }
            }
            return previousToken = token;
        }


    }
}
