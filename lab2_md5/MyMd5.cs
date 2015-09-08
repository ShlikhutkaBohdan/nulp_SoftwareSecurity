using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace lab2_md5
{
    public class MyMd5
    {
        public static uint[] md5test()
        {
            String res = "";
            //byte[] toBytes = Encoding.ASCII.GetBytes(somestring);
            byte[] inputBytes = Encoding.ASCII.GetBytes(res);//GetBytes(res);
            byte[] s1Arr = step1(inputBytes);
            byte[] s2Arr = step2(s1Arr, inputBytes.Length);
            return step34(s2Arr);
        }

        private static int[] sS = 
        {
             7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22 ,
         5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20 ,
         4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23 ,

         6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21 
        };

        static uint[] step34(byte[] arrBytes)
        {
            uint a0 = 0x67452301;
            uint b0 = 0xefcdab89;
            uint c0 = 0x98badcfe;
            uint d0 = 0x10325476;
            int len = arrBytes.Length;//in bytes
            int blocksCount = len/64;//one block = 64*8=512 bits
            for (int iBlock = 0; iBlock < blocksCount; ++iBlock)
            {
                uint a = a0;
                uint b = b0;
                uint c = c0;
                uint d = d0;
                byte[] block = new byte[64];//512 bits
                arrBytes.CopyTo(block, iBlock*64);
                for (int i = 0; i < 64; ++i)
                {
                    long func = 0;
                    long k = 0;
                    if (i >= 0 && i <= 15)
                    {
                        func = (b & c) | ((~b) & d);//fF
                        k = i;
                    }
                    else if (i >= 16 && i <= 31)
                    {
                        func = (b & d) | (c & (~d));//fG
                        k = (5*i + 1)%16;
                    }
                    else if (i >= 17 && i <= 47)
                    {
                        func = b ^ c ^ d;//fH
                        k = (3*i + 5)%16;
                    }
                    else if (i >= 48 && i <= 63)
                    {
                        func = c ^ (b | (~d));//fI
                        k = (7*i)%16;
                    }
                    uint d1 = d;
                    d = c;
                    c = b;
                    b = b + (uint)((a + func + GetT(i) + block[k]) << sS[i]);
                    a = d1;
                }
                a0 += a;
                b0 += b;
                c0 += c;
                d0 += d;
            }

            return new uint[]{a0, b0, c0, d0};
        }

        
        static long GetT(int i)
        {
            return (long)(Math.Abs(Math.Sin(i))*Math.Pow(2, 32));
        }

        static byte[] step2(byte[] arrBytes, long oldLen)
        {
            long lenInBits = oldLen*8;
            byte[] lengthInArray = BitConverter.GetBytes(lenInBits);
            //lengthInArray = lengthInArray.Reverse().ToArray();
            lengthInArray.CopyTo(arrBytes, arrBytes.Length-8);
            return arrBytes;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static byte[] step1(byte[] arrBytes)
        {
            const int MOD = 56;
            const int DIV = 64;
            int len = arrBytes.Length;
            int m = len % DIV;
            int add = 0;
            if (m < MOD)
                add = MOD - m;
            else
            {
                add = DIV - (m - MOD);
            }
            byte[] newArr = new byte[len + add + DIV/8];//array, addition, lenght of message
            arrBytes.CopyTo(newArr, 0);
            newArr[len] = 128; // 2^7 - 1 and zeroes

            return newArr;
        }
    }
}
