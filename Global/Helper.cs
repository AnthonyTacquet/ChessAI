using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global
{
    public static class Helper
    {
        public static bool IsGreaterThan(this float val1, float val2)
        {
            return val1 > val2;
        }

        public static bool IsLessThan(this float val1, float val2)
        {
            return val1 < val2;
        }
    }

}
