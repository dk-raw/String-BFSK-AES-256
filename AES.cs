using System;
using System.Security.Cryptography;
using System.Text;

namespace FSK
{
    partial class AES
    {
        public static byte[] Encrypt(string plainText, string secretKey)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.GenerateIV();

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                    byte[] combinedBytes = new byte[aes.IV.Length + encryptedBytes.Length];
                    Array.Copy(aes.IV, 0, combinedBytes, 0, aes.IV.Length);
                    Array.Copy(encryptedBytes, 0, combinedBytes, aes.IV.Length, encryptedBytes.Length);

                    return combinedBytes;
                }
            }
        }

        public static string Decrypt(byte[] encryptedBytes, string secretKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] iv = new byte[16];
            Array.Copy(encryptedBytes, 0, iv, 0, iv.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 16, encryptedBytes.Length - 16);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}
