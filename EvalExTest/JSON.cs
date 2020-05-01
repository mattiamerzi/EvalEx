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
    public class JSON: EvalExTest
    {
        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestPropertyAccess(Expression expression)
        {
            expression.SetStringVariable("mystr", this.TestJsonObject);

            // standard properties
            Assert.AreEqual(123.45d, expression.EvalDouble("mystr.adouble"));
            Assert.AreEqual(37, expression.EvalInt("mystr.aint"));
            Assert.AreEqual("hello", expression.EvalString("mystr.astring"));
            CollectionAssert.AreEqual(new double[] { 1, 3, 5, 7 }, expression.EvalArray("mystr.aarray"));

            // nested properties
            Assert.AreEqual(54.321d, expression.EvalDouble("mystr.nested.ndouble"));
            Assert.AreEqual(73, expression.EvalInt("mystr.nested.nint"));
            Assert.AreEqual("world", expression.EvalString("mystr.nested.nstring"));
            Assert.ThrowsException<ExpressionException>(() => expression.EvalArray("mystr.nested.narray[2]"));
        }

        [DataTestMethod]
		[DynamicData(nameof(EvalExTest.GetExpression), typeof(EvalExTest), DynamicDataSourceType.Method)]
        public void TestJSONPath(Expression expression)
        {
            expression.SetStringVariable("mystr", this.TestJsonObject);
            Assert.AreEqual(3, expression.EvalInt("mystr.\"nested.narray[2]\""));

            expression.SetStringVariable("bigjson", TestBigJsonObject);
            Assert.AreEqual(50, expression.EvalInt("bigjson.\"$.Manufacturers[?(@.Name == 'Acme Co')].Products[0].Price\""));
            CollectionAssert.AreEqual(new double[] { 123d, 737d }, expression.EvalArray("bigjson.\"$..Products[?(@.Price >= 50)].ID\""));
            CollectionAssert.AreEqual(new double[] { 1, 3, 5, 2, 4, 6 }, expression.EvalArray("bigjson.\"$..Products[?(@.Price >= 50)].Deps\""));
            CollectionAssert.AreEqual(new double[] { 1, 2, 3, 4, 5, 6 }, expression.EvalArray("SORT(bigjson.\"$..Products[?(@.Price >= 50)].Deps\")"));
        }

    }
}
