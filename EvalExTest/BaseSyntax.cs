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
    public class BaseSyntax: EvalExTest
    {

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestSyntaxErrors(Expression expression)
        {
            Assert.ThrowsException<ExpressionException>(() => expression.EvalDouble("AVG(1,2,3,)"));
            Assert.ThrowsException<ExpressionException>(() => expression.EvalDouble("^AVG(1,2,3)"));
        }

    }
}
