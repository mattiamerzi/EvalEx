using System.Collections.Generic;

namespace EvalEx.Lib
{
    public class LazyRNGPERC : LazyNumber
    {
        private readonly List<LazyNumber> lazyParams;

        public LazyRNGPERC(List<LazyNumber> lazyParams)
        {
            this.lazyParams = lazyParams;
        }

        public override double Eval()
        {
            return Which().Eval();
        }

        public override double[] EvalArray()
        {
            return Which().EvalArray();
        }

        public override string EvalString()
        {
            return Which().EvalString();
        }

        private LazyNumber Which()
        {
            double val = lazyParams[0].Eval();
            for (int i = 1; i < (lazyParams.Count - 2); i += 2)
            {
                double perc = lazyParams[i].Eval();
                if (val <= perc)
                {
                    return lazyParams[i + 1];
                }
            }
            return lazyParams[lazyParams.Count - 1];
        }
    }

    public class RNGPERC : LazyFunction
    {
        public RNGPERC() : base("RNGPERC", -1)
        {
        }

        public override LazyNumber LazyEval(List<LazyNumber> lazyParams)
        {
            if (lazyParams.Count < 4)
                throw new ExpressionBreakException();
            return new LazyRNGPERC(lazyParams);
        }
    }
}
