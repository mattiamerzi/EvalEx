using EvalEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvalExTest
{

    [TestClass]
    public class Unary: EvalExTest
    {

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestUnary(Expression expression)
        {
            Assert.AreEqual(-1, expression.EvalInt("---1"));
            Assert.AreEqual(1, expression.EvalInt("+++1"));
            Assert.AreEqual(1, expression.EvalInt("++1"));
            Assert.AreEqual(1, expression.EvalInt("--1"));
            Assert.AreEqual(-1, expression.EvalInt("+-1"));
            Assert.AreEqual(-1, expression.EvalInt("-+1"));
            Assert.AreEqual(0, expression.EvalInt("1-+1"));
            Assert.AreEqual(-1, expression.EvalInt("-+---+++--++-1"));
            Assert.AreEqual(-2, expression.EvalInt("1--++++---2+-+----1"));
        }
    }
}
