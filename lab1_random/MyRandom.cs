using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_random
{
    public class MyRandom
    {
        private long xn;

        private readonly long m;
        private readonly long a;
        private readonly long c;
        private readonly long x0;

        public MyRandom(long x0, long m, long a, long c)
        {
            this.x0 = x0;//5;
            this.m = m;//(long) (Math.Pow(2, 22) - 1);
            this.a = a;//(long) Math.Pow(9, 3);
            this.c = c;//33;
        }

        public void srand()
        {
            xn = x0;
        }

        public long random()
        {
            long res = xn;
            xn = (a*xn + c)%m;
            return xn;
        }
    }
}
