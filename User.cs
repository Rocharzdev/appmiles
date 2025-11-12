using System;

namespace MinhaApp.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Armazenaremos a senha com o formato: {iteracoes}.{saltBase64}.{hashBase64}
        public string SenhaProtegida { get; set; } = string.Empty;

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Para gravação em arquivo texto (CSV simples com ";")
        // Formato: Id;Nome;Email;SenhaProtegida;CriadoEmUtc
        public string ToFileLine()
        {
            // Por simplicidade, não permitimos ";" em Nome e Email (validaremos no cadastro)
            return $"{Id};{Nome};{Email};{SenhaProtegida};{CriadoEm:O}";
        }

        public static User? FromFileLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            var parts = line.Split(';');
            if (parts.Length < 5) return null;

            return new User
            {
                Id = Guid.TryParse(parts[0], out var id) ? id : Guid.NewGuid(),
                Nome = parts[1],
                Email = parts[2],
                SenhaProtegida = parts[3],
                CriadoEm = DateTime.TryParse(parts[4], null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt) ? dt : DateTime.UtcNow
            };
        }
    }
}
