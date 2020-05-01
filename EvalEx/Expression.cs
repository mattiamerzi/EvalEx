using EvalEx.Lib;
using EvalEx.Lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static EvalEx.Lib.ExConst;
using static EvalEx.Lib.ExConv;

namespace EvalEx
{

    public class Expression
    {

        #region declarations

        protected Dictionary<string, Operator> operators = new Dictionary<string, Operator>();
        protected Dictionary<string, LazyFunction> functions = new Dictionary<string, LazyFunction>();
        protected Dictionary<string, double> variables = new Dictionary<string, double>();
        protected Dictionary<string, string> stringVariables = new Dictionary<string, string>();
        protected Dictionary<string, double[]> vararrays = new Dictionary<string, double[]>();

        private readonly Random Random = new Random();

        #endregion

        #region expression hacks
        /**
         * The double representation of the left parenthesis, 
         * used for parsing varying numbers of function parameters.
         */
        private class ParamsStartLazyNumber : LazyNumber
        {
            public override double[] EvalArray()
            {
                return new double[0];
            }
            public override double Eval()
            {
                return double.NaN;
            }
        };

        private readonly LazyNumber PARAMS_START = new ParamsStartLazyNumber();

        #endregion

        #region constructor

        public Expression()
        {
            AddOperator(new FunOperator("+", 20, true, (v1, v2) => v1 + v2));
            AddOperator(new FunStringOperator("++", 20, true, (v1, v2) => v1 + " " + v2));
            AddOperator(new FunStringOperator("+++", 20, true, (v1, v2) => v1 + " + " + v2));
            AddOperator(new FunStringOperator("%%", 20, true, (v1, v2) => v1 + v2));
            AddOperator(new FunOperator("-", 20, true, (v1, v2) => v1 - v2));
            AddOperator(new FunOperator("*", 30, true, (v1, v2) => v1 * v2));
            AddOperator(new FunOperator("/", 30, true, (v1, v2) => v1 / v2));
            AddOperator(new FunOperator("%", 30, true, (v1, v2) => v1 % v2));
            AddOperator(new FunOperator("^", 40, false, (v1, v2) => Math.Pow(v1, v2)));
            AddOperator(new FunBoolOperator("&&", 4, false, (v1, v2) => (v1 != 0.0D && v2 != 0.0D)));
            AddOperator(new FunBoolOperator("||", 2, false, (v1, v2) => (v1 != 0.0D || v2 != 0.0D)));
            AddOperator(new FunBoolOperator(">", 10, false, (v1, v2) => (v1 > v2)));
            AddOperator(new FunBoolStringOperator(">>", 10, false, (v1, v2) => v1.Trim().Length > v2.Trim().Length));
            AddOperator(new FunBoolOperator(">=", 10, false, (v1, v2) => (v1 >= v2)));
            AddOperator(new FunBoolOperator("<", 10, false, (v1, v2) => (v1 < v2)));
            AddOperator(new FunBoolStringOperator("<<", 10, false, (v1, v2) => v1.Trim().Length < v2.Trim().Length));
            AddOperator(new FunBoolOperator("<=", 10, false, (v1, v2) => (v1 <= v2)));
            AddOperator(new FunBoolOperator("=", 7, false, (v1, v2) => (v1 == v2)));
            AddOperator(new FunBoolStringOperator("==", 7, false, (v1, v2) => v1.Trim().Equals(v2.Trim(), StringComparison.InvariantCultureIgnoreCase)));
            AddOperator(new FunBoolStringOperator("!=", 7, false, (v1, v2) => !v1.Trim().Equals(v2.Trim(), StringComparison.InvariantCultureIgnoreCase)));
            AddOperator(new FunBoolOperator("<>", 7, false, (v1, v2) => (v1 != v2)));
            AddOperator(new FunUnaryOperator("-", 60, false, (v) => v * -1.0D));
            AddOperator(new FunUnaryOperator("+", 60, false, (v) => v * +1.0D));
            AddOperator(new FunUnaryOperator("!", 60, false, (v) => (v == 0.0D ? 1.0D : 0.0D)));
            AddFunction(new Fun1PFunction("NOT", (v) => (v == 0.0D ? 1.0D : 0.0D)));
            AddFunction(new FunNoParamsFunction("RANDOM", () => Random.NextDouble()));
            AddFunction(new Fun1PFunction("SIN", (v) => Math.Sin(v)));
            AddFunction(new Fun1PFunction("COS", (v) => Math.Cos(v)));
            AddFunction(new Fun1PFunction("TAN", (v) => Math.Tan(v)));
            AddFunction(new Fun1PFunction("ASIN", (v) => Math.Asin(v)));
            AddFunction(new Fun1PFunction("ACOS", (v) => Math.Acos(v)));
            AddFunction(new Fun1PFunction("ATAN", (v) => Math.Atan(v)));
            AddFunction(new Fun1PFunction("RAD", (v) => Math.PI * v / 180.0D));
            AddFunction(new Fun1PFunction("DEG", (v) => v * (180.0 / Math.PI)));
            AddFunction(new FunFunction("MAX", (vv) => vv.Max()));
            AddFunction(new FunFunction("MIN", (vv) => vv.Min()));
            AddFunction(new FunFunction("SUM", (vv) => vv.Sum()));
            AddFunction(new FunFunction("AVG", (vv) => vv.Average()));
            AddFunction(new Fun1PFunction("ABS", (v) => Math.Abs(v)));
            AddFunction(new Fun1PFunction("LN", v => Math.Log(v)));
            AddFunction(new Fun1PFunction("LOG10", v => Math.Log10(v)));
            AddFunction(new Fun2PFunction("LOG", (v, b) => Math.Log(v, b)));
            AddFunction(new Fun1PFunction("ROUND", (v) => Math.Round(v)));
            AddFunction(new Fun1PFunction("FLOOR", (v) => Math.Floor(v)));
            AddFunction(new Fun1PFunction("CEIL", (v) => Math.Ceiling(v)));
            AddFunction(new Fun1PFunction("SQRT", (v) => Math.Sqrt(v)));
            AddLazyFunction(new FunIf());
            AddLazyFunction(new FunIfBreak());
            AddLazyFunction(new FunLazyFunction("JOIN", (parms) =>
                new FunLazyNumberA(() =>
                {
                    List<double> joined = new List<double>();
                    parms.ForEach(n => joined.AddRange(n.EvalArray()));
                    return joined.ToArray();
                })
            ));
            AddLazyFunction(new FunLazyFunction("CONCAT", (parms) =>
            {
                FunLazyString tmp = new FunLazyString(() =>
                {
                    StringBuilder joined = new StringBuilder();
                    parms.ForEach(n => joined.Append(n.EvalString()));
                    return joined.ToString();
                });
                return tmp;
            }
            ));
            AddLazyFunction(new FunLazyFunction("SORT", (parms) =>
            {
                FunLazyNumberA tmp = new FunLazyNumberA(() =>
                {
                    if (parms.Count() == 0)
                        return new double[0];
                    if (parms.Count() == 1)
                        return parms[0].EvalArray().OrderBy(v => v).ToArray();
                    return parms.SelectMany(v => v.EvalArray()).OrderBy(d => d).ToArray();
                });
                return tmp;
            }
            ));
            AddLazyFunction(new FunLazyFunction("STRLEN", (parms) => new FunLazyNumberS(() => parms.Select(n => n.EvalString().Length).Sum())));
            AddLazyFunction(new FunLazyFunction("STRCMP", (parms) => new FunLazyNumberS(() => parms.All(n => n.EvalString().Equals(parms.First().EvalString(), StringComparison.InvariantCulture)))));
            AddLazyFunction(new FunLazyFunction("STRICMP", (parms) => new FunLazyNumberS(() => parms.All(n => n.EvalString().Equals(parms.First().EvalString(), StringComparison.InvariantCultureIgnoreCase)))));
            AddLazyFunction(new FunLazyFunction("IN", (parms) => new FunLazyNumberS(() =>
            {
                var parm0 = parms[0].EvalString();
                var oparms = parms.Skip(1);
                return oparms.Any(p => p.EvalString().Equals(parm0));
            })));
            AddLazyFunction(new RNGPERC());

            variables.Add("e", e);
            variables.Add("PI", PI);
            variables.Add("NaN", double.NaN);
            variables.Add("null", double.NaN);
            variables.Add("TRUE", 1.0D);
            variables.Add("FALSE", 0.0D);

        }

