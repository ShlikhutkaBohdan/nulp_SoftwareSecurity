using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab1_random;
using lab2_md5;

namespace lab3_rc5
{
    public class MyRC5
    {
        public MyRC5(uint numberOfRounds, KeyLength keyLength, uint wordLength)
        {
            _numberOfRounds = (int) numberOfRounds;
            //_keyLength = keyLength;
            _wordLength = wordLength;
            switch (keyLength)
            {
                case KeyLength.KEY_LENGTH_16:
                    _pw = P_16;
                    _qw = Q_16;
                    break;
                case KeyLength.KEY_LENGTH_32: 
                    _pw = P_32;
                    _qw = Q_32; 
                    break;
                case KeyLength.KEY_LENGTH_64: 
                    _pw = P_64;
                    _qw = Q_64;
                    break;
            }
            _myRandom = new MyRandom(31, 2147483647, 16807, 17711);
            _myMd5 = new MyMD5();
        }

        public void SetKey(byte[] key)
        {
            
        }

        public void Encrypt(Stream plainText, ref Stream enryptedText, byte[] key)
        {
            ushort a = (ushort) _myRandom.random();
            ushort b = (ushort) _myRandom.random();


        }

        public void Decrypt(Stream enryptedText, ref Stream plainText, byte[] key)
        {

        }

        private void EncryptBlock(ushort a0, ushort b0, ushort[] s, ref ushort a1, ref ushort b1)
        {
            ushort a = (ushort) (a0 + s[0]);
            ushort b = (ushort) (b0 + s[1]);
            for (int i = 0; i < _numberOfRounds; ++i)
            {
                a = (ushort) (RotateLeft(a ^ b, b) + s[2*i]);
                b = (ushort) (RotateLeft(a ^ b, a) + s[2*i + 1]);
            }
            a1 = a;
            b1 = b;
        }

        private void DecryptBlock(ushort a0, ushort b0, ushort[] s, ref ushort a1, ref ushort b1)
        {
            ushort a = a0;
            ushort b = b0;
            for (int i = _numberOfRounds-1; i >= 0; ++i)
            {
                b = (ushort) (RotateRight(b - s[2*i + 1], a) ^ a);
                a = (ushort) (RotateRight(a - s[2*i], b) ^ b);
            }
            a1 = (ushort) (a - s[0]);
            b1 = (ushort) (b - s[1]);
        }

        private ushort[] GenerateSubkeys(ushort[] keys)//modify
        {
            ushort[] copy = new ushort[keys.Length];
            keys.CopyTo(copy, 0);
            var subKeys = new ushort[2 * _numberOfRounds + 2];
            for (uint i = 0; i < subKeys.Length; i++)
            {
                subKeys[i] = (ushort)(_pw + i * _qw);
            }
            int ii = 0;
            int j = 0, t = Math.Max(keys.Length, subKeys.Length);
            ushort a = 0, b = 0;
            for (int s = 0; s < 3 * t; s++)
            {
                subKeys[ii] = RotateLeft(subKeys[ii] + a + b, 3);
                a = subKeys[ii];
                ii = ii % subKeys.Length;
                copy[j] = RotateLeft(copy[j] + a + b, (ushort)(a + b));
                b = copy[j];
                j = j % copy.Length;
            }
            return subKeys;
        }

        private ushort RotateLeft(int value, ushort offset)//value 2 bytes
        {
            offset = (ushort)(offset % 16);
            ushort v = (ushort)value;
            ushort result = 0;
            result = (ushort)((v << offset) | (v >> (16 - offset)));
            return result;
        }

        private ushort RotateRight(int value, ushort offset)//value 2 bytes
        {
            offset = (ushort)(offset % 16);
            ushort v = (ushort)value;
            ushort result = 0;
            result = (ushort)((v >> offset) | (v << (16 - offset)));
            return result;
        }

        private int _numberOfRounds;
        //private KeyLength _keyLength;
        private uint _wordLength;
        private MyRandom _myRandom;
        private MyMD5 _myMd5;

        private ulong _qw;
        private ulong _pw;

        private const ulong P_16 = 0xb7e1;
        private const ulong P_32 = 0xb7e15163;
        private const ulong P_64 = 0xb7e151628aed2a6b;

        private const ulong Q_16 = 0x9e37;
        private const ulong Q_32 = 0x9e3779b9;
        private const ulong Q_64 = 0x9e3779b97f4a7c15;

        public enum KeyLength
        {
            KEY_LENGTH_16,
            KEY_LENGTH_32,
            KEY_LENGTH_64
        }
    }
}
