using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IdentityV3PasswordHasher
{
    public static class PasswordHasher
    {
        //prf=HMACSHA256
        //iterationcount=10000
        //saltsize=16
        public static string GenerateIdentityV3Hash(string password, KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256, int iterationCount = 10000, int saltSize = 16)
        {
            using (var rng = RandomNumberGenerator.Create()){
                var salt = new byte[saltSize];
                rng.GetBytes(salt);
                
                var pbkdf2Hash = KeyDerivation.Pbkdf2(password, salt, prf, iterationCount, 32);
                return Convert.ToBase64String(ComposeIdentityV3Hash(salt, (uint)iterationCount, pbkdf2Hash));
            }
        }

        private static byte[] ComposeIdentityV3Hash(byte[] salt, uint iterationCount, byte[] passwordHash)
        {
            var hash = new byte[1 + 4/*KeyDerivationPrf value*/ + 4/*Iteration count*/ + 4/*salt size*/ + salt.Length /*salt*/ + 32 /*password hash size*/];
            hash[0] = 1; //Identity V3 marker

            Buffer.BlockCopy(ConvertToNetworkOrder((uint)KeyDerivationPrf.HMACSHA256), 0, hash, 1, sizeof(uint));
            Buffer.BlockCopy(ConvertToNetworkOrder((uint)iterationCount), 0, hash, 1 + sizeof(uint), sizeof(uint));
            Buffer.BlockCopy(ConvertToNetworkOrder((uint)salt.Length), 0, hash, 1 + 2 * sizeof(uint), sizeof(uint));
            Buffer.BlockCopy(salt, 0, hash, 1 + 3 * sizeof(uint), salt.Length);
            Buffer.BlockCopy(passwordHash, 0, hash, 1 + 3 * sizeof(uint) + salt.Length, passwordHash.Length);

            return hash;
        }
        public static bool VerifyIdentityV3Hash(string password, string passwordHash)
        {
            var identityV3HashArray = Convert.FromBase64String(passwordHash);
            if (identityV3HashArray[0] != 1) throw new InvalidOperationException("passwordHash is not Identity V3");

            var prfAsArray = new byte[4];
            Buffer.BlockCopy(identityV3HashArray, 1, prfAsArray, 0, 4);
            var prf = (KeyDerivationPrf)ConvertFromNetworOrder(prfAsArray);

            var iterationCountAsArray = new byte[4];
            Buffer.BlockCopy(identityV3HashArray, 5, iterationCountAsArray, 0, 4);
            var iterationCount = (int)ConvertFromNetworOrder(iterationCountAsArray);

            var saltSizeAsArray = new byte[4];
            Buffer.BlockCopy(identityV3HashArray, 9, saltSizeAsArray, 0, 4);
            var saltSize = (int)ConvertFromNetworOrder(saltSizeAsArray);

            var salt = new byte[saltSize];
            Buffer.BlockCopy(identityV3HashArray, 13, salt, 0, saltSize);

            var savedHashedPassword = new byte[identityV3HashArray.Length - 1 - 4 - 4 - 4 - saltSize];
            Buffer.BlockCopy(identityV3HashArray, 13 + saltSize, savedHashedPassword, 0, savedHashedPassword.Length);

            var hashFromInputPassword = KeyDerivation.Pbkdf2(password, salt, prf, iterationCount, 32);

            return AreByteArraysEqual(hashFromInputPassword, savedHashedPassword);
        }

        private static bool AreByteArraysEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length) return false;

            var areEqual = true;
            for (var i = 0; i < array1.Length; i++)
            {
                areEqual &= (array1[i] == array2[i]);
            }
            //If you stop as soon as the arrays don't match you'll be disclosing information about how different they are by the time it takes to compare them
            //this way no information is disclosed
            return areEqual;
        }

        private static byte[] ConvertToNetworkOrder(uint number)
        {
            return BitConverter.GetBytes(number).Reverse().ToArray();
        }

        private static uint ConvertFromNetworOrder(byte[] reversedUint)
        {
            return BitConverter.ToUInt32(reversedUint.Reverse().ToArray(), 0);
        }

    }
}