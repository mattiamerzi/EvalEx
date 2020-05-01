using System;
using System.Collections.Generic;
using System.Text;

namespace EvalEx.Lib
{
    public abstract class Operator
    {
        /**
         * This operators name (pattern).
         */
        private readonly string Oper;
        /**
         * Operators precedence.
         */
        private readonly int Precedence;
        /**
         * Operator is left associative.
         */
        private readonly bool LeftAssoc;
        /**
         * Operands must be evaluated as strings
         */
        private readonly bool StringOperator;

        /**
         * Creates a new operator.
         * 
         * @param oper
         *            The operator name (pattern).
         * @param precedence
         *            The operators precedence.
         * @param leftAssoc
         *            <code>true</code> if the operator is left associative,
         *            else <code>false</code>.
         */
        public Operator(string oper, int precedence, bool leftAssoc): this(oper, precedence, leftAssoc, false)
        {
        }

        public Operator(string oper, int precedence, bool leftAssoc, bool stringOperator)
        {
            this.Oper = oper;
            this.Precedence = precedence;
            this.LeftAssoc = leftAssoc;
            this.StringOperator = stringOperator;
        }

        public string GetOper()
        {
            return Oper;
        }

        public int GetPrecedence()
        {
            return Precedence;
        }

        public bool IsLeftAssoc()
        {
            return LeftAssoc;
        }

        public bool IsStringOperator()
        {
            return StringOperator;
        }

        public virtual bool IsUnary()
        {
            return false;
        }
        /**
         * Implementation for this operator.
         * 
         * @param v1
         *            Operand 1.
         * @param v2
         *            Operand 2.
         * @return The result of the operation.
         */
        public abstract LazyNumber Eval(LazyNumber v1, LazyNumber v2);
    }

    public class FunOperator : Operator
    {
        private readonly Func<double, double, double> op;
        public FunOperator(string oper, int precedence, bool leftAssoc, Func<double, double, double> op) : base(oper, precedence, leftAssoc)
        {
            this.op = op;
        }
        public override LazyNumber Eval(LazyNumber v1, LazyNumber v2)
        {
            return new FunLazyNumberS(() => op.Invoke(v1.Eval(), v2.Eval()));
        }
    }

    public class FunBoolOperator : Operator
    {
        private readonly Func<double, double, bool> op;
        public FunBoolOperator(string oper, int precedence, bool leftAssoc, Func<double, double, bool> op) : base(oper, precedence, leftAssoc)
        {
            this.op = op;
        }
        public override LazyNumber Eval(LazyNumber v1, LazyNumber v2)
        {
            return new FunLazyNumberS(() => this.op.Invoke(v1.Eval(), v2.Eval()));
        }
    }

    public class FunStringOperator : Operator
    {
        private readonly Func<string, string, string> op;
        public FunStringOperator(string oper, int precedence, bool leftAssoc, Func<string, string, string> op) : base(oper, precedence, leftAssoc)
        {
            this.op = op;
        }
        public override LazyNumber Eval(LazyNumber v1, LazyNumber v2)
        {
            return new FunLazyString(() => op.Invoke(v1.EvalString(), v2.EvalString()));
        }
    }

    public class FunBoolStringOperator : Operator
    {
        private readonly Func<string, string, bool> op;
        public FunBoolStringOperator(string oper, int precedence, bool leftAssoc, Func<string, string, bool> op) : base(oper, precedence, leftAssoc)
        {
            this.op = op;
        }
        public override LazyNumber Eval(LazyNumber v1, LazyNumber v2)
        {
            return new FunLazyNumberS(() => op.Invoke(v1.EvalString(), v2.EvalString()));
        }
    }

    public abstract class UnaryOperator : Operator
    {

        public UnaryOperator(string oper, int precedence, bool leftAssoc) : base(oper, precedence, leftAssoc)
        {
        }

        public override LazyNumber Eval(LazyNumber v1, LazyNumber v2)
        {
            if (v2 != null)
            {
                throw new ExpressionException("Did not expect a second parameter for unary operator");
            }
            return EvalUnary(v1);
        }

        abstract public LazyNumber EvalUnary(LazyNumber v1);

        public override bool IsUnary()
        {
            return true;
        }
    }

    public class FunUnaryOperator : UnaryOperator
    {
        private readonly Func<double, double> op;
        public FunUnaryOperator(string oper, int precedence, bool leftAssoc, Func<double, double> op) : base(oper, precedence, leftAssoc)
        {
            this.op = op;
        }
        public override LazyNumber EvalUnary(LazyNumber v)
        {
            return new FunLazyNumberS(() => this.op.Invoke(v.Eval()));
        }
    }

}
