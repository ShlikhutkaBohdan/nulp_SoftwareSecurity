using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using lab1_random;
using lab2_md5;
using lab3_rc5.model;

namespace lab3_rc5.model
{
    public class MyRc5W64 : MyRc5//WordType - for a and b
    {
        public override event ProgressChangedEvent OnProgressChanged;
        public override event DecryptedPasswordFailedEvent OnDecryptedPasswordFailed;
        public override event ProcessEndedEvent OnProcessEnded;
       
        public MyRc5W64(string password, int keyLength, byte numberOfRounds)
        {
            //Type type = typeof (WordType);
            _mW = sizeof(ulong);//8;//(byte) (Marshal.SizeOf(type)*8);//8;//64 bits
            _numberOfRounds = numberOfRounds;//16;
            _mKeyLength = keyLength;
            _numberOfRounds = numberOfRounds;
            SetPassword(password);
        }
       
        public override void Encrypt(string source, string dest)
        {
            if (File.Exists(dest))
                File.Delete(dest);//delete old file
            var input = File.OpenRead(source);
            var output = File.Open(dest, FileMode.OpenOrCreate, FileAccess.Write);
            EncryptStream(input, output);
            input.Close();
            output.Close();
            if (OnProcessEnded != null)
                OnProcessEnded();
        }

        public override void Decrypt(string source, string dest)
        {
            if (File.Exists(dest))
                File.Delete(dest);
            var input = File.OpenRead(source);
            var output = File.Open(dest, FileMode.OpenOrCreate, FileAccess.Write);
            bool isDecrypted = DecryptStream(input, output);
            input.Close();
            output.Close();
            if (!isDecrypted)
            {
                File.Delete(dest);
            }
            if (OnProcessEnded != null)
                OnProcessEnded();
        }

        private void EncryptStream(Stream inputStream, Stream outputStream)
        {
            byte[] key = _mKey;
            var s = GenerateSubKeys(key);
            ulong a0 = 0, b0 = 0, a, b;
            MyRandom myRandom = new MyRandom(31, 2147483647, 16807, 17711);
            a0 = myRandom.random();//generate random a and b
            b0 = myRandom.random();
            int w2 =  (_mW<<1);
            var buffer = new byte[w2];//buffer of a and b
            inputStream.Position = 0;
            outputStream.Position = 0;
            int paddingLength = 0;
            int readBytesCount;
            do{//main cycle
                readBytesCount = inputStream.Read(buffer, 0, w2);
                if (readBytesCount == 0)
                    break;
                a = BitConverter.ToUInt64(buffer, 0);//get a
                b = BitConverter.ToUInt64(buffer, _mW);//get b
                EncryptBlock(ref a0, ref b0, ref a, ref b, s);
                outputStream.Write(BitConverter.GetBytes(a0), 0, _mW);
                outputStream.Write(BitConverter.GetBytes(b0), 0, _mW);
                if (readBytesCount < w2)//if the last block < w2
                    paddingLength = w2 - readBytesCount;//fill padding by random bytes
                
                if (OnProgressChanged != null)//calculate progress
                    OnProgressChanged((int)(inputStream.Position / (float)inputStream.Length*100));
            }while (true);
            int countOfPaddingBlocks = _mKey.Length + 1;//space on md5 + length of prev padding
            countOfPaddingBlocks = countOfPaddingBlocks/w2 + (countOfPaddingBlocks/w2 > 0 ? 1 : 0);
            buffer = new byte[w2*countOfPaddingBlocks];
            _mKey.CopyTo(buffer, 0);//password md5 padding
            buffer[buffer.Length-1] = (byte)paddingLength;//padding length
            for (int i = 0; i < countOfPaddingBlocks; ++i)//padding filler
            {
                a = BitConverter.ToUInt64(buffer, i*w2);
                b = BitConverter.ToUInt64(buffer, i * w2+_mW);
                EncryptBlock(ref a0, ref b0, ref a, ref b, s);
                outputStream.Write(BitConverter.GetBytes(a), 0, _mW);
                outputStream.Write(BitConverter.GetBytes(b), 0, _mW);
            }
            if (OnProgressChanged != null)
                OnProgressChanged(100);
        }
        
