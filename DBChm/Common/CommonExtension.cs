using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCHM
{
    public static class CommonExtension
    {
        public static string FormatString(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
    }
}
