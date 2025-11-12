using System;
using System.Security.Cryptography;
using System.Text;

namespace MinhaApp.Security
{
    public static class PasswordHasher
    {
        // PBKDF2 com HMACSHA256
        private const int SaltSize = 16;         // 128 bits
        private const int KeySize = 32;          // 256 bits
        private const int Iterations = 100_000;  // ajuste conforme necessidade

        public static string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            // Gera sal
            var salt = RandomNumberGenerator.GetBytes(SaltSize);

            // Deriva chave
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(KeySize);

            // Formato: iteracoes.saltBase64.hashBase64
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string protectedPassword)
        {
            if (string.IsNullOrWhiteSpace(protectedPassword)) return false;

            var parts = protectedPassword.Split('.');
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var storedHash = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(storedHash.Length);

            return CryptographicOperations.FixedTimeEquals(storedHash, computed);
        }
    }
}
