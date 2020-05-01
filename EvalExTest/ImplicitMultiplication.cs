using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvalEx;
using EvalEx.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace EvalExTest
{
    [TestClass]
    public class ImplicitMultiplication: EvalExTest
    {
        // generally speaking, implicit multiplication is evil, and should be avoided
        // especially using blank (a b) == (a*b) or after a parenthesis (a)5 => 5(a) => 5a
        // TODO: we should add a configuration parameter to enable or disable implicit multiplication

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestImplicitMultiplicationVarsNoParenthesis(Expression expression)
        {
            expression.SetIntVariable("a", 5);
            expression.SetIntVariable("b", 3);
            Assert.AreEqual(15, expression.EvalInt("a b"));
            Assert.AreEqual(15, expression.EvalInt("5b"));
            Assert.ThrowsException<UnknownVariableInExpressionException>(() => expression.EvalInt("a3"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestImplicitMultiplicationConstSingleParenthesis(Expression expression)
        {
            Assert.AreEqual(15, expression.EvalInt("3(5)"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestImplicitMultiplicationConstDoubleParenthesis(Expression expression)
        {
            Assert.AreEqual(15, expression.EvalInt("(3)(5)"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestImplicitMultiplicationVarsSingleParenthesis(Expression expression)
        {
            expression.SetIntVariable("a", 5);
            expression.SetIntVariable("b", 3);
            expression.SetIntVariable("c", 3);
            Assert.AreEqual(15, expression.EvalInt("3(a)"));
            Assert.AreEqual(15, expression.EvalInt("(a)b"));
            Assert.AreEqual(45, expression.EvalInt("(a b c)"));
            Assert.AreEqual(270, expression.EvalInt("(a 2 b 3 c)"));
            // implicit multiplication here could be misleading (what if I have a function named 'b'?!)
            Assert.ThrowsException<UnknownFunctionInExpressionException>(() => expression.EvalInt("b(a)"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestImplicitMultiplicationVarsDoubleParenthesis(Expression expression)
        {
            expression.SetIntVariable("a", 5);
            expression.SetIntVariable("b", 3);
            Assert.AreEqual(15, expression.EvalInt("(a)(b)"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestImplicitMultiplicationExpressionSingleParenthesis(Expression expression)
        {
            expression.SetIntVariable("a", 5);
            expression.SetIntVariable("b", 3);
            Assert.AreEqual(40, expression.EvalInt("5(a+b)"));
            Assert.AreEqual(40, expression.EvalInt("(a+b)5"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestImplicitMultiplicationExpressionDoubleParenthesis(Expression expression)
        {
            expression.SetIntVariable("a", 1);
            expression.SetIntVariable("b", 2);
            expression.SetIntVariable("c", 4);
            expression.SetIntVariable("d", 1);
            Assert.AreEqual(9, expression.EvalInt("(a+b)(c-d)"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestImplicitMultiplicationExpressionTripleParenthesis(Expression expression)
        {
            expression.SetIntVariable("a", 1);
            expression.SetIntVariable("b", 2);
            expression.SetIntVariable("c", 4);
            expression.SetIntVariable("d", 1);
            expression.SetIntVariable("e", 2);
            expression.SetIntVariable("f", 1);
            expression.SetIntVariable("g", 2);
            Assert.AreEqual(9, expression.EvalInt("(a+b)(c-d)(e+f-g)"));
        }

    }
}

