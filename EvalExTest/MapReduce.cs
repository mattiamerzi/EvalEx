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
    public class MapReduce: EvalExTest
    {
        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestMap(Expression expression)
        {
            double[] arr = new double[] { 1d, 2d, 3d };
            double[] expected = new double[] { 1d, 4d, 9d };
            expression.AddLazyFunction(new TestSquare());
            expression.SetArrayVariable("arr", arr);
            CollectionAssert.AreEqual(expected, expression.EvalArray("$TESTSQUARE(arr)"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestReduce(Expression expression)
        {
            double[] rarr1 = new double[] { 1d };
            double[] rarr2 = new double[] { 1d, 4d };
            double[] rarr3 = new double[] { 1d, 4d, 7d };
            double[] rarr4 = new double[] { 1d, 4d, 7d, 11d };
            expression.AddLazyFunction(new TestSum());
            expression.AddLazyFunction(new Test3Sum());
            expression.SetArrayVariable("rarr1", rarr1);
            expression.SetArrayVariable("rarr2", rarr2);
            expression.SetArrayVariable("rarr3", rarr3);
            expression.SetArrayVariable("rarr4", rarr4);

            Assert.AreEqual(1d, expression.EvalDouble("@TESTSUM(rarr1)"));
            Assert.AreEqual(5d, expression.EvalDouble("@TESTSUM(rarr2)"));
            Assert.AreEqual(12d, expression.EvalDouble("@TESTSUM(rarr3)"));
            Assert.AreEqual(23d, expression.EvalDouble("@TESTSUM(rarr4)"));

            Assert.AreEqual(1d, expression.EvalDouble("@TEST3SUM(rarr1, 100)"));
            Assert.AreEqual(105d, expression.EvalDouble("@TEST3SUM(rarr2, 100)"));
            Assert.AreEqual(212d, expression.EvalDouble("@TEST3SUM(rarr3, 100)"));
            Assert.AreEqual(323d, expression.EvalDouble("@TEST3SUM(rarr4, 100)"));

            Assert.AreEqual(10d, expression.EvalDouble("@TESTSUM(rarr1)* 10"));
            Assert.AreEqual(50d, expression.EvalDouble("@TESTSUM(rarr2)* 10"));
            Assert.AreEqual(120d, expression.EvalDouble("@TESTSUM(rarr3)* 10"));
            Assert.AreEqual(230d, expression.EvalDouble("@TESTSUM(rarr4)* 10"));

            Assert.AreEqual(10d, expression.EvalDouble("@TEST3SUM(rarr1, 100)* 10"));
            Assert.AreEqual(1050d, expression.EvalDouble("@TEST3SUM(rarr2, 100)* 10"));
            Assert.AreEqual(2120d, expression.EvalDouble("@TEST3SUM(rarr3, 100)* 10"));
            Assert.AreEqual(3230d, expression.EvalDouble("@TEST3SUM(rarr4, 100)* 10"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestErrors(Expression expression)
        {
            double[] arr = new double[] { 1d, 4d, 9d };
            expression.SetArrayVariable("arr", arr);
            Assert.ThrowsException<ExpressionException>(() => expression.EvalDouble("$"));
            Assert.ThrowsException<ExpressionException>(() => expression.EvalDouble("@"));
            Assert.ThrowsException<ExpressionException>(() => expression.EvalDouble("@27(arr)"));
            Assert.ThrowsException<ExpressionException>(() => expression.EvalDouble("@arr+13"));
            Assert.ThrowsException<ExpressionException>(() => expression.EvalDouble("$27(arr)"));
            Assert.ThrowsException<ExpressionException>(() => expression.EvalDouble("$arr+13"));
        }

    }
}
