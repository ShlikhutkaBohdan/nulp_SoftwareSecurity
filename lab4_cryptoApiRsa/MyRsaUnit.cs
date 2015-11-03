using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab4_cryptoApiRsa
{
    public class MyRsaUnit: IDisposable
    {
        private const int KeyLenght = 1024;
        private const int MaxRsaBlock = KeyLenght / 8 - 11 - 30 - 1;

        private void SerializeRsaKey(RSAParameters rparam, string file, string pass)
        {
            FileStream fileStreams;
            using (fileStreams = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var binformt = new BinaryFormatter();
                binformt.Serialize(fileStreams, rparam.Exponent); //серіалізуємо публічну частину
                binformt.Serialize(fileStreams, rparam.Modulus);
                if (pass != null) //зберігаємо приватну частину
                {
                    var memoryStream = new MemoryStream();
                    binformt.Serialize(memoryStream, rparam.P);
                    binformt.Serialize(memoryStream, rparam.Q);
                    binformt.Serialize(memoryStream, rparam.D);
                    binformt.Serialize(memoryStream, rparam.InverseQ);
                    binformt.Serialize(memoryStream, rparam.DP);
                    binformt.Serialize(memoryStream, rparam.DQ);
                    var pdb = new PasswordDeriveBytes(pass, null) { HashName = "SHA256" }; //генеруємо ключ експорту
                    var rjn = (Rijndael)new RijndaelManaged();
                    rjn.KeySize = 256;
                    rjn.Key = pdb.GetBytes(256 / 8);
                    rjn.IV = new byte[rjn.BlockSize / 8];
                    var tr = rjn.CreateEncryptor();
                    var arr = memoryStream.ToArray();
                    arr = tr.TransformFinalBlock(arr, 0, arr.Length);
                    fileStreams.Write(arr, 0, arr.Length);
                    rjn.Clear();
                    memoryStream.Close();
                }
                fileStreams.Close();
            }
        }

        private RSAParameters DeserializeRsaKey(string file, string pass)
        {
            FileStream fs;
            var rp = new RSAParameters();
            using (fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bf = new BinaryFormatter();
                rp.Exponent = (byte[])bf.Deserialize(fs); //сериализуем публичную часть
                rp.Modulus = (byte[])bf.Deserialize(fs);
                if (pass != null) //сохраняем приватную часть
                {
                    var len = fs.Length - fs.Position;
                    var arr = new byte[len];
                    len = fs.Read(arr, 0, (int)len);
                    var pdb = new PasswordDeriveBytes(pass, null); //генерируем ключ экспорта
                    pdb.HashName = "SHA256"; //будем использовать SHA256
                    var rjn = (Rijndael)new RijndaelManaged();
                    rjn.KeySize = 256;
                    rjn.Key = pdb.GetBytes(256 / 8);
                    rjn.IV = new byte[rjn.BlockSize / 8];
                    var tr = rjn.CreateDecryptor();
                    arr = tr.TransformFinalBlock(arr, 0, (int)len);
                    rjn.Clear();
                    var ms = new MemoryStream(arr, false);
                    rp.P = (byte[])bf.Deserialize(ms);
                    rp.Q = (byte[])bf.Deserialize(ms);
                    rp.D = (byte[])bf.Deserialize(ms);
                    rp.InverseQ = (byte[])bf.Deserialize(ms);
                    rp.DP = (byte[])bf.Deserialize(ms);
                    rp.DQ = (byte[])bf.Deserialize(ms);
                    ms.Close();
                }
                fs.Close();
                return rp;
            }
        }

        private DSAParameters _dsaSignPrivate, _dsaSignPublic;
        private RSAParameters _rsaEncPrivate, _rsaEncPublic;
        private RSAParameters rsaencpub, rsaencpriv, rsasgnpriv, rsasgnpub, dsasgnpriv, dsasgnpub;

        public bool EncryptRsa(string inputFilePath, string outputFilePath)
        {
            try
            {
                FileStream ins, outs;
                using (ins = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    outs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    //var bf = new BinaryFormatter();
                    var rsa = new RSACryptoServiceProvider(KeyLenght);
                    var tmp = new RSAParameters
                    {
                        Exponent = (byte[])rsaencpub.Exponent.Clone(),
                        Modulus = (byte[])rsaencpub.Modulus.Clone()
                    };
                    rsa.ImportParameters(tmp); //встановлюємо ключ
                    byte[] inbuf = new byte[MaxRsaBlock];
                    byte[] outbuf = new byte[MaxRsaBlock];
                    int len;
                    while ((len = ins.Read(inbuf, 0, MaxRsaBlock)) == MaxRsaBlock)
                    {
                        byte[] enc = rsa.Encrypt(inbuf, true); //шифруємо
                        outs.Write(enc, 0, enc.Length);
                    }
                    outbuf = rsa.Encrypt(inbuf, true);
                    outs.Write(outbuf, 0, outbuf.Length);
                    //AsymmetricSignatureFormatter sf;
                    outs.Close();
                    ins.Close();
                }
                //MessageBox.Show("Готово!");
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public bool DecryptRsa(string inputFilePath, string outputFilePath)
        {
            try
            {
                FileStream ins, outs;
                using (ins = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    outs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    var rsa = new RSACryptoServiceProvider(KeyLenght);
                    var tempParam = new RSAParameters
                    {
                        D = (byte[]) rsaencpriv.D.Clone(),
                        DP = (byte[]) rsaencpriv.DP.Clone(),
                        DQ = (byte[]) rsaencpriv.DQ.Clone(),
                        Exponent = (byte[]) rsaencpriv.Exponent.Clone(),
                        InverseQ = (byte[]) rsaencpriv.InverseQ.Clone(),
                        Modulus = (byte[]) rsaencpriv.Modulus.Clone(),
                        P = (byte[]) rsaencpriv.P.Clone(),
                        Q = (byte[]) rsaencpriv.Q.Clone()
                    };
                    rsa.ImportParameters(tempParam); //встановлюємо ключ
                    var inbuf = new byte[KeyLenght / 8];
                    var outbuf = new byte[MaxRsaBlock];
                    try
                    {
                        while (ins.Position < ins.Length) //нужно не прочитать подпись ненароком
                        {
                            ins.Read(inbuf, 0, KeyLenght / 8);
                            var dec = rsa.Decrypt(inbuf, true); //decrypt block
                            int length = dec.Length;
                            //if (ins.Position != ins.Length)
                                outs.Write(dec, 0, length);
                        }

                        //outbuf = new byte[0];
                        //AsymmetricSignatureDeformatter df;
                        //byte[] sig;
                    }
                    catch (Exception ex)
                    {
                         MessageBox.Show(""+ex);
                    }
                    outs.Close();
                    ins.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool GetPublicKey(string keyPath)
        {
            rsaencpub = DeserializeRsaKey(keyPath, null);
            return true;
        }

        public bool GetPrivateKey(string keyPath, string password)
        {
            rsaencpriv = DeserializeRsaKey(keyPath, password);
            return true;
        }

        /*private void TakePublicKey(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog {DefaultExt = ".epb", Title = "Виберіть файл публічного ключа"};
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    rsaencpub = DeserializeRsaKey(ofd.FileName, null);
                    EncPubPresent = true;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("" + exception);
                }
            }
        }

        private void TakePrivateKey(object sender, RoutedEventArgs e)
        {
            var openDl = new OpenFileDialog {DefaultExt = ".spr", Title = "Виберіть файл приватного ключа"};
            if (openDl.ShowDialog() == true)
            {
                    try
                    {
                        rsaencpriv = DeserializeRsaKey(openDl.FileName, RsaPass.Text);
                        EncPrivPresent = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Можливо ви ввели неправильний пароль");
                    }
            }
        }
         */
         
        public bool GenerateKeyPair(string privatKeyFileName, string publicKeyFileName, string password)
        {
            var rsa = new RSACryptoServiceProvider(KeyLenght);
            rsaencpriv = rsa.ExportParameters(true);
            rsaencpub = rsa.ExportParameters(false);
            SerializeRsaKey(rsaencpriv, privatKeyFileName, password);
            SerializeRsaKey(rsaencpub, publicKeyFileName, null);
            return true;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
