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
    public class LazyIf: EvalExTest
    {
        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestLazyIf(Expression expression)
        {
            Assert.AreEqual(1, expression.EvalInt("IF(3<5, 1, 3/0)"));
        }
    }
}
