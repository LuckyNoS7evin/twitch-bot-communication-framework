using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace backend.ExtensionMethods
{
    public static class StringEncryptionExtensions
    {
        public static string Encrypt(this string text, string secretString, string clientIdString)
        {
            var key = Convert.FromBase64String(secretString);
            var iv = Convert.FromBase64String(clientIdString);
            var textArray = Encoding.UTF8.GetBytes(text);
            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 256;          // in bits
                aes.Key = key;  // 32 bytes for 256 bit encryption
                aes.IV = iv;   // AES needs a 16-byte IV

                byte[] cipherText = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(textArray, 0, textArray.Length);
                    }

                    cipherText = ms.ToArray();
                }

                return Convert.ToBase64String(cipherText);
            }
            
        }

        public static string Decrypt(this string text, string secretString, string clientIdString)
        {
            var key = Convert.FromBase64String(secretString);
            var iv = Convert.FromBase64String(clientIdString);
            var textArray = Convert.FromBase64String(text);
            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 256;          // in bits
                aes.Key = key;  // 32 bytes for 256 bit encryption
                aes.IV = iv;   // AES needs a 16-byte IV

                byte[] plainText = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(textArray, 0, textArray.Length);
                    }

                    plainText = ms.ToArray();
                }
                return Encoding.UTF8.GetString(plainText);
            }
        }
    }
}
