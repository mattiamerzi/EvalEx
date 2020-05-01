using System;
using System.Collections.Generic;
using System.Text;

namespace EvalEx.Lib
{
    // basic " Rn -> R " functions and most common variants

    public abstract class Function : LazyFunction
    {
        public Function(string name, int numParams) : base(name, numParams)
        {
        }

        private class LazyNumberFunctionEval : LazyNumber
        {
            private List<double> parms;
            private readonly List<LazyNumber> lazyParams;
            private readonly Function function;
            public LazyNumberFunctionEval(List<LazyNumber> lazyParams, Function function)
            {
                this.lazyParams = lazyParams;
                this.function = function;
            }

            public override double Eval()
            {
                return function.Eval(Params);
            }

            public override double[] EvalArray()
            {
                return new double[] { Eval() };
            }
            private List<double> Params
            {
                get
                {
                    if (parms == null)
                    {
                        parms = new List<double>();
                        foreach (LazyNumber lazyParam in lazyParams)
                        {
                            parms.Add(lazyParam.Eval());
                        }
                    }
                    return parms;
                }
            }
        }

        public override LazyNumber LazyEval(List<LazyNumber> lazyParams)
        {
            return new LazyNumberFunctionEval(lazyParams, this);
        }

        public abstract double Eval(List<double> parameters);
    }

    public class FunFunction : Function
    {
        private readonly Func<List<double>, double> op;
        public FunFunction(string name, Func<List<double>, double> op) : base(name, -1)
        {
            this.op = op;
        }

        public override double Eval(List<double> parameters)
        {
            return op.Invoke(parameters);
        }
    }

    public class FunNoParamsFunction : Function
    {
        private readonly Func<double> op;
        public FunNoParamsFunction(string name, Func<double> op) : base(name, 0)
        {
            this.op = op;
        }

        public override double Eval(List<double> parameters)
        {
            return op.Invoke();
        }
    }

    public class Fun1PFunction : Function
    {
        private readonly Func<double, double> op;
        public Fun1PFunction(string name, Func<double, double> op) : base(name, 1)
        {
            this.op = op;
        }

        public override double Eval(List<double> parameters)
        {
            return op.Invoke(parameters[0]);
        }
    }

    public class Fun2PFunction : Function
    {
        private readonly Func<double, double, double> op;
        public Fun2PFunction(string name, Func<double, double, double> op) : base(name, 2)
        {
            this.op = op;
        }

        public override double Eval(List<double> parameters)
        {
            return op.Invoke(parameters[0], parameters[1]);
        }
    }

    public class Fun3PFunction : Function
    {
        private readonly Func<double, double, double, double> op;
        public Fun3PFunction(string name, Func<double, double, double, double> op) : base(name, 3)
        {
            this.op = op;
        }

        public override double Eval(List<double> parameters)
        {
            return op.Invoke(parameters[0], parameters[1], parameters[2]);
        }
    }
}