        #endregion

        #region shunting yard
        /**
         * Implementation of the <i>Shunting Yard</i> algorithm to transform an
         * infix expression to a RPN expression.
         */
        internal List<Token> ShuntingYard(string expression)
        {
            List<Token> outputQueue = new List<Token>();
            Stack<Token> stack = new Stack<Token>();

            Tokenizer tokenizer = new Tokenizer(this, expression);

            Token lastFunction = null;
            Token previousToken = null;
            foreach (Token token in tokenizer)
            {
                switch (token.type)
                {
                    case TokenType.STRINGPARAM:
                        //stack.Push(token);
                        outputQueue.Add(token);
                        break;
                    case TokenType.LITERAL:
                    case TokenType.HEX_LITERAL:
                    case TokenType.VARIABLE:
                        if (previousToken != null)
                        {
                            if (previousToken.type == TokenType.LITERAL || previousToken.type == TokenType.CLOSE_PAREN
                                || previousToken.type == TokenType.VARIABLE
                                || previousToken.type == TokenType.HEX_LITERAL)
                            {
                                // Implicit multiplication, e.g. 23(a+b) or (a+b)(a-b)
                                Token multiplication = new Token();
                                multiplication.Append("*");
                                multiplication.type = TokenType.OPERATOR;
                                stack.Push(multiplication);
                            }
                        }
                        outputQueue.Add(token);
                        break;
                    case TokenType.OBJPATH:
                        outputQueue.Add(token);
                        break;
                    case TokenType.FUNCTION:
                        stack.Push(token);
                        lastFunction = token;
                        break;
                    case TokenType.COMMA:
                        if (previousToken != null && previousToken.type == TokenType.OPERATOR)
                        {
                            throw new ExpressionException("Missing parameter(s) for operator " + previousToken +
                                    " at character position " + previousToken.pos);
                        }
                        while (!stack.IsEmpty() && stack.Peek().type != TokenType.OPEN_PAREN)
                        {
                            outputQueue.Add(stack.Pop());
                        }
                        if (stack.IsEmpty())
                        {
                            throw new ExpressionException("Parse error for function '"
                                    + lastFunction + "'");
                        }
                        break;
                    case TokenType.OPERATOR:
                        {
                            if (previousToken != null && (previousToken.type == TokenType.COMMA || previousToken.type == TokenType.OPEN_PAREN))
                            {
                                throw new ExpressionException("Missing parameter(s) for operator " + token +
                                        " at character position " + token.pos);
                            }
                            if (!operators.TryGetValue(token.surface, out Operator o1))
                            {
                                throw new ExpressionException("Unknown operator '" + token
                                        + "' at position " + (token.pos + 1));
                            }

                            ShuntOperators(outputQueue, stack, o1);
                            stack.Push(token);
                            break;
                        }
                    case TokenType.UNARY_OPERATOR:
                        {
                            if (previousToken != null && previousToken.type != TokenType.OPERATOR && previousToken.type != TokenType.UNARY_OPERATOR
                                    && previousToken.type != TokenType.COMMA && previousToken.type != TokenType.OPEN_PAREN)
                            {
                                throw new ExpressionException("Invalid position for unary operator " + token +
                                        " at character position " + token.pos);
                            }
                            Operator o1;
                            if (operators.ContainsKey(token.surface))
                            {
                                o1 = operators[token.surface];
                            }
                            else
                            {
                                throw new ExpressionException("Unknown unary operator '"
                                        + token.surface.Substring(0, token.surface.Length - 1)
                                        + "' at position " + (token.pos + 1));
                            }

                            ShuntOperators(outputQueue, stack, o1);
                            stack.Push(token);
                            break;
                        }
                    case TokenType.OPEN_PAREN:
                        if (previousToken != null)
                        {
                            if (previousToken.type == TokenType.LITERAL || previousToken.type == TokenType.CLOSE_PAREN
                                    || previousToken.type == TokenType.VARIABLE
                                    || previousToken.type == TokenType.HEX_LITERAL)
                            {
                                // Implicit multiplication, e.g. 23(a+b) or (a+b)(a-b)
                                Token multiplication = new Token();
                                multiplication.Append("*");
                                multiplication.type = TokenType.OPERATOR;
                                stack.Push(multiplication);
                            }
                            // if the ( is preceded by a valid function, then it
                            // denotes the start of a parameter list
                            if (previousToken.type == TokenType.FUNCTION)
                            {
                                outputQueue.Add(token);
                            }
                        }
                        stack.Push(token);
                        break;
                    case TokenType.CLOSE_PAREN:
                        if (previousToken != null && previousToken.type == TokenType.OPERATOR)
                        {
                            throw new ExpressionException("Missing parameter(s) for operator " + previousToken +
                                    " at character position " + previousToken.pos);
                        }
                        if (previousToken != null && previousToken.type == TokenType.COMMA)
                        {
                            throw new ExpressionException("Missing parameter after comma at character position " + previousToken.pos);
                        }
                        while (!stack.IsEmpty() && stack.Peek().type != TokenType.OPEN_PAREN)
                        {
                            outputQueue.Add(stack.Pop());
                        }
                        if (stack.IsEmpty())
                        {
                            throw new ExpressionException("Mismatched parentheses");
                        }
                        stack.Pop();
                        if (!stack.IsEmpty() && stack.Peek().type == TokenType.FUNCTION)
                        {
                            outputQueue.Add(stack.Pop());
                        }
                        break;
                }
                previousToken = token;
            }

            while (!stack.IsEmpty())
            {
                Token element = stack.Pop();
                if (element.type == TokenType.OPEN_PAREN || element.type == TokenType.CLOSE_PAREN)
                {
                    throw new ExpressionException("Mismatched parentheses");
                }
                outputQueue.Add(element);
            }
            return outputQueue;
        }

