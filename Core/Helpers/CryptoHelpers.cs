using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helpers
{
    public class CryptoHelpers
    {
        public static byte[] CreateRandom(int size)
        {
            //Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);
            return buff;
        }

        public static byte[] Hash(string value, byte[] salt)
        {
            return Hash(Encoding.UTF8.GetBytes(value), salt);
        }

        public static byte[] Hash(byte[] value, byte[] salt)
        {
            byte[] saltedValue = value.Concat(salt).ToArray();
            return new SHA256Managed().ComputeHash(saltedValue);
        }


        public static bool ConfirmPassword(string password, byte[] storedPassword, byte[] salt)
        {
            byte[] passwordHash = Hash(password, salt);
            return storedPassword.SequenceEqual(passwordHash);
        }
    }
}
