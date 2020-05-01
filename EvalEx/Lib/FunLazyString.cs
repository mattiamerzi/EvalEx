using System;
using System.Collections.Generic;
using System.Text;
using static EvalEx.Lib.ExConv;

namespace EvalEx.Lib
{
    public class FunLazyString : LazyNumber
    {
        private readonly Func<string> evalStringFunc;
        public FunLazyString(Func<string> evalStringFunc)
        {
            this.evalStringFunc = evalStringFunc;
        }

        public override double Eval()
        {
            return StoD(this.EvalString());
        }

        public override double[] EvalArray()
        {
            return new double[] { Eval() };
        }

        public override string EvalString()
        {
            return this.evalStringFunc.Invoke();
        }
    }
}
