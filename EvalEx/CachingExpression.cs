using EvalEx.Lib;
using EvalEx.Lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;
using static EvalEx.Lib.ExConst;
using static EvalEx.Lib.ExConv;

namespace EvalEx
{

    public class CachingExpression: Expression
    {
        private static readonly Dictionary<string, List<string>> uvcache = new Dictionary<string, List<string>>();

        public List<string> UsedVars(string expression)
        {
            List<string> usedVars;
            if (uvcache.TryGetValue(expression, out usedVars))
                return usedVars;
            GetSignature(expression);
            if (uvcache.TryGetValue(expression, out usedVars))
                return usedVars;
            return new List<string>();
        }


        public string GetSignature(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return string.Empty;
            var parsedExp = Parse(expression);
            if (parsedExp == null)// || parsedExp.DeclaredVariables == null || parsedExp.DeclaredVariables.Count == 0)
                return expression;
            var usedVars = parsedExp.DeclaredVariables;
            StringBuilder sb = new StringBuilder(parsedExp.CleanExpression);
            usedVars.Sort();
            foreach (var v in usedVars)
            {
                if (variables.ContainsKey(v))
                {
                    AppendVar(sb, 'd', v, variables[v].ToString());
                }
                else
                {
                    if (vararrays.ContainsKey(v))
                    {
                        AppendVar(sb, 'a', v, AtoS(vararrays[v]));
                    }
                    else
                    {
                        if (stringVariables.ContainsKey(v))
                        {
                            AppendVar(sb, 's', v, stringVariables[v]);
                        }
                    }
                }
            }
            uvcache[parsedExp.CleanExpression] = usedVars;
            return sb.ToString();
        }

        private void AppendVar(StringBuilder sb, char dt, string name, string value)
        {
            sb.Append('[').Append(name).Append("]=[").Append(dt).Append("][").Append(value).Append(']');
        }

        private static readonly Dictionary<string, List<Token>> rpncache = new Dictionary<string, List<Token>>();
        internal override List<Token> GetRPN(string expression)
        {
            // the original expression is cached, because both Parse() and GetSignature() call GetRPN, and there would be an infinite recursion
            if (rpncache.TryGetValue(expression, out List<Token> res))
            {
                return res;
            }
            var RPN = ShuntingYard(expression);
            Validate(RPN);
            rpncache.Add(expression, RPN);
            return RPN;
        }

        private void RegisterExpressionBreakSignature(string signature)
        {
            if (!bkrcache.Contains(signature))
                bkrcache.Add(signature);
        }

        private static readonly HashSet<string> bkrcache = new HashSet<string>();
        private static readonly Dictionary<string, double> doubleresultscache = new Dictionary<string, double>();
        private static readonly Dictionary<string, string> stringresultscache = new Dictionary<string, string>();
        private static readonly Dictionary<string, double[]> arrayresultscache = new Dictionary<string, double[]>();

        private T GetCachedResult<T>(string expression, Dictionary<string, T> cache, Func<string, T> evaluator) {
            string signature = GetSignature(expression);
            if (cache.ContainsKey(signature))
            {
                return cache[signature];
            }
            if (bkrcache.Contains(signature))
                throw new ExpressionBreakException();
            try
            {
                T t = evaluator.Invoke(expression);
                return cache[signature] = t;
            }
            catch (ExpressionBreakException)
            {
                RegisterExpressionBreakSignature(expression);
                throw;
            }
        }

        public override double EvalDouble(string expression)
        {
            return GetCachedResult<double>(expression, doubleresultscache, (e) => base.EvalDouble(e));
        }

        public override double[] EvalArray(string expression)
        {
            return GetCachedResult<double[]>(expression, arrayresultscache, (e) => base.EvalArray(e));
        }

        public override string EvalString(string expression)
        {
            return GetCachedResult<string>(expression, stringresultscache, (e) => base.EvalString(e));
        }

        public bool HasUnknownVars(string expression)
        {
            List<string> usedVars = UsedVars(expression);
            foreach (var usedVar in usedVars)
            {
                if (!(stringVariables.ContainsKey(usedVar) || variables.ContainsKey(usedVar) || vararrays.ContainsKey(usedVar)))
                    return true;
            }
            return false;
        }

        public bool ThrowsBreak(string signature)
        {
            return bkrcache.Contains(signature);
        }
    }
 }
