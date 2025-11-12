using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsmilhasApp.Model
{
    public sealed class Usuario
    {
        public string Nome { get; set; } = string.Empty; // <- Novo
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty; // Em producao use hash!
        public string CPF { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public DateTime CreateUtc { get; set; } = DateTime.UtcNow;

        //Serializacao simples para linha de arquivo
        public string ToCsv()
            => $"usuario, {CPF},{Email},{Senha},{Nome},{Telefone},{CreateUtc:o}";

        //Parse seguro a partir de uma linha 
        public static bool TryParse(string line, out Usuario? usuario)
        {
            usuario = null;
            if (string.IsNullOrWhiteSpace(line)) return false;

            var p = line.Split(',');
            if (p.Length < 6) return false;
            if (!String.Equals(p[0], "usuario", StringComparison.OrdinalIgnoreCase)) return false;

            if (!DateTime.TryParse(p[6], null, System.Globalization.DateTimeStyles.RoundtripKind, out var created))
                created = DateTime.UtcNow;

            usuario = new Usuario
            {
                CPF = p[1],
                Email = p[2],
                Senha = p[3],
                Nome = p.Length >= 7 ? p[6] : string.Empty,  //compativel com linhas antigas sem nome
                Telefone = p[5],
                CreateUtc = created,
            };

            return true;
        }


    }

}