        private void ShuntOperators(List<Token> outputQueue, Stack<Token> stack, Operator o1)
        {
            Token nextToken = stack.IsEmpty() ? null : stack.Peek();
            while (nextToken != null &&
                    (nextToken.type == TokenType.OPERATOR || nextToken.type == TokenType.UNARY_OPERATOR)
                    && ((o1.IsLeftAssoc()
                        && o1.GetPrecedence() <= operators[nextToken.surface].GetPrecedence())
                        || (o1.GetPrecedence() < operators[nextToken.surface].GetPrecedence())))
            {
                outputQueue.Add(stack.Pop());
                nextToken = stack.IsEmpty() ? null : stack.Peek();
            }
        }

        internal virtual List<Token> GetRPN(string expression)
        {
            var RPN = ShuntingYard(expression);
            Validate(RPN);
            return RPN;
        }

        #endregion

        #region evaluators

        public virtual double EvalDouble(string expression)
        {
            return LazyEval(expression).Eval();
        }

        public virtual double[] EvalArray(string expression)
        {
            return LazyEval(expression).EvalArray();
        }

        public virtual string EvalString(string expression)
        {
            return LazyEval(expression).EvalString();
        }

        public long EvalInt(string expression)
        {
            return (long)Math.Round(EvalDouble(expression));
        }

