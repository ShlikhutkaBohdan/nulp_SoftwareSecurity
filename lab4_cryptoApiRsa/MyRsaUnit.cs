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
        private const int KeySize = 1024;
        
        public string InputFilePath { get; set; }
        public string OutputFileFilePath { get; set; }

        public bool GenerateKeyPair(string publicKeyFilePath, string privateKeyFilePath)
        {
            try
            {
                using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(KeySize))
                {
                    byte[] keyData = rsaProvider.ExportCspBlob(false);//generate public key
                    File.WriteAllBytes(publicKeyFilePath, keyData);
                    keyData = rsaProvider.ExportCspBlob(true);//generate private key
                    File.WriteAllBytes(privateKeyFilePath, keyData);
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
                using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(KeySize))
                {
                    rsaProvider.ImportCspBlob(publicKey);
                    using (
                        FileStream plainMessageStream = new FileStream(InputFilePath, FileMode.Open),
                            encryptedMessageStream = new FileStream(OutputFileFilePath, FileMode.Create))
                    {
                        EncryptData(rsaProvider, plainMessageStream, encryptedMessageStream);
                        plainMessageStream.Close();
                        encryptedMessageStream.Close();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void EncryptData(RSACryptoServiceProvider provider, Stream plainMessage, Stream encryptedMessage)
        {
            var bytesBuffer = new byte[KeyCryptFileDataLength];
            int readedBytesCount = KeyCryptFileDataLength;
            byte offset = 0;//for last block
            byte[] encryptedBytesBuffer;
            while (readedBytesCount == KeyCryptFileDataLength)
            {
                readedBytesCount = plainMessage.Read(bytesBuffer, 0, KeyCryptFileDataLength);
                if (readedBytesCount == 0) break;
                if (readedBytesCount < KeyCryptFileDataLength)
                {
                    offset = (byte)readedBytesCount;
                }
                encryptedBytesBuffer = provider.Encrypt(bytesBuffer, false);
                encryptedMessage.Write(encryptedBytesBuffer, 0, encryptedBytesBuffer.Length);

                if (OnProgresChange != null)//firing event of changing progress
                {
                    OnProgresChange((int)(plainMessage.Position / (double)plainMessage.Length * 100));
                }
            }
            bytesBuffer[0] = offset;//write a length of last block
            encryptedBytesBuffer = provider.Encrypt(bytesBuffer, false);
            encryptedMessage.Write(encryptedBytesBuffer, 0, encryptedBytesBuffer.Length);
        }

        public bool Decrypt(byte[] privateKey)
        {
            try
            {
                using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(KeySize))
                {
                    rsaProvider.ImportCspBlob(privateKey);
                    using (
                        FileStream encryptedMessageStream = new FileStream(InputFilePath, FileMode.Open),
                            decryptedMessageStream = new FileStream(OutputFileFilePath, FileMode.Create))
                    {
                        DecryptData(rsaProvider, encryptedMessageStream, decryptedMessageStream);
                        encryptedMessageStream.Close();
                        decryptedMessageStream.Close();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void DecryptData(RSACryptoServiceProvider provider, Stream encryptedMessage, Stream decryptedMessage)
        {
            var bytesBuffer = new byte[KeyDecryptFileDataLength];
            int readedBytesCount = KeyDecryptFileDataLength;
            encryptedMessage.Position = encryptedMessage.Length - KeyDecryptFileDataLength;
            readedBytesCount = encryptedMessage.Read(bytesBuffer, 0, KeyDecryptFileDataLength);
            byte[] decryptedBytesBuffer = provider.Decrypt(bytesBuffer, false);
            byte offset = decryptedBytesBuffer[0]; //length of padding
            encryptedMessage.Position = 0;//move to start of stream
            while (encryptedMessage.Position < encryptedMessage.Length - KeyDecryptFileDataLength)
            {
                readedBytesCount = encryptedMessage.Read(bytesBuffer, 0, KeyDecryptFileDataLength);
                if (readedBytesCount == 0) break;
                decryptedBytesBuffer = provider.Decrypt(bytesBuffer, false);
                int length = decryptedBytesBuffer.Length;
                if (encryptedMessage.Position >= encryptedMessage.Length - KeyDecryptFileDataLength)//position on the last block (additional)
                    length = offset;
                decryptedMessage.Write(decryptedBytesBuffer, 0, length);

                if (OnProgresChange != null)
                {
                    OnProgresChange((int)(encryptedMessage.Position / (double)encryptedMessage.Length * 100));
                }
            }
        }


        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
