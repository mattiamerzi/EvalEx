using System;
using System.Collections.Generic;
using EvalEx;
using EvalEx.Lib;
using EvalEx.Lib.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EvalExTest
{
    [TestClass]
    public class CaseSensitive: EvalExTest
    {

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void VariableIsCaseSensitive(Expression expression)
        {
            expression.SetIntVariable("A", 20);
            Assert.ThrowsException<UnknownVariableInExpressionException>(() => expression.EvalInt("a"));
            Assert.AreEqual(20, expression.EvalInt("A"));

            expression.SetIntVariable("a", 10);
            Assert.AreEqual(40, expression.EvalInt("A+A"));
            Assert.AreEqual(30, expression.EvalInt("A+a"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void FunctionCaseSensitive(Expression expression)
            {
                //"a+testsum(1,3)");
                expression.SetIntVariable("A", 1);
                expression.SetIntVariable("B", 1);
                expression.AddFunction(new TestSum());
                Assert.ThrowsException<UnknownFunctionInExpressionException>(() => expression.EvalInt("testsum(A,B)") == 2);
                Assert.IsTrue(expression.EvalInt("TESTSUM(A,B)") == 2);
        }

    }
}