        public bool EvalBool(string expression)
        {
            return EvalDouble(expression) != 0.0D;
        }

        private LazyNumber LazyEval(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ExpressionException("Empty expression");

            Stack<LazyNumber> stack = new Stack<LazyNumber>();

            foreach (Token token in GetRPN(expression))
            {
                switch (token.type)
                {
                    case TokenType.UNARY_OPERATOR:
                        LazyNumber value = stack.Pop();
                        stack.Push(operators[token.surface].Eval(value, null));
                        break;
                    case TokenType.OPERATOR:
                        LazyNumber v2 = stack.Pop();
                        LazyNumber v1 = stack.Pop();
                        stack.Push(operators[token.surface].Eval(v1, v2));
                        break;
                    case TokenType.VARIABLE:
                        LazyNumber lazyNumber;
                        if (variables.ContainsKey(token.surface))
                        {
                            lazyNumber = new FunLazyNumberS(() => variables[token.surface]);
                        }
                        else
                        {
                            if (vararrays.ContainsKey(token.surface))
                            {
                                lazyNumber = new FunLazyNumberA(() => vararrays[token.surface]);
                            }
                            else
                            {
                                if (stringVariables.ContainsKey(token.surface))
                                {
                                    lazyNumber = new FunLazyString(() => stringVariables[token.surface]);
                                }
                                else
                                {
                                    throw new UnknownVariableInExpressionException("Unknown variable: " + token.ToString(), token.ToString());
                                }
                            }
                        }
                        stack.Push(lazyNumber);
                        break;
                    case TokenType.OBJPATH:
                        if (!string.IsNullOrEmpty(token.surface))
                        {
                            LazyNumber avar = stack.Peek();
                            if (avar is FunLazyString)
                            {
                                var lazyjson = new FunLazyJSON((FunLazyString)avar)
                                {
                                    ObjectPath = token.surface
                                };
                                stack.Pop();
                                stack.Push(lazyjson);
                            }
                            else
                            {
                                throw new ExpressionException("Object path set to a variable that is not a JSON object");
                            }
                        }
                        else
                        {
                            throw new ExpressionException("Empty or invalid object path");
                        }
                        break;
                    case TokenType.FUNCTION:
                        bool isMap = false, isReduce = false;
                        string s = token.surface;
                        if (s[0] == MapChar)
                        {
                            s = s.Substring(1);
                            isMap = true;
                        }
                        if (s[0] == ReduceChar)
                        {
                            s = s.Substring(1);
                            isReduce = true;
                        }
                        LazyFunction f = functions[s];
                        List<LazyNumber> p = new List<LazyNumber>(
                                !f.NumParamsVaries() ? f.NumParams : 0);
                        // pop parameters off the stack until we hit the start of
                        // this function's parameter list
                        while (!stack.IsEmpty() && stack.Peek() != PARAMS_START)
                        {
                            p.Insert(0, stack.Pop());
                        }

                        if (stack.Peek() == PARAMS_START)
                        {
                            stack.Pop();
                        }

                        LazyNumber fResult;
                        if (isMap)
                        {
                            fResult = new FunLazyNumberA(() =>
                            {
                                double[] vals = p[0].EvalArray();
                                if (vals.Length == 0)
                                    return new double[0];
                                double[] res = new double[vals.Length];
                                int i = 0;
                                foreach (double val in vals)
                                {
                                    p[0] = new FunLazyNumberS(() => val);
                                    res[i++] = f.LazyEval(p).Eval();
                                }
                                return res;
                            });
                        }
                        else
                        {
                            if (isReduce)
                            {
                                fResult = new FunLazyNumberS(() =>
                                {
                                    double[] vals = p[0].EvalArray();
                                    if (vals.Length == 0)
                                        return 0.0D;
                                    if (vals.Length == 1)
                                        return vals[0];
                                    double res = 0.0D;
                                    List<LazyNumber> pred = new List<LazyNumber>(p.Count + 1)
                                    { // collection initialization ... ugly ....:)
                                        null,
                                        null
                                    };
                                    if (p.Count > 1)
                                        pred.AddRange(p.GetRange(1, p.Count - 1));
                                    pred[0] = new FunLazyNumberS(() => vals[0]);
                                    pred[1] = new FunLazyNumberS(() => vals[1]);
                                    res = f.LazyEval(pred).Eval();
                                    for (int i=2; i<vals.Length; i++)
                                    {
                                        pred[0] = new FunLazyNumberS(() => res);
                                        pred[1] = new FunLazyNumberS(() => vals[i]);
                                        res = f.LazyEval(pred).Eval();
                                    }
                                    return res;
                                });
                            }
                            else
                            {
                                fResult = f.LazyEval(p);
                            }
                        }
                        stack.Push(fResult);
                        break;
                    case TokenType.OPEN_PAREN:
                        stack.Push(PARAMS_START);
                        break;
                    case TokenType.LITERAL:
                        stack.Push(new FunLazyNumberS(() => StoD(token.surface)));
                        break;
                    case TokenType.STRINGPARAM:
                        FunLazyString tmpNumber = new FunLazyString(() => token.surface);
                        stack.Push(tmpNumber);
                        break;
                    case TokenType.HEX_LITERAL:
                        double hexd = double.NaN;
                        try
                        {
                            hexd = (double)Convert.ToInt64(token.surface, 16);
                        }
                        catch (Exception) { } // no need to worry, just NaN
                        stack.Push(new FunLazyNumberS(() => hexd));
                        break;
                }
            }
            return stack.Pop();
        }

