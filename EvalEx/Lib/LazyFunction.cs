using System;
using System.Collections.Generic;
using System.Text;

namespace EvalEx.Lib
{

    public abstract class LazyFunction
    {
        public string Name { get; private set; }
        public int NumParams { get; private set; }

        public LazyFunction(string name, int numParams)
        {
            this.Name = name;
            this.NumParams = numParams;
        }

        public bool NumParamsVaries()
        {
            return NumParams < 0;
        }
        public abstract LazyNumber LazyEval(List<LazyNumber> lazyParams);
    }

    public class FunLazyFunction : LazyFunction
    {
        private readonly Func<List<LazyNumber>, LazyNumber> op;
        public FunLazyFunction(string name, Func<List<LazyNumber>, LazyNumber> op) : base(name, -1)
        {
            this.op = op;
        }

        public override LazyNumber LazyEval(List<LazyNumber> lazyParams)
        {
            return op.Invoke(lazyParams);
        }
    }

    public class FunIf : LazyFunction
    {
        public FunIf() : base("IF", 3)
        {

        }

        public override LazyNumber LazyEval(List<LazyNumber> lazyParams)
        {
            return new LazyIf(lazyParams[0], lazyParams[1], lazyParams[2]);
        }
    }

    public class LazyIf : LazyNumber
    {
        private readonly LazyNumber check;
        private readonly LazyNumber numif;
        private readonly LazyNumber numelse;

        public LazyIf(LazyNumber check, LazyNumber numif, LazyNumber numelse)
        {
            this.check = check;
            this.numif = numif;
            this.numelse = numelse;
        }

        private LazyNumber Which()
        {
            return this.check.Eval() != 0.0D ? this.numif : this.numelse;
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

    }

    public class FunIfBreak : LazyFunction
    {
        public FunIfBreak() : base("IFBRK", 2)
        {

        }

        public override LazyNumber LazyEval(List<LazyNumber> lazyParams)
        {
            return new LazyIfBreak(lazyParams[0], lazyParams[1]);
        }
    }

    public class LazyIfBreak : LazyNumber
    {
        private readonly LazyNumber check;
        private readonly LazyNumber numif;

        public LazyIfBreak(LazyNumber check, LazyNumber numif)
        {
            this.check = check;
            this.numif = numif;

        }
        private LazyNumber Which()
        {
            if (this.check.Eval() != 0.0D)
                return this.numif;
            throw new ExpressionBreakException();
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

    }

}
