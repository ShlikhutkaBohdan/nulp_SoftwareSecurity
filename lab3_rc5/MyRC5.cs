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
        public delegate void ProgressChangedEvent(int persents);
        public delegate void DecryptedPasswordFailedEvent();
        public delegate void ProcessEndedEvent();

        public event ProgressChangedEvent OnProgressChanged;
        public event DecryptedPasswordFailedEvent OnDecryptedPasswordFailed;
        public event ProcessEndedEvent OnProcessEnded;

        public MyRC5(string password)
        {
            _numberOfRounds = 16;
            SetPassword(password);
        }

        public void SetPassword(string password)
        {
            //password = "a";
            
            MyMD5 myMd5 = new MyMD5();
            var md5 = myMd5.GetMd5ArrFromString(password);
            _mKey = md5.SelectMany(BitConverter.GetBytes).ToArray();
            _mPasswordMd5 = _mKey;
            
            /*Encrypt("C:\\Users\\Boday-Alfaro\\Desktop\\MpShlikhutkaLab4.docx", "C:\\Users\\Boday-Alfaro\\Desktop\\qwe1");
            Decrypt("C:\\Users\\Boday-Alfaro\\Desktop\\qwe1", "C:\\Users\\Boday-Alfaro\\Desktop\\qwe1.docx");
            MessageBox.Show("done");*/
        }

        public void Encrypt(string source, string dest)
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

        public void Decrypt(string source, string dest)
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
            ulong a0 = 0, b0 = 0;
            MyRandom myRandom = new MyRandom(31, 2147483647, 16807, 17711);
            var a = myRandom.random();//generate random a and b
            var b = myRandom.random();
            int w2 =  (_mW<<1);
            var buffer = new byte[w2];//buffer of a and b
            inputStream.Position = 0;
            outputStream.Position = 0;
            int paddingLength = 0;
            int readBytesCount;
            do{//main cycle
                EncryptBlock(ref a0, ref b0, ref a, ref b, s);
                outputStream.Write(BitConverter.GetBytes(a), 0, _mW);
                outputStream.Write(BitConverter.GetBytes(b), 0, _mW);
                readBytesCount = inputStream.Read(buffer, 0, w2);
                if (readBytesCount == 0)
                    break;
                if (readBytesCount < w2)//if the last block < w2
                    paddingLength = w2 - readBytesCount;//fill padding by random bytes
                a = BitConverter.ToUInt64(buffer, 0);//get a
                b = BitConverter.ToUInt64(buffer, _mW);//get b
                if (OnProgressChanged != null)//calculate progress
                    OnProgressChanged((int)(inputStream.Position / (float)inputStream.Length*100));
            }while (true);
            int countOfPaddingBlocks = _mPasswordMd5.Length + 1;//space on md5 + length of prev padding
            countOfPaddingBlocks = countOfPaddingBlocks/w2 + (countOfPaddingBlocks/w2 > 0 ? 1 : 0);
            buffer = new byte[w2*countOfPaddingBlocks];
            _mPasswordMd5.CopyTo(buffer, 0);//password md5 padding
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
            int w2 = _mW << 1, i, l = _mPasswordMd5.Length + 1;
            var buffer = new byte[w2];
            bool fl = false;
            inputStream.Position = 0;
            outputStream.Position = 0;
            l = l / w2 + (l / w2 > 0 ? 1 : 0);//number of padding blocks
            do
            {
                inputStream.Read(buffer, 0, w2);
                a = BitConverter.ToUInt64(buffer, 0);
                b = BitConverter.ToUInt64(buffer, _mW);
                DecryptBlock(ref a0, ref b0, ref a, ref b, s);
                if (fl)
                {
                    outputStream.Write(BitConverter.GetBytes(a), 0, _mW);
                    outputStream.Write(BitConverter.GetBytes(b), 0, _mW);
                }
                fl = true;
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
            for (i = 0; i < _mPasswordMd5.Length; ++i)//check md5
            {//16-31 password md5
                if (_mPasswordMd5[i] != padding[i + 16])
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

        private ulong[] GenerateSubKeys(byte[] key)//todo
        {
            ulong aa = 0, bb = 0, i, j;
            var b = key.Length;
            var c = (ulong)(b / _mW);
            var l = new ulong[c];
            Buffer.BlockCopy(key, 0, l, 0, b);
            ulong t = (ulong) (2 * _numberOfRounds + 2);
            var s = new ulong[t];
            s[0] = Pw64;
            for (i = 1; i < t; i++)
                s[i] = s[i - 1] + Qw64;
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

        ulong RotateLeft(ulong value, int offset, bool reverse = false)
        {
            if (reverse) 
                return (value << (offset % 64)) | (value >> (64 - (offset % 64)));
            return (value >> (offset % 64)) | (value << (64 - (offset % 64)));

        }

        private const ulong Pw64 = 0xb7e151628aed2a6b;
        private const ulong Qw64 = 0x9e3779b97f4a7c15;

        private byte _mW = 8;
        private byte _numberOfRounds;// = 16;
        private byte[] _mPasswordMd5;
        private byte[] _mKey;//key in byte format

    }
}