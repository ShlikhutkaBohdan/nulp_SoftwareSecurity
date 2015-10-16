using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3_rc5.model
{
    public abstract class MyRc5
    {
        public static MyRc5 GetRc5(int keyLength, string password)
        {
            switch (keyLength)
            {
                case 64:
                    return new MyRc5W64(password);
                    break;
                case 32:
                    return new MyRc5W32(password);
                    break;
                case 16:
                    return new MyRc5W16(password);
                    break;
                default:
                    return null;
            }
        }

        public delegate void ProgressChangedEvent(int persents);

        public delegate void DecryptedPasswordFailedEvent();

        public delegate void ProcessEndedEvent();

        public abstract event ProgressChangedEvent OnProgressChanged;
        public abstract event DecryptedPasswordFailedEvent OnDecryptedPasswordFailed;
        public abstract event ProcessEndedEvent OnProcessEnded;

        public abstract void SetPassword(string password);
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
