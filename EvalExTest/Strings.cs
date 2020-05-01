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
    public class Strings: EvalExTest
    {
        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestLiteral(Expression expression)
        {
            Assert.AreEqual("hello world", expression.EvalString(@"CONCAT(""hello "", ""world"")"));
            Assert.AreEqual(@"double""quote", expression.EvalString(@"""double\""quote"""));
            Assert.AreEqual(@"""", expression.EvalString(@"""\"""""));
            Assert.AreEqual(@"""""", expression.EvalString(@"""\""\"""""));
            Assert.AreEqual(@"\""\""", expression.EvalString(@"""\\""\\"""""));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestStringification(Expression expression)
        {
            expression.SetDoubleVariable("d", 123.45);
            expression.SetIntVariable("i", 123);
            expression.SetArrayVariable("a", new double[] { 123.45111, 54.31999 });
            Assert.AreEqual("123.45", expression.EvalString("d"));
            Assert.AreEqual("123.00", expression.EvalString("i"));
            Assert.AreEqual("[123.45,54.32]", expression.EvalString("a"));
        }

    }
}
