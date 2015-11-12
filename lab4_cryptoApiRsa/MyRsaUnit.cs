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
        public delegate void ProgresChangeEvent(int persents);

        public event ProgresChangeEvent OnProgresChange;

        private const int KeyCryptFileDataLength = 64;
        private const int KeyDecryptFileDataLength = 128;
        
        public string InputFilePath { get; set; }
        public string OutputFileFilePath { get; set; }

        public bool GenerateKeyPair(string pubFilePath, string privFilePath)
        {
            try
            {
                using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(1024))
                {
                    byte[] key = rsaProvider.ExportCspBlob(false);
                    File.WriteAllBytes(pubFilePath, key);
                    key = rsaProvider.ExportCspBlob(true);
                    File.WriteAllBytes(privFilePath, key);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public bool Encrypt(byte[] publicKey)
        {
            try
            {
                using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(1024))
                {
                    rsaProvider.ImportCspBlob(publicKey);
                    using (
                        FileStream inStream = new FileStream(InputFilePath, FileMode.Open),
                            outStream = new FileStream(OutputFileFilePath, FileMode.Create))
                    {
                        EncryptData(rsaProvider, inStream, outStream);
                        inStream.Close();
                        outStream.Close();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void EncryptData(RSACryptoServiceProvider provider, Stream inStream, Stream outStream)
        {
            var bytes = new byte[KeyCryptFileDataLength];
            int readBytes = KeyCryptFileDataLength;
            byte[] realReadBytes = null;
            byte offset = 0;
            byte[] cryptBytes;
            while (readBytes == KeyCryptFileDataLength)
            {
                readBytes = inStream.Read(bytes, 0, KeyCryptFileDataLength);
                realReadBytes = bytes;
                if (readBytes == 0) break;
                if (readBytes < KeyCryptFileDataLength)
                {
                    offset = (byte)readBytes;
                }
                cryptBytes = provider.Encrypt(realReadBytes, false);
                outStream.Write(cryptBytes, 0, cryptBytes.Length);

                if (OnProgresChange != null)
                {
                    OnProgresChange((int)(inStream.Position / (double)inStream.Length) * 100);
                }
            }
            realReadBytes[0] = offset;
            cryptBytes = provider.Encrypt(realReadBytes, false);
            outStream.Write(cryptBytes, 0, cryptBytes.Length);
        }

        public bool Decrypt(byte[] privateKey)
        {
            try
            {
                using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(1024))
                {
                    rsaProvider.ImportCspBlob(privateKey);
                    using (
                        FileStream inStream = new FileStream(InputFilePath, FileMode.Open),
                            outStream = new FileStream(OutputFileFilePath, FileMode.Create))
                    {
                        DecryptData(rsaProvider, inStream, outStream);
                        inStream.Close();
                        outStream.Close();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void DecryptData(RSACryptoServiceProvider provider, Stream inStream, Stream outStream)
        {
            var bytes = new byte[KeyDecryptFileDataLength];
            int readBytes = KeyDecryptFileDataLength;
            inStream.Position = inStream.Length - KeyDecryptFileDataLength;
            readBytes = inStream.Read(bytes, 0, KeyDecryptFileDataLength);
            byte[] cryptBytes = provider.Decrypt(bytes, false);
            byte offset = cryptBytes[0]; //length of padding
            inStream.Position = 0;
            while (inStream.Position < inStream.Length - KeyDecryptFileDataLength)
            {
                readBytes = inStream.Read(bytes, 0, KeyDecryptFileDataLength);
                if (readBytes == 0) break;
                cryptBytes = provider.Decrypt(bytes, false);
                int length = cryptBytes.Length;
                if (inStream.Position >= inStream.Length - KeyDecryptFileDataLength)
                    length = offset;
                outStream.Write(cryptBytes, 0, length);

                if (OnProgresChange != null)
                {
                    OnProgresChange((int)(inStream.Position / (double)inStream.Length) * 100);
                }
            }
        }


        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