        public void AddOperator(Operator op)
        {
            string key = op.GetOper();
            if (key.Length > MAX_OPERATOR_LENGTH)
                throw new ExpressionException(string.Format("Max operator length is {0} characters", MAX_OPERATOR_LENGTH));
            if (op is UnaryOperator)
            {
                key += "u";
            }
            operators.Add(key, op);
        }
        #endregion

        #region funcs and vars utils
        public void AddFunction(Function function)
        {
            functions[function.Name] = function;
        }

        public void AddLazyFunction(LazyFunction function)
        {
            functions[function.Name] = function;
        }

        public Expression SetDoubleVariable(string variable, double value)
        {
            variables[variable] = value;
            return this;
        }

        public Expression SetIntVariable(string variable, long value)
        {
            variables[variable] = value;
            return this;
        }

        public Expression SetStringVariable(string variable, string value)
        {
            stringVariables[variable] = value;
            return this;
        }

        public Expression SetArrayVariable(string variable, double[] value)
        {
            vararrays[variable] = value;
            return this;
        }

        public string GetStringVariable(string variable)
        {
            if (this.stringVariables.TryGetValue(variable, out string value))
                return value;
            return null;
        }

        public double GetDoubleVariable(string variable)
        {
            if (this.variables.TryGetValue(variable, out double value))
                return value;
            return double.NaN;
        }

