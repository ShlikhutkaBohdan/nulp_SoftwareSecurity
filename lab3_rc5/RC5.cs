using System;
using System.Collections.Generic;
using System.IO;

namespace RC5_Encryption
{
    internal static class Rc5
    {
        private const byte W = 8;
        private const byte W2 = W * 2;
        private const byte Rounds = 16;
        private const ulong Pw = 0xb7e151628aed2a6b;
        private const ulong Qw = 0x9e3779b97f4a7c15;

        internal static void Encrypt(byte[] key, Stream input, Stream output)
        {
            var s = Keys(key);

            ulong a0 = 0, b0 = 0;
            var a = (ulong)Generator.Next();
            var b = (ulong)Generator.Next();

            var inBuffer = new byte[W2];
            input.Position = 0;
            output.Position = 0;

            EncodeBlock(ref a0, ref b0, a, b, s, output);

            while (input.Position < input.Length)
            {
                var readed = input.Read(inBuffer, 0, W2);
                if (readed < W2)
                {
                    Filling(inBuffer, readed, W2 - readed);
                    Buffer.BlockCopy(BitConverter.GetBytes(W2 - readed), 0, inBuffer, W2 - 1, 1);
                }
                a = BitConverter.ToUInt64(inBuffer, 0);
                b = BitConverter.ToUInt64(inBuffer, W);
                EncodeBlock(ref a0, ref b0, a, b, s, output);
            }
        }

        internal static void Decrypt(byte[] key, Stream input, Stream output)
        {
            var s = Keys(key);

            ulong a0 = 0, b0 = 0;
            var inBuffer = new byte[W2];
            input.Position = 0;
            output.Position = 0;
            input.Read(inBuffer, 0, W2);
            var a = BitConverter.ToUInt64(inBuffer, 0);
            var b = BitConverter.ToUInt64(inBuffer, W);
            DecodeBlock(ref a0, ref b0, ref a, ref b, s, output, false);
            while (input.Position < input.Length - W2)
            {
                input.Read(inBuffer, 0, W2);
                a = BitConverter.ToUInt64(inBuffer, 0);
                b = BitConverter.ToUInt64(inBuffer, W);
                DecodeBlock(ref a0, ref b0, ref a, ref b, s, output, true);
            }
        }

        internal static void Filling(Array a, int offset, int count)
        {
            for (var i = 0; i < count; i++)
                (a as dynamic)[offset + i] = (byte)0;
        }

        private static void EncodeBlock(ref ulong a0, ref ulong b0, ulong a, ulong b, IList<ulong> s, Stream output)
        {
            a ^= a0;
            b ^= b0;

            a += s[0];
            b += s[1];
            for (var i = 1; i <= Rounds; i++)
            {
                a = RotateLeft(a ^ b, (int)b) + s[2 * i];
                b = RotateLeft(b ^ a, (int)a) + s[2 * i + 1];
            }
            a0 = a;
            b0 = b;
            output.Write(BitConverter.GetBytes(a), 0, W);
            output.Write(BitConverter.GetBytes(b), 0, W);
        }

        private static void DecodeBlock(ref ulong a0, ref ulong b0, ref ulong a, ref ulong b, IList<ulong> s, Stream output, bool writeFlag)
        {
            var tmpA = a;
            var tmpB = b;

            for (var i = Rounds; i > 0; i--)
            {
                b = RotateRight(b - s[2 * i + 1], (int)a) ^ a;
                a = RotateRight(a - s[2 * i], (int)b) ^ b;
            }
            b -= s[1];
            a -= s[0];
            a ^= a0;
            b ^= b0;

            a0 = tmpA;
            b0 = tmpB;
            if (!writeFlag) return;
            output.Write(BitConverter.GetBytes(a), 0, W);
            output.Write(BitConverter.GetBytes(b), 0, W);
        }

        private static ulong[] Keys(byte[] key)
        {
            ulong aa = 0, bb = 0, i, j;
            var b = key.Length;
            var c = (ulong)(b / W);
            var l = new ulong[c];
            Buffer.BlockCopy(key, 0, l, 0, b);

            const ulong t = 2 * Rounds + 2;
            var s = new ulong[t];
            s[0] = Pw;
            for (i = 1; i < t; i++)
                s[i] = s[i - 1] + Qw;

            i = j = 0;
            for (ulong k = 0; k < 3 * Math.Max(t, c); k++)
            {
                aa = s[i] = RotateLeft(s[i] + aa + bb, 3);
                bb = l[j] = RotateLeft(l[j] + aa + bb, (int)(aa + bb));
                i = (i + 1) % t;
                j = (j + 1) % c;
            }
            return s;
        }

        internal static ulong RotateLeft(ulong a, int offset)
        {
            offset = offset % 64;
            return (a << offset) | (a >> (64 - offset));
        }

        internal static ulong RotateRight(ulong a, int offset)
        {
            offset = offset % 64;
            return (a >> offset) | (a << (64 - offset));
        }
    }
}