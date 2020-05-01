using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvalEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EvalExTest
{
    [TestClass]
    public class Arrays: EvalExTest
    {
        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestArrayVar(Expression expression)
        {
            double[] arr = new double[] { 1.2d, 1.7d, 1.9d };
            expression.SetArrayVariable("arr", arr);
            CollectionAssert.AreEqual(arr, expression.EvalArray("arr"));
            Assert.AreEqual(arr.Length, expression.EvalInt("arr"));
            Assert.AreEqual((double)(arr.Length), expression.EvalDouble("arr"));
            Assert.AreEqual(arr.Length + 3, expression.EvalInt("arr + 3"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestArrayResult(Expression expression)
        {
            double[] arr = new double[] { 1.234d };
            expression.SetDoubleVariable("a", 1.234d);
            CollectionAssert.AreEqual(arr, expression.EvalArray("a"));

            string str = "1.0000000";
            double[] stra = new double[] { 1.0d };
            expression.SetStringVariable("s", str);
            CollectionAssert.AreEqual(stra, expression.EvalArray("s"));
        }

    }
}
