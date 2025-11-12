using AsmilhasApp.Model;
using UserController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;             

namespace UserController
{
    internal sealed class UserController
    {                                                    
        private readonly string _caminhoArquivo;
        private readonly object _lock = new();

        public UserController(string caminhoArquivo)
        {
            _caminhoArquivo = caminhoArquivo;
            GarantirArquivo();
        }

        private void GarantirArquivo()
        {
            var dir = Path.GetDirectoryName(_caminhoArquivo);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(_caminhoArquivo))
                File.WriteAllText(_caminhoArquivo, string.Empty);
        }

        public Usuario? ObterPorEmail(string email)
        {
            lock (_lock)
            {
                foreach (var linha in File.ReadAllLines(_caminhoArquivo))
                {
                    if (Usuario.TryParse(linha, out var u) &&
                        string.Equals(u!.Email, email, StringComparison.OrdinalIgnoreCase))
                    {
                        return u;
                    }
                }
            }
            return null;
        }

        public bool ExisteEmail(string email) => ObterPorEmail(email) != null;

        public void Adicionar(Usuario usuario)
        {
            lock (_lock)
                File.AppendAllLines(_caminhoArquivo, new[] { usuario.ToCsv() });
        }

        public bool AtualizarSenha(string email, string novaSenha)
        {
            var linhas = new List<string>();
            var alterou = false;

            lock (_lock)
            {
                foreach (var linha in File.ReadAllLines(_caminhoArquivo))
                {
                    if (Usuario.TryParse(linha, out var u) &&
                        string.Equals(u!.Email, email, StringComparison.OrdinalIgnoreCase))
                    {
                        u.Senha = novaSenha;
                        linhas.Add(u.ToCsv());
                        alterou = true;
                    }
                    else
                    {
                        linhas.Add(linha);
                    }
                }
            }
            if (alterou) File.WriteAllLines(_caminhoArquivo, linhas);
            return alterou;
        }

        public bool Excluir(string email)
        {
            var linhas = new List<string>();
            var removeu = false;
            lock (_lock)
            {
                foreach (var linha in File.ReadAllLines(_caminhoArquivo))
                {
                    if (Usuario.TryParse(linha, out var u) &&
                        string.Equals(u!.Email, email, StringComparison.OrdinalIgnoreCase))
                    {
                        removeu = true; //nao adiciona
                        continue;
                    }
                    linhas.Add(linha);
                }
                if (removeu) File.WriteAllLines(_caminhoArquivo, linhas);
            }
            return removeu;
        }
    }
}
