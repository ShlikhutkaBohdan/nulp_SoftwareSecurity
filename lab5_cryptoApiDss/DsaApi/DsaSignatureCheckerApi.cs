using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace lab5_cryptoApiDss.DsaApi
{
    public class DsaSignatureCheckerApi : IDisposable
    {
        public string EncryptedFilePath { get; set; }
        public string SignatureFilePath { get; set; }
        public string SaveFilePath { get; set; }

        public KeyFactory Keys { get; set; }

        public string ResultOutput { get; set; }

        public bool Process()
        {
            int keyLen = DsaSignatureCreatorApi.KeyLen;
            using (var ins = new FileStream(EncryptedFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var outs = new FileStream(SaveFilePath, FileMode.Create, FileAccess.Write))
                {
                    var rsa = new RSACryptoServiceProvider(keyLen);
                    rsa.ImportParameters(new RSAParameters
                    {
                        D = (byte[])Keys.RsaEncPrivate.D.Clone(),
                        DP = (byte[])Keys.RsaEncPrivate.DP.Clone(),
                        DQ = (byte[])Keys.RsaEncPrivate.DQ.Clone(),
                        Exponent = (byte[])Keys.RsaEncPrivate.Exponent.Clone(),
                        InverseQ = (byte[])Keys.RsaEncPrivate.InverseQ.Clone(),
                        Modulus = (byte[])Keys.RsaEncPrivate.Modulus.Clone(),
                        P = (byte[])Keys.RsaEncPrivate.P.Clone(),
                        Q = (byte[])Keys.RsaEncPrivate.Q.Clone()
                    });

                    var inbuf = new byte[keyLen / 8];
                    var hashAlgorithm = new SHA1CryptoServiceProvider();
                    hashAlgorithm.Initialize();

                    int blockSize = keyLen / 8;
                    var buffer = new byte[blockSize];
                    var iterations = (int)(ins.Length / blockSize);

                    for (var i = 0; i < iterations; i++)
                    {
                        if (ins.Read(buffer, 0, blockSize) <= 0) continue;
                        var decryptResult = rsa.Decrypt(buffer, false);
                        hashAlgorithm.TransformBlock(decryptResult, 0, decryptResult.Length, inbuf, 0);
                        outs.Write(decryptResult, 0, decryptResult.Length);
                    }

                    var outbuf = new byte[0];
                    hashAlgorithm.TransformFinalBlock(outbuf, 0, 0);

                    var dsa = new DSACryptoServiceProvider(keyLen);
                    var tmp2 = new DSAParameters
                    {
                        Counter = Keys.DsaSignPublic.Counter,
                        G = (byte[])Keys.DsaSignPublic.G.Clone(),
                        J = (Keys.DsaSignPublic.J == null ? null : (byte[])Keys.DsaSignPublic.J.Clone()),
                        P = (byte[])Keys.DsaSignPublic.P.Clone(),
                        Q = (byte[])Keys.DsaSignPublic.Q.Clone(),
                        Seed = (byte[])Keys.DsaSignPublic.Seed.Clone(),
                        Y = (Keys.DsaSignPublic.Y == null ? null : (byte[])Keys.DsaSignPublic.Y.Clone())
                    };
                    dsa.ImportParameters(tmp2);
                    AsymmetricSignatureDeformatter df = new DSASignatureDeformatter(dsa);
                    df.SetHashAlgorithm("SHA1");
                    var b = new StringBuilder(255);
                    using (var signStream = new FileStream(SignatureFilePath, FileMode.Open, FileAccess.Read))
                    {
                        var buff = new byte[signStream.Length];
                        signStream.Read(buff, 0, buff.Length);
                        b.Append("Підпис: ");
                        foreach (var b1 in buff)
                        {
                            b.AppendFormat("{0:X2}", b1);
                        }
                    }
                    b.Append("\nХеш SHA-1: ");
                    foreach (var b1 in hashAlgorithm.Hash)
                    {
                        b.AppendFormat("{0:X2}", b1);
                    }
                    ResultOutput = b.ToString();
                }
            }
            return true;
        }
        public void Dispose()
        {
            
        }
    }
}
