using System;
using System.Collections.Generic;
using System.Text;

namespace ExecuteSQLStatementsFromFIle
{
    internal class General
    {
        internal static string OneLine(string s)
        {
            // keep logs readable
            var t = s.Replace("\r", " ").Replace("\n", " ").Trim();
            return t.Length > 200 ? t.Substring(0, 200) + "..." : t;
        }
    }
}
