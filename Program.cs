using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IdentityV3PasswordHasher
{
    class Program
    {
        static void Main(string[] args)
        {
            //    if (args.Length == 0)
            //    {
            //        PrintUsage();
            //        return;
            //    }

            //    switch (args[0])
            //    {
            //        case "hash":
            //            if (args.Length != 2) {
            //                PrintUsage();
            //                break;
            //            }
            //            //Console.WriteLine(PasswordHasher.GenerateIdentityV3Hash(args[1]));
            //            Console.WriteLine(PasswordHasher.GenerateIdentityV3Hash("Password@123"));
            //            break;
            //        case "verify":                    
            //            if (args.Length != 3){
            //                PrintUsage();
            //                break;                        
            //            }
            //            //var password = args[1];
            //            var password = "Password@123";
            //            var identityV3Hash = args[2];
            //            Console.WriteLine(PasswordHasher.VerifyIdentityV3Hash(password, identityV3Hash) ? "Password is correct" : "Wrong password");
            //            break;
            //        default:
            //            PrintUsage();
            //            break;

            //    }
            Console.WriteLine(PasswordHasher.GenerateIdentityV3Hash("Password@1"));
            var res = PasswordHasher.GenerateIdentityV3Hash("Password@1");
            Console.WriteLine(PasswordHasher.VerifyIdentityV3Hash("Password@1", "AQAAAAEAACcQAAAAELv/EmAsTD5rXSF7iIlw5HSpiS0AJ62TtQglEar/4Tnrjw4iYC1d/nxuM7OP10EnKA==") ? "Password is correct" : "Wrong password");
            Console.WriteLine(PasswordHasher.VerifyIdentityV3Hash("Password@1", res) ? "Password is correct" : "Wrong password");

            //Console.WriteLine(PasswordHasher.HashPassword("Password@1"));
            //Console.WriteLine(PasswordHasher.HashPassword2("Password@1"));
            //Console.WriteLine(PasswordHasher.HashPassword3("Password@1"));
            //Console.WriteLine(PasswordHasher.HashPasswordV2("Password@1"));

            Console.ReadLine();
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: " + Environment.NewLine);
            Console.WriteLine("Generate ASP.NET Identity V3 Hash: hash [Password]");
            Console.WriteLine("Validate ASP.NET Identity V3 Password/Hash: verify [Password] [IdentityV3Hash]");
        }


    }
}
