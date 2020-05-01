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
    public class EdgeCases: EvalExTest
    {
        [DataTestMethod]
        [DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestPowers(Expression expression)
        {
            expression.SetDoubleVariable("x", 2);
            expression.SetDoubleVariable("y", 3);
            Assert.AreEqual(8, expression.EvalInt("x^y"));

            expression.SetDoubleVariable("z", 81);
            expression.SetDoubleVariable("w", 0.75);
            Assert.AreEqual(27, expression.EvalInt("81^0.75"));
            Assert.AreEqual(27, expression.EvalInt("z^w"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestIFBRK(Expression expression)
        {
            Assert.ThrowsException<ExpressionBreakException>(() => expression.EvalDouble("IFBRK(3>5, 1.0)"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestDivisionByZero(Expression expression)
        {
            Assert.AreEqual(double.PositiveInfinity, expression.EvalDouble("3/0"));
            Assert.AreEqual(double.NegativeInfinity, expression.EvalDouble("-3/0"));
        }
    }
}
