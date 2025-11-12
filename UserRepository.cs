using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MinhaApp.Models;
using MinhaApp.Security;

namespace MinhaApp.Data
{
    public class UserRepository
    {
        private readonly string _filePath;
        private readonly object _fileLock = new object();

        public UserRepository(string? filePath = null)
        {
            _filePath = filePath ?? GetDefaultFilePath();
            EnsureFileExists();
        }

        private static string GetDefaultFilePath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "MinhaApp");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "cadastro.txt");
        }

        private void EnsureFileExists()
        {
            lock (_fileLock)
            {
                if (!File.Exists(_filePath))
                {
                    using var fs = File.Create(_filePath);
                }
            }
        }

        public IEnumerable<User> GetAll()
        {
            lock (_fileLock)
            {
                var lines = File.ReadAllLines(_filePath);
                foreach (var line in lines)
                {
                    var u = User.FromFileLine(line);
                    if (u != null) yield return u;
                }
            }
        }

        public User? GetByEmail(string email)
        {
            email = email.Trim().ToLowerInvariant();
            return GetAll().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public bool EmailExiste(string email)
        {
            return GetByEmail(email) != null;
        }

        public bool Cadastrar(string nome, string email, string senha, out string? erro)
        {
            erro = null;

            nome = (nome ?? "").Trim();
            email = (email ?? "").Trim().ToLowerInvariant();

            // Validações simples
            if (string.IsNullOrWhiteSpace(nome)) { erro = "Informe o nome."; return false; }
            if (string.IsNullOrWhiteSpace(email)) { erro = "Informe o e-mail."; return false; }
            if (!email.Contains("@") || email.StartsWith("@") || email.EndsWith("@")) { erro = "E-mail inválido."; return false; }
            if (email.Contains(";")) { erro = "E-mail não pode conter ponto e vírgula (;)."; return false; }
            if (nome.Contains(";")) { erro = "Nome não pode conter ponto e vírgula (;)."; return false; }
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6) { erro = "A senha deve ter pelo menos 6 caracteres."; return false; }

            if (EmailExiste(email)) { erro = "Já existe um usuário com esse e-mail."; return false; }

            // Gera hash protegido
            var protegido = PasswordHasher.Hash(senha);

            var user = new User
            {
                Nome = nome,
                Email = email,
                SenhaProtegida = protegido,
                CriadoEm = DateTime.UtcNow
            };

            var line = user.ToFileLine();

            lock (_fileLock)
            {
                File.AppendAllLines(_filePath, new[] { line });
            }

            return true;
        }

        public bool Login(string email, string senha, out User? usuario, out string? erro)
        {
            usuario = null;
            erro = null;

            email = (email ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email)) { erro = "Informe o e-mail."; return false; }
            if (string.IsNullOrWhiteSpace(senha)) { erro = "Informe a senha."; return false; }

            var u = GetByEmail(email);
            if (u == null)
            {
                erro = "Usuário não encontrado.";
                return false;
            }

            if (!PasswordHasher.Verify(senha, u.SenhaProtegida))
            {
                erro = "Senha inválida.";
                return false;
            }

            usuario = u;
            return true;
        }
    }
}
