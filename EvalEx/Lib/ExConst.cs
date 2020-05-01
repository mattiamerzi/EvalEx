using System;
using System.Collections.Generic;
using System.Text;

namespace EvalEx.Lib
{
    internal class ExConst
    {
        internal const char NULL_CHAR = '\0';

        internal static double[] EMPTY_DOUBLE_ARRAY = { };

        internal const double PI = Math.PI;

        internal const double e = Math.E;

        internal const int MAX_OPERATOR_LENGTH = 3;

        private const string VarChars = "_~";

        private const string FirstPropertyChars = "_";
        private const string PropertyChars = "._-";

        internal const char DecimalSeparator = '.';

        internal const char MinusSign = '-';

        internal const char MapChar = '$';

        internal const char ReduceChar = '@';

        private const string FirstVarChars = "_";
        // TODO check that the cached / evaluated list of "used vars" trim out map / reduce characters.

        public static bool ValidVarStart(char ch)
        {
            return Char.IsLetter(ch) || FirstVarChars.IndexOf(ch) >= 0 || ch == MapChar || ch == ReduceChar;
        }
        public static bool ValidVarNameWith(string vname, char ch)
        {
            return ValidVarName(vname + ch);
        }
        public static bool ValidVarName(string vname)
        {
            if (vname.Length == 0)
                return false;
            int idx = 0;
            if (vname[0] == MapChar || vname[0] == ReduceChar)
                idx++;
            if (idx == 1 && vname.Length == 1) // map or reduce chars alone are NOT considered valid var names
                return false;
            if (!Char.IsLetter(vname[idx]) && FirstVarChars.IndexOf(vname[idx]) < 0)
                return false;
            for (int i = idx + 1; i < vname.Length; i++)
                if (!Char.IsLetterOrDigit(vname[i]) && VarChars.IndexOf(vname[i]) < 0)
                    return false;
            return true;
        }
        public static bool ValidPropertyWith(string prop, char ch)
        {
            return ValidProperty(prop + ch);
        }

        public static bool ValidProperty(string prop)
        {
            if (prop.Length == 0)
                return false;
            if (!Char.IsLetter(prop[0]) && FirstPropertyChars.IndexOf(prop[0]) < 0)
                return false;
            for (int i=1; i<prop.Length; i++)
            {
                if (!Char.IsLetterOrDigit(prop[i]) && PropertyChars.IndexOf(prop[i]) < 0)
                    return false;
            }
            return true;
        }
    }
}
