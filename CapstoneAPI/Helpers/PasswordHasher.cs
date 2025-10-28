/*

Handles pwd hashing and verification
--------------------------------------------------------------------------------------
Ensures that user passwords are never stored in plaintext. Uses standards-based 
hashing algorithm called PBKDF2 built into .NET

*/

using System.Security.Cryptography;

namespace CapstoneAPI.Helpers
{
    public static class PasswordHasher
    {
        private const int Iterations = 100_000;   // increase as hardware gets faster
        private const int SaltSize = 16;          // 128-bit salt
        private const int KeySize = 32;           // 256-bit derived key

        // creates a secure random salt for this pwd using a cryptographically strong RNG
        // salt ensures that even if 2 users have the same pwd, their hashes will be different
        public static string Hash(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            // uses SBKDF2-HMAC-SHA256, takes the password + salt and derives a fixed-length hash
            // uses Iterations times to slow down brute force
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(KeySize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }
        
        // splits thge stored string into its 3 components: iterations, salt, hash
        // returns false if the format isnt valid
        public static bool Verify(string password, string encoded)
        {
            var parts = encoded.Split('.', 3);
            if (parts.Length != 3) return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var expected = Convert.FromBase64String(parts[2]);

            // re runs the PBKDF2 algorithm with the provided pwd and stored salt
            // generates a new derived key (actual) of the same length as the stored
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var actual = pbkdf2.GetBytes(expected.Length);

            // constant-time comparison
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
    }
}