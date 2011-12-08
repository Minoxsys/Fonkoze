using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Services;
using System.Security.Cryptography;
using System.Text;

namespace Web.Security
{
    public class SecurePassword : ISecurePassword
    {
        private const string cryptoKey = "cryptoKey";
        // The Initialization Vector for the DES encryption routine
        private static readonly byte[] IV =
            new byte[8] { 240, 3, 45, 29, 0, 76, 173, 59 };

        public string EncryptPassword(string password)
        {
            if (password == null || password.Length == 0) return string.Empty;

            string result = string.Empty;
            byte[] buffer = Encoding.ASCII.GetBytes(password);

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();

            des.Key =  MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));
            des.IV = IV;
            result = Convert.ToBase64String(
                des.CreateEncryptor().TransformFinalBlock(
                    buffer, 0, buffer.Length));

            return result;
        }

        public string DecryptPassword(string cryptedPassword)
        {
            if (cryptedPassword == null || cryptedPassword.Length == 0) return string.Empty;

            string result = string.Empty;
            byte[] buffer = Convert.FromBase64String(cryptedPassword);

            TripleDESCryptoServiceProvider des =  new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();

            des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));

            des.IV = IV;
            result = Encoding.ASCII.GetString(
                des.CreateDecryptor().TransformFinalBlock(
                buffer, 0, buffer.Length));

            return result;
        }
    }
}