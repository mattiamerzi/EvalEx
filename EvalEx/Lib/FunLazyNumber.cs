using System;
using System.Collections.Generic;
using System.Text;
using static EvalEx.Lib.ExConst;

namespace EvalEx.Lib
{

    public class FunLazyNumber : LazyNumber
    {
        private readonly Func<double> evalFunc;
        private readonly Func<double[]> evalArrayFunc;
        public Func<string> StringFunc { get; set; }

        public FunLazyNumber(Func<double> evalFunc, Func<double[]> evalArrayFunc)
        {
            this.evalFunc = evalFunc;
            this.evalArrayFunc = evalArrayFunc;
            if (evalFunc == null && evalArrayFunc == null)
                throw new ExpressionException("Cannot declare a FunLazyNumber with BOTH evaluation functions as NULL, you've hit a BUG in a function declaration!");
            StringFunc = base.EvalString;
        }

        public override double Eval()
        {
            if (evalFunc == null)
            {
                double[] val = EvalArray();
                if (val == null)
                    return double.NaN;
                return val.Length;
            }
            return evalFunc.Invoke();
        }

        public override double[] EvalArray()
        {
            if (evalArrayFunc == null)
            {
                double val = Eval();
                if (double.IsNaN(val))
                    return EMPTY_DOUBLE_ARRAY;
                return new double[] { val };
            }
            return evalArrayFunc.Invoke();
        }

        public override string EvalString()
        {
            return StringFunc.Invoke();
        }
    }

    public class FunLazyNumberS : FunLazyNumber
    {
        public FunLazyNumberS(Func<double> evalFunc) : base(evalFunc, null)
        {
        }

        public FunLazyNumberS(Func<bool> evalFunc) : base(() => evalFunc.Invoke() ? 1.0D : 0.0D, null)
        {
        }
    }

    public class FunLazyNumberA : FunLazyNumber
    {
        public FunLazyNumberA(Func<double[]> evalArrayFunc) : base(null, evalArrayFunc)
        {
        }
    }
}