        public IEnumerable<string> GetDeclaredVariables()
        {
            return variables.Keys;
        }

        public IEnumerable<string> GetDeclaredOperators()
        {
            return operators.Keys;
        }
        public bool OperatorExists(string op)
        {
            return this.operators.ContainsKey(op);
        }

        public IEnumerable<string> GetDeclaredFunctions()
        {
            return functions.Keys;
        }

        #endregion

        #region validator
        /**
         * Check that the expression has enough numbers and variables to fit the
         * requirements of the operators and functions, also check 
         * for only 1 result stored at the end of the evaluation.
         */
        internal void Validate(List<Token> rpn)
        {
            /*-
            * Thanks to Norman Ramsey:
            * http://http://stackoverflow.com/questions/789847/postfix-notation-validation
            */
            // each push on to this stack is a new function scope, with the value of each
            // layer on the stack being the count of the number of parameters in that scope
            Stack<int> stack = new Stack<int>();

            // push the 'global' scope
            stack.Push(0);

            foreach (Token token in rpn)
            {
                switch (token.type)
                {
                    case TokenType.UNARY_OPERATOR:
                        if (stack.Peek() < 1)
                        {
                            throw new ExpressionException("Missing parameter(s) for operator " + token);
                        }
                        break;
                    case TokenType.OPERATOR:
                        if (stack.Peek() < 2)
                        {
                            throw new ExpressionException("Missing parameter(s) for operator " + token);
                        }
                        // pop the operator's 2 parameters and add the result
                        int v = stack.Pop();
                        stack.Push(v - 2 + 1);
                        break;
                    case TokenType.FUNCTION:
                        bool isMap = false, isReduce = false;
                        string s = token.surface;
                        if (s[0] == '$')
                        {
                            isMap = true;
                            s = s.Substring(1);
                        }
                        if (s[0] == '@')
                        {
                            isReduce = true;
                            s = s.Substring(1);
                        }
                        if (!functions.TryGetValue(s, out LazyFunction tmplazyfun)) {
                            throw new UnknownFunctionInExpressionException(string.Format("Unknown function: {0}", s), s);
                        }

                        LazyFunction f = tmplazyfun;
                        if (f == null)
                        {
                            throw new ExpressionException("Unknown function '" + token + "' at position " + (token.pos + 1));
                        }
                        int numParams = stack.Pop();
                        if (isReduce)
                            numParams++;
                        if (!f.NumParamsVaries())
                        {
                            if (numParams != f.NumParams)
                            {
                                throw new ExpressionException("Function " + token + " expected " + f.NumParams + " parameters, got " + numParams);
                            }
                            // TODO check numero parametri della funzione mappata
                            if (isMap && numParams == 0)
                            {
                                throw new ExpressionException("Mapping requires at least one parameter, calling function " + token);
                            }
                            // TODO check numero parametri della funzione ridotta
                            if (isReduce && numParams < 2)
                            {
                                throw new ExpressionException("Reduce requires at least two parameters, calling function " + token);
                            }
                        }
                        if (stack.Count <= 0)
                        {
                            throw new ExpressionException("Too many function calls, maximum scope exceeded");
                        }
                        // push the result of the function
                        int val = stack.Pop();
                        stack.Push(val + 1);
                        break;
                    case TokenType.OPEN_PAREN:
                        stack.Push(0);
                        break;
                    case TokenType.OBJPATH:
                        break;
                    default:
                        val = stack.Pop();
                        stack.Push(val + 1);
                        break;
                }
            }

            if (stack.Count > 1)
            {
                throw new ExpressionException("Too many unhandled function parameter lists");
            }
            else if (stack.Peek() > 1)
            {
                throw new ExpressionException("Too many numbers or variables");
            }
            else if (stack.Peek() < 1)
            {
                throw new ExpressionException("Empty expression");
            }
        }

