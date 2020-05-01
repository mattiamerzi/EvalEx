using System;
using System.Collections.Generic;
using System.Text;

namespace EvalEx.Lib
{
    public class VarDef
    {
        public string Name { get; set; }
        public DFDataType DataType { get; set; }

        public VarDef(string name, DFDataType dataType)
        {
            Name = name;
            DataType = dataType;
        }
    }
    public class DynaFunction
    {
        public string FunctionName { get; private set; }
        public List<VarDef> Variables { get; private set; }
        public DFDataType ReturnType { get; private set; }
        public string StringExpression { get; private set; }
        public int NumVars { get { return Variables.Count;  } }

        public LazyFunction AsLazyFunction()
        {
            return new DynaLazyFunction(this);
        }

        private class DynaLazyFunction: LazyFunction
        {
            private readonly DynaFunction dynaFunction;
            public DynaLazyFunction(DynaFunction dynaFunction):base(dynaFunction.FunctionName, dynaFunction.NumVars)
            {
                this.dynaFunction = dynaFunction;
            }

            public override LazyNumber LazyEval(List<LazyNumber> lazyParams)
            {
                Expression ex = new Expression();
                int i = 0;
                for (i = 0; i < this.dynaFunction.NumVars; i++)
                {
                    string tmpName = this.dynaFunction.Variables[i].Name;
                    switch (this.dynaFunction.Variables[i].DataType)
                    {
                        case DFDataType.FLOAT: ex.SetDoubleVariable(tmpName, lazyParams[i].Eval()); break;
                        case DFDataType.STRING: ex.SetStringVariable(tmpName, lazyParams[i].EvalString()); break;
                        case DFDataType.INT: ex.SetIntVariable(tmpName, Convert.ToInt64(lazyParams[i].Eval())); break;
                    }
                }
                switch (this.dynaFunction.ReturnType)
                {
                    case DFDataType.FLOAT: return new FunLazyNumberS(() => ex.EvalDouble(this.dynaFunction.StringExpression));
                    case DFDataType.INT: return new FunLazyNumberS(() => ex.EvalInt(this.dynaFunction.StringExpression));
                    case DFDataType.STRING: return new FunLazyString(() => ex.EvalString(this.dynaFunction.StringExpression));
                    default: return null;
                }
            }
        }

        public string Stringify()
        {
            var sb = new StringBuilder();
            sb.Append(FunctionName).Append('(');
            bool first = true;
            foreach (var v in Variables)
            {
                if (!first) sb.Append(',');
                sb.Append(v.Name).Append(": ").Append(v.DataType);
                first = false;
            }
            sb.Append("): ").Append(ReturnType).Append(" => ");
            sb.Append(StringExpression);
            return sb.ToString();
        }

        public DynaFunction(string fdef)
        {
            Variables = new List<VarDef> ();
            var fen = new EOFCharEnumerator(fdef);
            FunctionName = UntilSeparator(fen, "(", "function");
            EatBlanks(fen);
            if (fen.PeekNext() == ')') // NO ARGUMENTS FUNCTION
            {
                fen.MoveNext();
            } else 
            {
                while (fen.Current != ')')
                {
                    var tmpVarName = UntilSeparator(fen, ":", "variable name");
                    if (!ExConst.ValidVarName(tmpVarName))
                        throw new ExpressionException("Invalid name for variable " + tmpVarName);
                    var tmpType = UntilSeparator(fen, ",)", "variable type");
                    if (Enum.TryParse(tmpType.ToUpperInvariant(), out DFDataType tmpDFDataType))
                    {
                        Variables.Add(new VarDef(tmpVarName, tmpDFDataType));
                    }
                    else
                    {
                        throw new ExpressionException("Invalid data type for variable " + tmpVarName + ": " + tmpType);
                    }
                }
            }
            EatUntil(fen, ':');
            var tmprettype = UntilSeparator(fen, "=", "return type");
            if (!Enum.TryParse(tmprettype.ToUpperInvariant(), out DFDataType rettype))
            {
                throw new ExpressionException("Invalid return type: " + tmprettype);
            }
            ReturnType = rettype;
            fen.MoveNext();
            if (fen.Current != '>')
                throw new ExpressionException("Unexpected character: " + fen.Current);

            var exp = new StringBuilder();
            while (fen.MoveNext())
            {
                exp.Append(fen.Current);
            }
            StringExpression = exp.ToString().Trim();
            if (string.IsNullOrEmpty(StringExpression))
                throw new ExpressionException("Empty expression");
            new Expression().Parse(StringExpression);
        }
        private void EatBlanks(EOFCharEnumerator fen)
        {
            while (char.IsWhiteSpace(fen.Current) && fen.MoveNext());
        }

        private void EatUntil(EOFCharEnumerator fen, char sep)
        {
            while (fen.MoveNext() && fen.Current != sep)
            {
                if (!char.IsWhiteSpace(fen.Current))
                    throw new ExpressionException("Unexpected char: " + fen.Current);
            }
        }
        private string UntilSeparator(EOFCharEnumerator f, string sep, string what)
        {
            StringBuilder name = new StringBuilder();
            while (f.MoveNext() && sep.IndexOf(f.Current) < 0)
            {
                name.Append(f.Current);
            }
            string ret = name.ToString().Trim();
            if (string.IsNullOrEmpty(ret))
                throw new ExpressionException("Invalid syntax: empty " + what + " name");
            foreach (char c in ret)
            {
                if (char.IsWhiteSpace(c))
                    throw new ExpressionException("Invalid function syntax: unexpected whitespace in " + what + " name");
            }
            return ret;
        }

        private class EOFCharEnumerator
        {
            private readonly string str;
            private int idx;
            private bool EOF { get { return idx == str.Length; } }
            public EOFCharEnumerator(string str)
            {
                this.idx = -1;
                this.str = str;
            }
            public bool MoveNext()
            {
                if (idx < str.Length)
                    idx++;
                return !EOF;
            }
            public char PeekNext()
            {
                return idx+1 == str.Length ? '\0' : str[idx+1];
            }
            public char Current => EOF ? throw new ExpressionException("Unexpected end of input") : idx < 0 ? throw new ExpressionException("You've found a BUG!") : str[idx];
        }

    }
        public enum DFDataType
        {
            STRING, FLOAT, INT
        }
}
