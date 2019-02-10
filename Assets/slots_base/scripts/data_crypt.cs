using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace crypt
{
    public class data
    {
        private static string uniqueKey = "unicorn";
        public static string Encrypt(string str)
        {
            return Encrypt(str, uniqueKey + DeviceID());
        }
        public static string Decrypt(string str)
        {
            return Decrypt(str, uniqueKey + DeviceID());
        }
        /// /// Шифрует строку value
        /// 
        /// Строка которую необходимо зашифровать
        /// Ключ шифрования
        private static string Encrypt(string str, string keyCrypt)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(str), keyCrypt));
        }

        /// /// Расшифроывает данные из строки value
        /// 
        /// Зашифрованая строка
        /// Ключ шифрования
        /// Возвращает null, если прочесть данные не удалось
        [DebuggerNonUserCodeAttribute]
        private static string Decrypt(string str, string keyCrypt)
        {
            string Result;
            try
            {
                CryptoStream Cs = InternalDecrypt(Convert.FromBase64String(str), keyCrypt);
                StreamReader Sr = new StreamReader(Cs);

                Result = Sr.ReadToEnd();

                Cs.Close();
                Cs.Dispose();

                Sr.Close();
                Sr.Dispose();
            }
            catch (CryptographicException)
            {
                return null;
            }

            return Result;
        }

        private static byte[] Encrypt(byte[] key, string value)
        {
            SymmetricAlgorithm Sa = Rijndael.Create();
            ICryptoTransform Ct = Sa.CreateEncryptor((new PasswordDeriveBytes(value, null)).GetBytes(16), new byte[16]);

            MemoryStream Ms = new MemoryStream();
            CryptoStream Cs = new CryptoStream(Ms, Ct, CryptoStreamMode.Write);

            Cs.Write(key, 0, key.Length);
            Cs.FlushFinalBlock();

            byte[] Result = Ms.ToArray();

            Ms.Close();
            Ms.Dispose();

            Cs.Close();
            Cs.Dispose();

            Ct.Dispose();

            return Result;
        }

        private static CryptoStream InternalDecrypt(byte[] key, string value)
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            ICryptoTransform ct = sa.CreateDecryptor((new PasswordDeriveBytes(value, null)).GetBytes(16), new byte[16]);

            MemoryStream ms = new MemoryStream(key);
            return new CryptoStream(ms, ct, CryptoStreamMode.Read);
        }

        private static string DeviceID()
        {
#if UNITY_IOS && !UNITY_EDITOR
                return UnityEngine.iOS.Device.advertisingIdentifier;
#elif UNITY_ANDROID
        return SystemInfo.deviceUniqueIdentifier;
#else

            return "DeviceID-Project";
#endif
        }
    }
}