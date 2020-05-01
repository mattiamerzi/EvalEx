using System;
using System.Collections.Generic;
using System.Text;
using static EvalEx.Lib.ExConv;

namespace EvalEx.Lib
{
    public abstract class LazyNumber
    {
        public abstract double Eval();
        public abstract double[] EvalArray();
        public virtual string EvalString()
        {
            double[] arr = EvalArray();
            switch (arr.Length)
            {
                case 0: return "NaN";
                case 1: return DtoS(arr[0]);
                default: return AtoS(arr);
            }
        }
    }
}
