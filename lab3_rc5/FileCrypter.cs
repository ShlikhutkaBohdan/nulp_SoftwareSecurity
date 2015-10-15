using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using lab2_md5;

namespace lab3_rc5
{
    public class FileCrypter
    {
        private Rc516Counter m_rc5;

        public FileCrypter(string password)
        {
            //IMd5Stream stream = new Md5StringStream(password);
            MyMD5 myMd5 = new MyMD5();
            var keys = myMd5.GetMd5ArrFromString(password);//Md5Counter.FindHashCode(stream);
            ushort[] uskeys = new ushort[keys.Length * 2];
            keys.Select((u, i) =>
            {
                uskeys[i * 2] = (ushort)u;
                uskeys[i * 2 + 1] = (ushort)(u>>16);
                return true;
            }).ToList();
            m_rc5 = new Rc516Counter(8, uskeys);
        }

        public void Crypt(string source, string dest)
        {
            if (File.Exists(dest))
                File.Delete(dest);
            var input = File.OpenRead(source);
            var output = File.Open(dest, FileMode.OpenOrCreate, FileAccess.Write);
            m_rc5.Crypt(input, output);
            input.Close();
            output.Close();
        }

        public void Decrypt(string source, string dest)
        {
            if (File.Exists(dest))
                File.Delete(dest);
            var input = File.OpenRead(source);
            var output = File.Open(dest, FileMode.OpenOrCreate, FileAccess.Write);
            m_rc5.Decrypt(input, output);
            input.Close();
            output.Close();
        }

    }
}
