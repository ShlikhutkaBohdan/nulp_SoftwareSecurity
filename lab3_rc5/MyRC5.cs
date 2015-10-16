using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using lab1_random;
using lab2_md5;

namespace lab3_rc5
{
    class MyRC5
    {
        private const byte W = 8;
        private const byte W2 = W * 2;
        private const byte Rounds = 16;
        private const ulong Pw = 0xb7e151628aed2a6b;
        private const ulong Qw = 0x9e3779b97f4a7c15;

        private byte[] _mPasswordMd5;

        public MyRC5(string password)
        {
            password = "a";
            MyMD5 myMd5 = new MyMD5();
            var md5 = myMd5.GetMd5ArrFromString(password);
            _mKey = md5.SelectMany(BitConverter.GetBytes).ToArray();
            _mPasswordMd5 = _mKey;


            Encrypt("C:\\Users\\Boday-Alfaro\\Desktop\\MpShlikhutkaLab4.docx", "C:\\Users\\Boday-Alfaro\\Desktop\\qwe1");
            Decrypt("C:\\Users\\Boday-Alfaro\\Desktop\\qwe1", "C:\\Users\\Boday-Alfaro\\Desktop\\qwe1.docx");
            MessageBox.Show("done");
        }

        private byte[] _mKey;

        public void Encrypt(string source, string dest)
        {
            if (File.Exists(dest))
                File.Delete(dest);
            var input = File.OpenRead(source);
            var output = File.Open(dest, FileMode.OpenOrCreate, FileAccess.Write);
            EncryptStream(input, output);
            input.Close();
            output.Close();
        }
        private void EncryptStream(Stream input, Stream output)
        {
            byte[] key = _mKey;
            var s = Keys(key);

            ulong a0 = 0, b0 = 0;
            MyRandom myRandom = new MyRandom(31, 2147483647, 16807, 17711);
            var a = (ulong)myRandom.random();
            var b = (ulong)myRandom.random();

            var inBuffer = new byte[W2];
            input.Position = 0;
            output.Position = 0;

            EncodeBlock(ref a0, ref b0, a, b, s, output);

            int paddingLength = 0;

            while (input.Position < input.Length)
            {
                var readed = input.Read(inBuffer, 0, W2);
                if (readed < W2)
                {
                    paddingLength = W2 - readed;
                    Filling(inBuffer, readed, paddingLength);//padding
                    Buffer.BlockCopy(BitConverter.GetBytes(paddingLength), 0, inBuffer, W2 - 1, 1);
                }
                a = BitConverter.ToUInt64(inBuffer, 0);
                b = BitConverter.ToUInt64(inBuffer, W);
                EncodeBlock(ref a0, ref b0, a, b, s, output);
            }
            int l = _mPasswordMd5.Length + 1;
            l = l/W2 + (l/W2 > 0 ? 1 : 0);
            inBuffer = new byte[W2*l];
            _mPasswordMd5.CopyTo(inBuffer, 0);//password md5 padding
            inBuffer[inBuffer.Length-1] = (byte)paddingLength;//padding length
            for (int i = 0; i < l; ++i)
            {
                a = BitConverter.ToUInt64(inBuffer, i*W2);
                b = BitConverter.ToUInt64(inBuffer, i * W2+W);
                EncodeBlock(ref a0, ref b0, a, b, s, output);
            }
            
        }


        public void Decrypt(string source, string dest)
        {
            if (File.Exists(dest))
                File.Delete(dest);
            var input = File.OpenRead(source);
            var output = File.Open(dest, FileMode.OpenOrCreate, FileAccess.Write);
            DecryptStream(input, output);
            input.Close();
            output.Close();
        }

        private void DecryptStream(Stream input, Stream output)
        {
            byte[] key = _mKey; 
            var s = Keys(key);
            ulong a0 = 0, b0 = 0;
            var inBuffer = new byte[W2];
            input.Position = 0;
            output.Position = 0;
            ulong a;
            ulong b;
            int i = 0;
            int receivedBytesCount = 0;
            bool fl = false;
            int l = _mPasswordMd5.Length + 1;
            l = l / W2 + (l / W2 > 0 ? 1 : 0);
            do
            {
                input.Read(inBuffer, 0, W2);
                a = BitConverter.ToUInt64(inBuffer, 0);
                b = BitConverter.ToUInt64(inBuffer, W);
                if (fl)
                {
                    DecodeBlock(ref a0, ref b0, ref a, ref b, s, output, true);
                }
                else
                {
                    fl = true;
                    DecodeBlock(ref a0, ref b0, ref a, ref b, s, output, false);
                }
            } while (input.Position < input.Length-l*W2-W2);
           
            byte[] padding = new byte[l*W2+W2];
            MemoryStream paddingStream = new MemoryStream(padding);
            paddingStream.Position = 0;
            do
            {
                input.Read(inBuffer, 0, W2); //padding
                a = BitConverter.ToUInt64(inBuffer, 0);
                b = BitConverter.ToUInt64(inBuffer, W);
                DecodeBlock(ref a0, ref b0, ref a, ref b, s, paddingStream, true);
            } while (input.Position < input.Length);
            int len = padding[padding.Length - 1];
            output.Write(padding, 0, W2-len);

        }

        void Filling(Array a, int offset, int count)
        {
            for (var i = 0; i < count; i++)
                (a as dynamic)[offset + i] = (byte)0;
        }

        private void EncodeBlock(ref ulong a0, ref ulong b0, ulong a, ulong b, IList<ulong> s, Stream output)
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

        private void DecodeBlock(ref ulong a0, ref ulong b0, ref ulong a, ref ulong b, IList<ulong> s, Stream output, bool writeFlag)
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

        private ulong[] Keys(byte[] key)
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

        ulong RotateLeft(ulong a, int offset)
        {
            offset = offset % 64;
            return (a << offset) | (a >> (64 - offset));
        }

        ulong RotateRight(ulong a, int offset)
        {
            offset = offset % 64;
            return (a >> offset) | (a << (64 - offset));
        }
    }
}