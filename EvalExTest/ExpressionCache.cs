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
    public class ExpressionCache: EvalExTest
    {
        // RANDOM() is non deterministic, thus we (statistically) expect that subsequent calls produce different results,
        // while cached result should be always the same

        [TestMethod]
        public void TestCachedExpression()
        {
            CachingExpression cexpression = new CachingExpression();
            double rand = cexpression.EvalDouble("RANDOM()");
            Assert.AreEqual(rand, cexpression.EvalDouble("RANDOM()"));
            Assert.AreEqual(rand, cexpression.EvalDouble("RANDOM()"));
            Assert.AreEqual(rand, cexpression.EvalDouble("RANDOM()"));
            Assert.AreEqual(rand, cexpression.EvalDouble(" RANDOM()"));
            Assert.AreEqual(rand, cexpression.EvalDouble("RANDOM() "));
            Assert.AreEqual(rand, cexpression.EvalDouble("RANDOM( )"));
            Assert.AreEqual(rand, cexpression.EvalDouble(" RANDOM ( ) "));
            cexpression.SetIntVariable("a", 123);
            rand = cexpression.EvalDouble("RANDOM() + a");
            Assert.AreEqual(rand, cexpression.EvalDouble("RANDOM() + a"));
            cexpression.SetIntVariable("a", 999);
            Assert.AreNotEqual(rand, cexpression.EvalDouble("RANDOM() + a"));
        }

    }
}
