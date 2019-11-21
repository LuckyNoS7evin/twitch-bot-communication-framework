using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace backend.ExtensionMethods
{
    public static class StringEncryptionExtensions
    {
        public static string Encrypt(this string text, string keyString)
        {
            var key = Convert.FromBase64String(keyString);
            var textArray = Encoding.UTF8.GetBytes(text);
            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;          // in bits
                aes.Key = key;  // 32 bytes for 256 bit encryption
                aes.IV = key;   // AES needs a 16-byte IV
                                                            // Should set Key and IV here.  Good approach: derive them from 
                                                            // a password via Cryptography.Rfc2898DeriveBytes 
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

        public static string Decrypt(this string text, string keyString)
        {
            var key = Convert.FromBase64String(keyString);
            var textArray = Convert.FromBase64String(text);
            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;          // in bits
                aes.Key = key;  // 32 bytes for 256 bit encryption
                aes.IV = key;   // AES needs a 16-byte IV
                                // Should set Key and IV here.  Good approach: derive them from 
                                // a password via Cryptography.Rfc2898DeriveBytes 
                

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
