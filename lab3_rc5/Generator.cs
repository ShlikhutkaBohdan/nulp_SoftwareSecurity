using System;
using System.Collections.Generic;

namespace RC5_Encryption
{
    internal class Generator
    {
        private static int _m = (int) (Math.Pow(2, 19) - 1);
        private static int _a = (int) Math.Pow(6, 3);
        private static int _c = 55;
        private readonly int _x0 = 1024;
        private readonly List<int> _randomNumberList;

        internal int Period
        {
            get { return _randomNumberList.IndexOf(_x0, 1, _randomNumberList.Count - 1); }
        }

        internal Generator(int m, int a, int c, int x0)
        {
            _m = m;
            _a = a;
            _c = c;
            _x0 = x0;

            _randomNumberList = new List<int> { _x0 };
        }

        internal List<int> GetRandomNumberList(int count)
        {
            for (var i = 0; i < count - 1; ++i)
            {
                var x = _randomNumberList[i];
                var randomNumber = GetRandomNumber(x);
                _randomNumberList.Add(randomNumber);
            }

            return _randomNumberList;
        }

        private static int GetRandomNumber(int x)
        {
            var nextX = (_a * x + _c) % _m;
            return nextX;
        }

        internal static int Next()
        {
            var nextX = (_a * DateTime.Now.Millisecond + _c) % _m;
            return nextX;
        }
    }
}
