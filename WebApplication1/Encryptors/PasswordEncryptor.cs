using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Encryptors
{
    public class PasswordEncryptor
    {
        public static String CreateSalt()
        {
            byte[] rand = new byte[16];
            using (var gen = RandomNumberGenerator.Create())
            {
                gen.GetBytes(rand);
                return Convert.ToBase64String(rand);
            }
        }

        public static String Create (string pass, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2
            (
                password: pass,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 256/8
            );
            return Convert.ToBase64String(valueBytes);
        }

        public static bool Validate (string pass, string salt, string hash)
        {
            return Create(pass, salt) == hash;
        }

    }

}