using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab5_cryptoApiDss
{
    public class DssUnit : IDisposable
    {
        public bool GenerateKeyPair(string pubPath, string privPath)
        {
            try
            {
                using (DSACryptoServiceProvider dsaProvider = new DSACryptoServiceProvider())
                {

                    DSAParameters publicKey = dsaProvider.ExportParameters(false);//pub
                    var binformt = new BinaryFormatter();
                    using (var fs = new FileStream(pubPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                    {
                        //binformt.Serialize(fs, publicKey.Counter);

                        binformt.Serialize(fs, publicKey.Q);
                        binformt.Serialize(fs, publicKey.Seed);
                        binformt.Serialize(fs, publicKey.G);
                        binformt.Serialize(fs, publicKey.P);
                        binformt.Serialize(fs, publicKey.Counter);
                        binformt.Serialize(fs, publicKey.Y);
                        //binformt.Serialize(fs, publicKey.X);
                        binformt.Serialize(fs, publicKey.J);
                        fs.Close();
                    }
                    //File.WriteAllBytes(pubPath, memoryStream.GetBuffer());

                    DSAParameters privateKey = dsaProvider.ExportParameters(true);//generate private key
                    var binformt1 = new BinaryFormatter();
                    //var memoryStream1 = new MemoryStream();
                    using (var fs = new FileStream(privPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                    {
                        binformt.Serialize(fs, privateKey.Q);
                        binformt.Serialize(fs, privateKey.Seed);
                        binformt.Serialize(fs, privateKey.G);
                        binformt.Serialize(fs, privateKey.P);
                        binformt.Serialize(fs, privateKey.Counter);
                        binformt.Serialize(fs, privateKey.Y);
                        binformt.Serialize(fs, privateKey.X);
                        binformt.Serialize(fs, privateKey.J);
                        fs.Close();
                    }
                    //File.WriteAllBytes(privPath, memoryStream1.GetBuffer());
                    //File.WriteAllBytes(privPath, keyData);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }

        public DSAParameters PublicKey { get; set; }
        public DSAParameters PrivateKey { get; set; }

        public bool OpenKeyPair(string pubPath, string privPath)
        {
            try
            {
                if (pubPath != null)
                {
                    using (var fs = new FileStream(pubPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var bf = new BinaryFormatter();
                        //byte[] w = (byte[]) bf.Deserialize(fs);
                        DSAParameters publicKey = new DSAParameters()
                        {
                            Q = (byte[])bf.Deserialize(fs),
                            Seed = (byte[])bf.Deserialize(fs),
                            G = (byte[])bf.Deserialize(fs),
                            P = (byte[])bf.Deserialize(fs),
                            Counter = (int)bf.Deserialize(fs),
                            Y = (byte[])bf.Deserialize(fs),
                            //X = (byte[])bf.Deserialize(fs),
                            J = (byte[])bf.Deserialize(fs),
                        };
                        fs.Close();
                        PublicKey = publicKey;
                    }

                }
                if (privPath != null)
                {
                    using (var fs = new FileStream(privPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var bf = new BinaryFormatter();
                        DSAParameters privateKey = new DSAParameters()
                        {
                            Q = (byte[])bf.Deserialize(fs),
                            Seed = (byte[])bf.Deserialize(fs),
                            G = (byte[])bf.Deserialize(fs),
                            P = (byte[])bf.Deserialize(fs),
                            Counter = (int)bf.Deserialize(fs),
                            Y = (byte[])bf.Deserialize(fs),
                            X = (byte[])bf.Deserialize(fs),
                            J = (byte[])bf.Deserialize(fs),
                        };
                        fs.Close();
                        PrivateKey = privateKey;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
            
            
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] arr)
        {
            var stringBuilder = new StringBuilder();
            foreach (byte t in arr)
            {
                stringBuilder.Append(t.ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        public string CreateSignatureForStream(Stream stream)
        {
            byte[] hash = (new SHA1Managed()).ComputeHash(stream);//for file or text
            try
            {
                using (var dsaCryptoProvider = new DSACryptoServiceProvider())
                {
                    dsaCryptoProvider.ImportParameters(PrivateKey);
                    var dsaFormatter = new DSASignatureFormatter(dsaCryptoProvider);
                    dsaFormatter.SetHashAlgorithm("SHA1");
                    byte[] signature = dsaFormatter.CreateSignature(hash);
                    return ByteArrayToString(signature);
                }
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        public bool VerifySignature(Stream stream, string signature)
        {
            try
            {
                using (var dsaCryptoProvider = new DSACryptoServiceProvider())
                {
                    byte[] hash = (new SHA1Managed()).ComputeHash(stream);//for file or text
                    dsaCryptoProvider.ImportParameters(PublicKey);
                    var dsaDeformatter = new DSASignatureDeformatter(dsaCryptoProvider);
                    dsaDeformatter.SetHashAlgorithm("SHA1");
                    return dsaDeformatter.VerifySignature(hash, StringToByteArray(signature));
                }
            }
            catch (CryptographicException e)
            {
                return false;
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
