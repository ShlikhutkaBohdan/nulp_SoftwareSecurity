using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace lab5_cryptoApiDss
{
    public class DsaApi
    {
        private const int KeyLen = 1024;
        public class KeyFactory
        {
            public string RsaPublicKeyPath { get; set; }
            public string RsaPrivateKeyPath { get; set; }
            public string DsaPublicKeyPath { get; set; }
            public string DsaPrivateKeyPath { get; set; }

            public DSAParameters DsaSignPrivate { get; set; }
            public DSAParameters DsaSignPublic { get; set; }
            public RSAParameters RsaEncPrivate { get; set; }
            public RSAParameters RsaEncPublic { get; set; }

            public bool GenerateKeysPairs()
            {
                try
                {
                    using (var dsa = new DSACryptoServiceProvider(KeyLen))
                    {
                        DsaSignPrivate = dsa.ExportParameters(true);
                        DsaSignPublic = dsa.ExportParameters(false);
                        using (var stream = new FileStream(DsaPublicKeyPath, FileMode.Create, FileAccess.Write))
                        {//dsa public
                            new XmlSerializer(DsaSignPublic.GetType()).Serialize(stream, DsaSignPublic);
                        }
                        using (var stream = new FileStream(DsaPrivateKeyPath, FileMode.Create, FileAccess.Write))
                        {//dsa private
                            new XmlSerializer(DsaSignPrivate.GetType()).Serialize(stream, DsaSignPrivate);
                        }
                    }
                    using (var rsa = new RSACryptoServiceProvider(KeyLen))
                    {
                        RsaEncPrivate = rsa.ExportParameters(true);
                        RsaEncPublic = rsa.ExportParameters(false);
                        using (var stream = new FileStream(RsaPrivateKeyPath, FileMode.Create, FileAccess.Write))
                        {//rsa private
                            new XmlSerializer(RsaEncPrivate.GetType()).Serialize(stream, RsaEncPrivate);
                        }
                        using (var stream = new FileStream(RsaPublicKeyPath, FileMode.Create, FileAccess.Write))
                        {//rsa public
                            new XmlSerializer(RsaEncPublic.GetType()).Serialize(stream, RsaEncPublic);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }

            bool LoadKeysPairs()
            {
                try
                {
                    using (var stream = new FileStream(DsaPrivateKeyPath, FileMode.Open, FileAccess.Read))
                    {
                        DsaSignPrivate = (DSAParameters)new XmlSerializer(typeof(DSAParameters)).Deserialize(stream);
                    }
                    using (var stream = new FileStream(DsaPublicKeyPath, FileMode.Open, FileAccess.Read))
                    {
                        DsaSignPublic = (DSAParameters)new XmlSerializer(typeof(DSAParameters)).Deserialize(stream);
                    }
                    using (var stream = new FileStream(RsaPrivateKeyPath, FileMode.Open, FileAccess.Read))
                    {
                        RsaEncPrivate = (RSAParameters)new XmlSerializer(typeof(RSAParameters)).Deserialize(stream);
                    }
                    using (var stream = new FileStream(RsaPublicKeyPath, FileMode.Open, FileAccess.Read))
                    {
                        RsaEncPublic = (RSAParameters)new XmlSerializer(typeof(RSAParameters)).Deserialize(stream);
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }

        }

    }
}