        #endregion

        #region static parser

        public ParseResult Parse(string strexpression)
        {
            if (string.IsNullOrEmpty(strexpression))
                return null;
            List<string> usedVars = new List<string>();
            var cleanexp = new StringBuilder();
            foreach (Token token in GetRPN(strexpression))
            {
                if (token.type == TokenType.VARIABLE && !usedVars.Contains(token.surface))
                    usedVars.Add(token.surface);
                cleanexp.Append(token.surface).Append(' ');
            }
            ParseResult res = new ParseResult()
            {
                DeclaredVariables = usedVars,
                CleanExpression = cleanexp.ToString().Trim()
            };
            return res;
        }

        #endregion

        #region clone

        public Expression Clone()
        {
            Expression e = new Expression()
            {
                operators = new Dictionary<string, Operator>(this.operators),
                functions = new Dictionary<string, LazyFunction>(this.functions),
                variables = new Dictionary<string, double>(this.variables),
                stringVariables = new Dictionary<string, string>(this.stringVariables),
                vararrays = new Dictionary<string, double[]>(this.vararrays)
            };
            return e;
        }

        #endregion

    }

    internal static class EvalExtensions
    {
        public static bool IsEmpty<T>(this Stack<T> s)
        {
            return s.Count == 0;
        }
    }
}
