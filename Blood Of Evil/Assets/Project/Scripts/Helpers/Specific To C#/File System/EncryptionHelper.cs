using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;

namespace BloodOfEvil.Helpers
{
    public static class EncryptionHelper
    {
        #region Fields
        /// <summary>
        /// C'est la clé de chiffrement.
        /// </summary>
        private static string Key = "This Key Will Be Later On A Server, Thanks you to read my code.";
        #endregion

        #region MD5 Encryption / Decryption
        /// <summary>
        /// Crypte une chaîne en MD5.
        /// </summary>
        public static string Encrypt(string source)
        {
            byte[] hashBytes;
            byte[] bufferBytes;
            TripleDESCryptoServiceProvider DESCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5CryptoProvider = new MD5CryptoServiceProvider();
            bufferBytes = Encoding.UTF8.GetBytes(source);

            hashBytes = MD5CryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(Key));
            DESCryptoProvider.Key = hashBytes;
            DESCryptoProvider.Mode = CipherMode.ECB;

            return Convert.ToBase64String(DESCryptoProvider.CreateEncryptor().TransformFinalBlock(bufferBytes, 0, bufferBytes.Length));
        }

        /// <summary>
        /// Décrypte du MD5, oui c'est pas logique et normal, mais je ne sais pas comment ce code fonctionne même si je les codé. (OMG).
        /// </summary>
        public static string Decrypt(string encodedText)
        {
            byte[] hashBytes;
            byte[] bufferBuffer;
            TripleDESCryptoServiceProvider DESCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5CryptoProvider = new MD5CryptoServiceProvider();
            bufferBuffer = Convert.FromBase64String(encodedText);

            hashBytes = MD5CryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(Key));
            DESCryptoProvider.Key = hashBytes;
            DESCryptoProvider.Mode = CipherMode.ECB;

            return Encoding.UTF8.GetString(DESCryptoProvider.CreateDecryptor().TransformFinalBlock(bufferBuffer, 0, bufferBuffer.Length));
        }

        public static string MD5(string data)
        {
            return ASCIIEncoding.ASCII.GetString(EncryptionHelper.MD5hash(Encoding.ASCII.GetBytes(data)));
        }

        private static byte[] MD5hash(byte[] data)
        {
            return new MD5CryptoServiceProvider().ComputeHash(data);
        }
        #endregion
    }
}