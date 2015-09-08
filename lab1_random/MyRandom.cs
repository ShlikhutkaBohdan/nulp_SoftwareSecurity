using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_random
{
    public class MyRandom
    {
        private static long xn;

        private static readonly long m;
        private static readonly long a;
        private static readonly long c;
        private static readonly long x0;

        static MyRandom()
        {
            x0 = 5;
            m = (long) (Math.Pow(2, 22) - 1);
            a = (long) Math.Pow(9, 3);
            c = 33;
        }

        public static void srand()
        {
            xn = x0;
        }

        public static long random()
        {
            long res = xn;
            xn = (a*xn + c)%m;
            return xn;
        }
    }
}
