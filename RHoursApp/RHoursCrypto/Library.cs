using System;
using System.Text;

using NSec.Cryptography;

namespace RHours.Crypto
{
    public class CryptoProvider
    {
        public static Tuple<string, string> CreateKeyPair()
        {
            var algorithm = SignatureAlgorithm.Ed25519;

            var kp = new KeyCreationParameters
            {
                ExportPolicy = KeyExportPolicies.AllowPlaintextExport
            };

            using (var key = Key.Create(algorithm, kp))
            {
                var publicKeyBytes = key.Export(KeyBlobFormat.PkixPublicKeyText);
                var publicKey = Encoding.UTF8.GetString(publicKeyBytes);
                publicKey = publicKey.Replace("-----BEGIN PUBLIC KEY-----", "");
                publicKey = publicKey.Replace("-----END PUBLIC KEY-----", "");
                publicKey = publicKey.Replace(Environment.NewLine, "");

                var privateKeyBytes = key.Export(KeyBlobFormat.PkixPrivateKeyText);
                var privateKey = Encoding.UTF8.GetString(privateKeyBytes);
                privateKey = privateKey.Replace("-----BEGIN PRIVATE KEY-----", "");
                privateKey = privateKey.Replace("-----END PRIVATE KEY-----", "");
                privateKey = privateKey.Replace(Environment.NewLine, "");

                return new Tuple<string, string>(publicKey, privateKey);
            }
        }

        public static byte[] Hash(byte[] data)
        {
            var algorithm = HashAlgorithm.Blake2b_256;

            var hash = algorithm.Hash(new ReadOnlySpan<byte>(data));
            return hash;
        }

        public static byte[] Sign(string privateKey, byte[] data)
        {
            var algorithm = SignatureAlgorithm.Ed25519;

            var kp = new KeyCreationParameters
            {
                ExportPolicy = KeyExportPolicies.AllowPlaintextExport
            };

            var privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            using (Key key = Key.Import(algorithm, new ReadOnlySpan<byte>(privateKeyBytes), KeyBlobFormat.PkixPrivateKeyText, kp))
            {
                var signature = algorithm.Sign(key, data);
                return signature;
            }
        }

        public static bool Verify(string publicKey, byte[] data, byte[] signature)
        {
            var algorithm = SignatureAlgorithm.Ed25519;
            var publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);
            var pubkey = PublicKey.Import(algorithm, new ReadOnlySpan<byte>(publicKeyBytes), KeyBlobFormat.PkixPublicKeyText);

            var verified = algorithm.Verify(pubkey, new ReadOnlySpan<byte>(data), new ReadOnlySpan<byte>(signature));
            return verified;
        }

    }
}
