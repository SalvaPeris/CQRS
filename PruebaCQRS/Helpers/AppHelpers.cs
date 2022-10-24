﻿using HashidsNet;

namespace PruebaCQRS.Helpers
{
    public static class AppHelpers
    {
        private static Hashids GetHasher() => new(HashIdsSalt, 8);

        public const string HashIdsSalt = "s3cret_s4lt";

        public static string ToHashId(this int number) => 
            GetHasher().Encode(number);

        public static int FromHashId(this string encoded) =>
            GetHasher().Decode(encoded).FirstOrDefault();
    }
}