        private bool DecryptStream(Stream inputStream, Stream outputStream)
        {
            byte[] key = _mKey; //key in byte format
            var s = GenerateSubKeys(key);
            ulong a0 = 0, b0 = 0, a, b;
            MyRandom myRandom = new MyRandom(31, 2147483647, 16807, 17711);
            a0 = myRandom.random();//generate random a and b
            b0 = myRandom.random();
            int w2 = _mW << 1, i, l = _mKey.Length + 1;
            var buffer = new byte[w2];
            inputStream.Position = 0;
            outputStream.Position = 0;
            l = l / w2 + (l / w2 > 0 ? 1 : 0);//number of padding blocks
            do
            {
                inputStream.Read(buffer, 0, w2);
                a = BitConverter.ToUInt64(buffer, 0);
                b = BitConverter.ToUInt64(buffer, _mW);
                DecryptBlock(ref a0, ref b0, ref a, ref b, s);
                outputStream.Write(BitConverter.GetBytes(a), 0, _mW);
                outputStream.Write(BitConverter.GetBytes(b), 0, _mW);
                
                if (OnProgressChanged != null)//calculate progress
                    OnProgressChanged((int)(inputStream.Position / (float)inputStream.Length * 100));
            } while (inputStream.Position < inputStream.Length-l*w2-w2);
            byte[] padding = new byte[l*w2+w2];
            MemoryStream paddingStream = new MemoryStream(padding);
            paddingStream.Position = 0;
            do
            {
                inputStream.Read(buffer, 0, w2); //padding
                a = BitConverter.ToUInt64(buffer, 0);
                b = BitConverter.ToUInt64(buffer, _mW);
                DecryptBlock(ref a0, ref b0, ref a, ref b, s);
                paddingStream.Write(BitConverter.GetBytes(a), 0, _mW);
                paddingStream.Write(BitConverter.GetBytes(b), 0, _mW);

            } while (inputStream.Position < inputStream.Length);
            for (i = 0; i < _mKey.Length; ++i)//check md5
            {//16-31 password md5
                if (_mKey[i] != padding[i + 16])
                {
                    //MessageBox.Show("not right password");
                    if (OnDecryptedPasswordFailed != null)
                        OnDecryptedPasswordFailed();
                    return false;
                }
            }
            int len = padding[padding.Length - 1];
            outputStream.Write(padding, 0, w2-len);
            if (OnProgressChanged != null)//calculate process
                OnProgressChanged(100);
            return true;
        }

        private void EncryptBlock(ref ulong a0, ref ulong b0, ref ulong a, ref ulong b, ulong[] s)
        {
            a ^= a0;
            b ^= b0;
            a += s[0];
            b += s[1];
            for (var i = 1; i <= _numberOfRounds; i++)
            {
                a = RotateLeft(a ^ b, (int)b) + s[2 * i];
                b = RotateLeft(b ^ a, (int)a) + s[2 * i + 1];
            }
            a0 = a;
            b0 = b;
        }

        private void DecryptBlock(ref ulong a0, ref ulong b0, ref ulong a, ref ulong b, ulong[] s)
        {
            var prevA = a;
            var prevB = b;
            for (var i = _numberOfRounds; i > 0; i--)
            {
                b = RotateLeft(b - s[2 * i + 1], (int)a, true) ^ a;//rotate right
                a = RotateLeft(a - s[2 * i], (int)b, true) ^ b;//rotate right
            }
            b -= s[1];
            a -= s[0];
            a ^= a0;
            b ^= b0;
            a0 = prevA;
            b0 = prevB;
        }

        private ulong[] GenerateSubKeys(byte[] key)
        {
            ulong i, j;
            var keyLength = key.Length;
            var c = (ulong)(keyLength / _mW);//K length in words
            var l = new ulong[c];
            Buffer.BlockCopy(key, 0, l, 0, keyLength);
            ulong subkeysCount = (ulong) (2 * _numberOfRounds + 2);//counts of subkeys
            var s = new ulong[subkeysCount];//subleys
            s[0] = _mPw;
            for (i = 1; i < subkeysCount; i++)
                s[i] = s[i - 1] + _mQw;
            i = j = 0;
            ulong a = 0, b = 0;
            for (ulong k = 0; k < 3 * Math.Max(subkeysCount, c); k++)
            {
                a = s[i] = RotateLeft(s[i] + a + b, 3);
                b = l[j] = RotateLeft(l[j] + a + b, (int)(a + b));
                i = (i + 1) % subkeysCount;
                j = (j + 1) % c;
            }
            return s;
        }

        ulong RotateLeft(ulong value, int offset, bool reverse = false)
        {
            int bitLength = _mW << 3;//*8
            if (reverse)
                return (value << (offset % bitLength)) | (value >> (bitLength - (offset % bitLength)));
            return (value >> (offset % bitLength)) | (value << (bitLength - (offset % bitLength)));

        }

        private ulong _mPw = Pw64;
        private ulong _mQw = Qw64;

        private byte _mW;
        private byte _numberOfRounds;// = 16;
        

    }
}