using Microsoft.Extensions.Configuration;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.ServiceImplementation
{

    public class ServiceEncryptionDecryption : IEncryptionDecryption
    {
        private readonly IConfiguration _configuration;
        string keyString = "";


        public ServiceEncryptionDecryption(IConfiguration configuration)
        {
            _configuration = configuration;
         
        }

    


        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef"); // 32-byte key for AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("abcdef0123456789"); // 16-byte IV

        public async Task<string> PasswordDecryption(string encryptedPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedPassword))
                {
                    return string.Empty;
                }
                else
                {

                    using var aes =  Aes.Create();
                    aes.Key = Key;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using var decryptor = aes.CreateDecryptor();
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                    return  Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch(Exception ex)
            {
                throw ;
            }
        }

        public async Task<string> PasswordEncryption(string password)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    return string.Empty;
                }
                else
                {
                    using var aes = Aes.Create();
                    aes.Key = Key;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using var encryptor = aes.CreateEncryptor();
                    byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch(Exception ex)
            {
                throw ;
            }
        }
    }
}
