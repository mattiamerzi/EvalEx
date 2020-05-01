using EvalEx;
using EvalEx.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvalExTest
{
    public class EvalExTest
    {

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
        }


        [TestInitialize]
        public void TestInitialize()
        {
        }

        public static IEnumerable<object[]> GetExpression()
        {
            yield return new object[] { new Expression() }; // test on a new object
            yield return new object[] { new CachingExpression() }; // test on a caching expression
        }

        protected class TestSquare : Function
        {
            public TestSquare() : base("TESTSQUARE", 1) { }

            public override double Eval(List<double> parameters)
            {
                return parameters[0] * parameters[0];
            }
        }

        protected class TestSum : Function
        {
            public TestSum() : base("TESTSUM", 2) { }

            public override double Eval(List<double> parameters)
            {
                return parameters[0] + parameters[1];
            }
        }

        protected class Test3Sum : Function
        {
            public Test3Sum() : base("TEST3SUM", 3) { }

            public override double Eval(List<double> parameters)
            {
                return parameters[0] + parameters[1] + parameters[2];
            }
        }

        protected string TestJsonObject =
            new JObject(
                new JProperty("adouble", 123.45d),
                new JProperty("aint", 37),
                new JProperty("astring", "hello"),
                new JProperty("aarray", new JArray(new double[] { 1, 3, 5, 7 })),
                new JProperty("nested",
                    new JObject(
                        new JProperty("ndouble", 54.321d),
                        new JProperty("nint", 73),
                        new JProperty("nstring", "world"),
                        new JProperty("narray", new JArray(new double[] { 7, 5, 3, 1 }))
                        )
                    )
            ).ToString();

        protected string TestBigJsonObject = @"{
  'Stores': [
    'Lambton Quay',
    'Willis Street'
  ],
  'Manufacturers': [
    {
      'Name': 'Acme Co',
      'Products': [
        {
          'ID': 123,
		  'Deps': [ 1, 3, 5 ],
          'Name': 'Anvil',
          'Price': 50
        }
      ]
    },
    {
      'Name': 'Contoso',
      'Products': [
        {
          'ID': 737,
		  'Deps': [ 2, 4, 6 ],
          'Name': 'Elbow Grease',
          'Price': 99.95
        },
        {
          'ID': 999,
		  'Deps': [ 7, 8, 9 ],
          'Name': 'Headlight Fluid',
          'Price': 4
        }
      ]
    }
  ]
}";
    }
}
