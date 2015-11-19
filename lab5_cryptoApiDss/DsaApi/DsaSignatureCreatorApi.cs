using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace lab5_cryptoApiDss
{
    public class DsaSignatureCreatorApi
    {
        public static readonly int KeyLen = 1024;


        public string SaveFilePath { get; set; }

        public string SignatureFilePath { get; set; }

        public KeyFactory Keys { get; set; }

        public bool CreateSignature(Stream stream)
        {
            try
            {
                using (var outs = new FileStream(SaveFilePath, FileMode.Create, FileAccess.Write))
                {//file out (crypted)
                    var rsa = new RSACryptoServiceProvider(KeyLen);
                    rsa.ImportParameters(new RSAParameters
                    {
                        Exponent = (byte[])Keys.RsaEncPrivate.Exponent.Clone(),
                        Modulus = (byte[])Keys.RsaEncPrivate.Modulus.Clone()
                    });
                    var inbuf = new byte[KeyLen / 8];

                    var hashAlgorithm = new SHA1CryptoServiceProvider();//hash
                    hashAlgorithm.Initialize();

                    int blockSize = (KeyLen / 8) - 11;
                    var buffer = new byte[blockSize];
                    int readed;
                    while ((readed = stream.Read(buffer, 0, blockSize)) == blockSize)
                    {
                        hashAlgorithm.TransformBlock(buffer, 0, buffer.Length, inbuf, 0);
                        var encRes = rsa.Encrypt(buffer, false);
                        outs.Write(encRes, 0, encRes.Length);
                    }
                    var appendix = new byte[readed];
                    Array.Copy(buffer, appendix, readed);
                    hashAlgorithm.TransformFinalBlock(appendix, 0, appendix.Length);
                    var enc = rsa.Encrypt(appendix, false);
                    outs.Write(enc, 0, enc.Length);

                    var dsa = new DSACryptoServiceProvider(KeyLen);
                    dsa.ImportParameters(new DSAParameters
                    {
                        Counter = Keys.DsaSignPrivate.Counter,
                        G = (byte[])Keys.DsaSignPrivate.G.Clone(),
                        J = (Keys.DsaSignPrivate.J == null ? null : (byte[])Keys.DsaSignPrivate.J.Clone()),
                        P = (byte[])Keys.DsaSignPrivate.P.Clone(),
                        Q = (byte[])Keys.DsaSignPrivate.Q.Clone(),
                        Seed = (byte[])Keys.DsaSignPrivate.Seed.Clone(),
                        X = (byte[])Keys.DsaSignPrivate.X.Clone(),
                        Y = (byte[])Keys.DsaSignPrivate.Y.Clone()
                    });
                    AsymmetricSignatureFormatter sf = new DSASignatureFormatter(dsa);

                    sf.SetHashAlgorithm("SHA1");
                    byte[] outbuf = sf.CreateSignature(hashAlgorithm.Hash);
                    var b = new StringBuilder(outbuf.Length * 2 + 2);
                    foreach (var b1 in outbuf)
                    {
                        b.AppendFormat("{0:X2}", b1);
                    }

                    using (var signStream = new FileStream(SignatureFilePath, FileMode.Create,
                                                            FileAccess.Write))
                    {
                        signStream.Write(outbuf, 0, outbuf.Length);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        

    }
}