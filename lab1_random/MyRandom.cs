using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_random
{
    public class MyRandom
    {
        private ulong xn;

        private readonly ulong m;
        private readonly ulong a;
        private readonly ulong c;
        private readonly ulong x0;

        public MyRandom(ulong x0, ulong m, ulong a, ulong c)
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


        public ulong random()
        {
            xn = (a*xn + c)%m;
            return xn;
        }
    }
}
