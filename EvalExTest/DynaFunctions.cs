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
    public class DynaFunctions: EvalExTest
    {

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestSimple(Expression expression)
        {
            expression.AddLazyFunction(new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float => SQRT( leg1^2 + leg2^2 )").AsLazyFunction());
			Assert.AreEqual(5, expression.EvalDouble("PITAGORA(3,4)"));

            expression.AddLazyFunction(new DynaFunction("NOPARAMS(): Float => 37").AsLazyFunction());
			Assert.AreEqual(37, expression.EvalDouble("NOPARAMS()"));
		}

        [TestMethod]
        public void TestInvalid()
        {
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: , leg2: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, : Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float leg2: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1 Float, leg2: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2 Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float: Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float) Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float > SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float = SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORAleg1: Float, leg2: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1- Float, leg2: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2? Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Flo&at, leg2: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, le*2: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: foo, leg2: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): bar => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float hello, leg2: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2 world: Float): Float => SQRT( leg1^2 + leg2^2)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(bummer): Float => SQRT( leg1^2 + leg2^2)"));

            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float => SQRT( leg1^2 + leg2^2"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float => SQRT()"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float => "));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float =>"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float ="));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Float "));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): Flo"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float): "));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float):"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float)"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Float"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: Fl"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2: "));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2:"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, leg2"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float, "));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float,"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Float"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: Flo"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1: "));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1:"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(leg1"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA(le"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA("));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction("PITAGORA"));
            Assert.ThrowsException<ExpressionException>(() => new DynaFunction(""));
		}

	}
}
