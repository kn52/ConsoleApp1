using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleApp1.Features
{

    public class BillAvenue
    {
        [JsonPropertyName("billerId")]
        public string BillerId { get; set; }
        [JsonPropertyName("circle")]
        public string Circle { get; set; }
    }
    public static class EncryptDecrypt
    {
        public static void Test()
        {
            string plainText = "9d845434dea7f3399ab6e33e45b5e09b725084a174f8903559b7be279949d71ab3aaffbbe3b5d880cc3fe7895e78b4ba66ab897845744b924545e0df5b594ca9";
            string encKey = "E53464862026EC4BCD081D0024EB0037";
            var obj = new BillAvenue
            {
                BillerId = "BILAVJIO000001",
                Circle = "MUMBAI"
            };
            var sobj = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            var decryptText = Decrypt(plainText, encKey);
            var encryptText = Encrypt(sobj, encKey);
        }
        public static string Decrypt(string encryptedHex, string workingKey)
        {
            byte[] keyBytes = GetMd5Hex(workingKey);
            byte[] ivBytes = GetIV();
            byte[] encryptedBytes = HexToByteArray(encryptedHex);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        public static string Encrypt(string plainText, string workingKey)
        {
            byte[] keyBytes = GetMd5Hex(workingKey);
            byte[] ivBytes = GetIV();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return ByteArrayToHex(encryptedBytes);
                }
            }
        }

        private static byte[] GetMd5Hex(string workingKey)
        {
            using var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(workingKey));
            return hash;
        }
        private static byte[] GetIV()
        {
            byte[] iv = [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f];
            return iv;
        }
        private static byte[] HexToByteArray(string encryptedText)
        {
            byte[] encryptedTextBytes = new byte[encryptedText.Length / 2];
            for (int i = 0; i < encryptedText.Length; i += 2)
            {
                encryptedTextBytes[i / 2] = Convert.ToByte(encryptedText.Substring(i, 2), 16);
            }
            return encryptedTextBytes;
        }
        private static string ByteArrayToHex(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
