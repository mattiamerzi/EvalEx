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
    public class ScientificNotation: EvalExTest
    {

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestSimple(Expression expression)
        {
			Assert.AreEqual(10000000000d, expression.EvalDouble("1e10"));
			Assert.AreEqual(10000000000d, expression.EvalDouble("1e+10"));
			Assert.AreEqual(10000000000d, expression.EvalDouble("1E10"));
			Assert.AreEqual(10000000000d, expression.EvalDouble("1E+10"));
			Assert.AreEqual(1234567d, expression.EvalDouble("123.4567E4"));
			Assert.AreEqual(2.5d, expression.EvalDouble("2.5e0"));
		}

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestNegative(Expression expression)
        {
			Assert.AreEqual(0.0000000001d, expression.EvalDouble("1e-10"));
			Assert.AreEqual(0.0000000001d, expression.EvalDouble("1E-10"));
			Assert.AreEqual(0.2135d, expression.EvalDouble("2135E-4"));
		}

		[DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
		public void TestOthers(Expression expression)
		{
			Assert.AreEqual(12345d, expression.EvalDouble("SQRT(152.399025e6)"));
			Assert.AreEqual(300, expression.EvalDouble("3.e2"));
			Assert.IsTrue(double.IsNaN(expression.EvalDouble("123e4.5"))); // invalid decimal exponent means "invalid string to double" => NaN
		}

	}
}
