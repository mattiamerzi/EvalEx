using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EvalEx.Lib
{
    public class ExConv
    {
        public static string AtoS(double[] da)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.Append('[');
            foreach (double d in da)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(DtoS(d));
                i++;
            }
            sb.Append(']');
            return sb.ToString();
        }

        public static string DtoS(double d)
        {
            if (Double.IsNaN(d))
                return "NaN";
            string s = d.ToString("F2", CultureInfo.InvariantCulture);
            return s;
        }

        public static double StoD(string s)
        {
            if (string.IsNullOrEmpty(s))
                return double.NaN;
            if (s.Equals("NaN", StringComparison.OrdinalIgnoreCase))
                return double.NaN;
            while (s.Length > 0 && s[0] == '0' && s != "0")
                s = s.Substring(1);
            if (string.IsNullOrEmpty(s))
                return double.NaN;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                return result;
            return double.NaN;
        }

        public static object StoW(string s)
        {
            double d = StoD(s);
            if (double.IsNaN(d))
                return s;
            return d;
        }

    }
}
