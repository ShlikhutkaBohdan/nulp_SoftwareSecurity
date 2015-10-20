using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2_md5;

namespace lab3_rc5.model
{
    public abstract class MyRc5
    {
        public static MyRc5 GetRc5(int wordLength,  string password, int keyLength, byte numberOfRounds)
        {
            switch (wordLength)
            {
                case 64:
                    return new MyRc5W64(password, keyLength, numberOfRounds);
                    break;
                case 32:
                    return new MyRc5W32(password, keyLength, numberOfRounds);
                    break;
                case 16:
                    return new MyRc5W16(password, keyLength, numberOfRounds);
                    break;
                default:
                    return null;
            }
        }

        protected int _mKeyLength;// = 16;

        protected void SetPassword(string password)
        {
            //password = "a";

            MyMD5 myMd5 = new MyMD5();
            var md5 = myMd5.GetMd5ArrFromString(password);
            switch (_mKeyLength)
            {
                case 8:
                    _mKey = new byte[8];
                    byte[] b = md5.SelectMany(BitConverter.GetBytes).ToArray();
                    for (int i = 8; i < b.Length; ++i)
                        _mKey[i-8] = b[i];
                    break;
                case 16:
                    _mKey = new byte[16];
                    md5.SelectMany(BitConverter.GetBytes).ToArray().CopyTo(_mKey, 0);
                    break;
                case 32:
                    _mKey = new byte[32];
                    md5.SelectMany(BitConverter.GetBytes).ToArray().CopyTo(_mKey, 0);
                    myMd5.GetMd5ArrFromString(myMd5.GetMd5FromString(password)).SelectMany(BitConverter.GetBytes).ToArray().CopyTo(_mKey, 16);
                    break;
            }


            //Encrypt("C:\\Users\\Boday-Alfaro\\Desktop\\SecurityShlikhutkaLab3.docx", "C:\\Users\\Boday-Alfaro\\Desktop\\2.docx");
            //Decrypt("C:\\Users\\Boday-Alfaro\\Desktop\\2.docx", "C:\\Users\\Boday-Alfaro\\Desktop\\3.docx");

            //            Encrypt("C:\\Users\\Boday-Alfaro\\Desktop\\qwe1.txt", "C:\\Users\\Boday-Alfaro\\Desktop\\qwe2.txt");
            //          Decrypt("C:\\Users\\Boday-Alfaro\\Desktop\\qwe2.txt", "C:\\Users\\Boday-Alfaro\\Desktop\\qwe3.txt");
            //MessageBox.Show("done");
        }

        protected byte[] _mKey;//key in byte format

        public delegate void ProgressChangedEvent(int persents);

        public delegate void DecryptedPasswordFailedEvent();

        public delegate void ProcessEndedEvent();

        public abstract event ProgressChangedEvent OnProgressChanged;
        public abstract event DecryptedPasswordFailedEvent OnDecryptedPasswordFailed;
        public abstract event ProcessEndedEvent OnProcessEnded;

        public abstract void Encrypt(string source, string dest);
        public abstract void Decrypt(string source, string dest);

        protected const ulong Pw64 = 0xb7e151628aed2a6b;
        protected const ulong Qw64 = 0x9e3779b97f4a7c15;
        protected const uint Pw32 = 0xb7e15163;
        protected const uint Qw32 = 0x9e3779b9;
        protected const ushort Pw16 = 0xb7e1;
        protected const ushort Qw16 = 0x9e37;
    }
}
