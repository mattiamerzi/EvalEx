using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace EvalEx.Lib
{
    internal class FunLazyJSON: LazyNumber
    {
        public string ObjectPath { get; set; }
        private FunLazyString lazyString;
        public FunLazyJSON(FunLazyString lazyString)
        {
            this.lazyString = lazyString;
        }

        private JObject EvalJObject()
        {
            try
            {
                return JObject.Parse(this.lazyString.EvalString());
            }
            catch (Exception)
            {
                throw new ExpressionException(string.Format("Trying to obtain path {0} from a string that is not a valid json object", this.ObjectPath));
            }
        }

        public override double Eval()
        {
            try
            {
                return EvalJObject()?.SelectToken(ObjectPath)?.ToObject<double>() ?? double.NaN;
            } catch (Exception ex)
            {
                throw new ExpressionException(string.Format("Error Trying to obtain value from path {0}: {1}", this.ObjectPath, ex.Message));
            }
        }

        public override double[] EvalArray()
        {
            var jobj = EvalJObject();
            if (jobj == null)
                return new double[0];
            IEnumerable<JToken> selectedTokens = jobj.SelectTokens(ObjectPath);
            if (selectedTokens == null)
                return new double[0];
            List<double> res = new List<double>();
            foreach (var jtoken in selectedTokens)
            {
                if (jtoken is JArray)
                    res.AddRange(jtoken?.ToObject<double[]>() ?? new double[0]);
                else
                    res.Add(jtoken?.ToObject<double>() ?? double.NaN);
            }
            return res.ToArray();
        }

        public override string EvalString()
        {
            try {
                return EvalJObject()?.SelectToken(ObjectPath)?.ToString() ?? string.Empty;
            } catch (Exception ex)
            {
                throw new ExpressionException(string.Format("Error Trying to obtain value from path {0}: {1}", this.ObjectPath, ex.Message));
            }
        }
    }
}
