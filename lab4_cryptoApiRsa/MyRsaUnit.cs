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
        private const int KeyCryptFileDataLength = 64;
        private const int KeyDecryptFileDataLength = 128;
        private readonly RSACryptoServiceProvider _rsaProvider = new RSACryptoServiceProvider(1024);
        private long _rsaTime;
        private long _symTime;

        public bool SaveKeyPair(string pubFilePath, string privFilePath)
        {
            byte[] key = _rsaProvider.ExportCspBlob(false);
            File.WriteAllBytes(pubFilePath, key);
            key = _rsaProvider.ExportCspBlob(true);
            File.WriteAllBytes(privFilePath, key);
            return false;
        }

        public bool RsaCrypt(string infile, string outfile, string publicKeyFile, string privateKeyFile)
        {
            _rsaProvider.ImportCspBlob(File.ReadAllBytes(publicKeyFile));
            _rsaProvider.ImportCspBlob(File.ReadAllBytes(privateKeyFile));
            using (FileStream inStream = new FileStream(infile, FileMode.Open), outStream = new FileStream(outfile, FileMode.Create))
            {
                var bytes = new byte[KeyCryptFileDataLength];
                int readBytes = KeyCryptFileDataLength;
                byte[] realReadBytes = null;
                byte offset = 0;
                byte[] cryptBytes;
                while (readBytes == KeyCryptFileDataLength) {
                    readBytes = inStream.Read(bytes, 0, KeyCryptFileDataLength);
                    realReadBytes = bytes;
                    if (readBytes == 0) break;
                    if (readBytes < KeyCryptFileDataLength)
                    {
                        offset = (byte) readBytes;
                    }
                    cryptBytes = _rsaProvider.Encrypt(realReadBytes, false);
                    outStream.Write(cryptBytes, 0, cryptBytes.Length);
                }
                realReadBytes[0] = offset;
                cryptBytes = _rsaProvider.Encrypt(realReadBytes, false);
                outStream.Write(cryptBytes, 0, cryptBytes.Length);
            }
            return true;
        }

        public bool RsaDecrypt(string infile, string outfile, string publicKeyFile, string privateKeyFile)
        {
            _rsaProvider.ImportCspBlob(File.ReadAllBytes(publicKeyFile));
            _rsaProvider.ImportCspBlob(File.ReadAllBytes(privateKeyFile));
            using (FileStream inStream = new FileStream(infile, FileMode.Open), outStream = new FileStream(outfile, FileMode.Create))
            {
                var bytes = new byte[KeyDecryptFileDataLength];
                int readBytes = KeyDecryptFileDataLength;

                inStream.Position = inStream.Length - KeyDecryptFileDataLength;
                readBytes = inStream.Read(bytes, 0, KeyDecryptFileDataLength);
                byte[] cryptBytes = _rsaProvider.Decrypt(bytes, false);
                byte offset = cryptBytes[0];//length of padding

                inStream.Position = 0;
                while (inStream.Position < inStream.Length - KeyDecryptFileDataLength)
                {
                    readBytes = inStream.Read(bytes, 0, KeyDecryptFileDataLength);
                    if (readBytes == 0) break;
                     cryptBytes = _rsaProvider.Decrypt(bytes, false);
                    int length = cryptBytes.Length;
                    if (inStream.Position >= inStream.Length - KeyDecryptFileDataLength)
                        length = offset;
                    outStream.Write(cryptBytes, 0, length);
                }
            }
            return true;
        }


        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
