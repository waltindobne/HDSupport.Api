using EncryptionDecryptionUsingSymmetricKey;
using HD_Support_API.Components;
using HD_Support_API.Enums;
using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace HD_Support_API.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly BancoContext _contexto = new BancoContext();
        private readonly IEmailSender _SendEmailRepository;

        public UsuarioRepositorio(IEmailSender sendEmailRepository)
        {
            _SendEmailRepository = sendEmailRepository;
        }
        public async Task<Usuarios> AdicionarUsuario(Usuarios usuario)
        {
            string paternInvalidoEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if(!Regex.IsMatch(usuario.Email, paternInvalidoEmail))
            {
                throw new Exception("Endereço de e-mail inválido.");
            }
            if(usuario.Cargo.Contains("Gerente") || usuario.Cargo.Contains("Funcionario")  || usuario.Cargo.Contains("RH") || usuario.Cargo.Contains("HelpDesk")){
                if (usuario.Email.Contains("@employer.com.br") || usuario.Email.Contains("@bne-empregos.com.br"))
                {
                    var usuarioBancoComEmail = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
                    if(usuarioBancoComEmail == null)
                    {
                        usuario.Senha = AesOperation.CriarHash(usuario.Senha);
                        await _contexto.Usuarios.AddAsync(usuario);
                        await _contexto.SaveChangesAsync();
                        return usuario;
                    }
                    else
                    {
                        throw new Exception("Email já cadastrado");
                    }
                }
                else
                {
                    throw new Exception("Dominio do email invalido, insira seu email corporativo");
                }
                
            }  
            throw new Exception("Cargo inexistente ou invalido");
        }

        public async Task<bool> AtualizarStatus(int id, int status)
        {
            var busca = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (busca == null)
            {
                throw new Exception("ID não encontrado.");
            }
            StatusHelpDeskConversa statusConversa = (StatusHelpDeskConversa)status;
            busca.StatusConversa = statusConversa;
            _contexto.Usuarios.Update(busca);
            await _contexto.Usuarios.ToListAsync();
            return true;
        }

        public async Task<Usuarios> AtualizarUsuario(Usuarios usuario, int id)
        {
            string paternInvalidoEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Usuarios buscarId = await BuscarUsuarioPorId(id);

            if (buscarId == null)
            {
                throw new Exception($"Id:{id} não encontrado na base de dados.");
            }
            if (!Regex.IsMatch(usuario.Email, paternInvalidoEmail))
            {
                throw new Exception("Endereço de e-mail inválido.");
            }
            if (usuario.Email.Contains("@employer.com.br") || usuario.Email.Contains("@bne-empregos.com.br"))
            {
                buscarId.Senha = usuario.Senha;
                buscarId.Nome = usuario.Nome;
                buscarId.Email = usuario.Email;

                _contexto.Usuarios.Update(buscarId);
                await _contexto.SaveChangesAsync();

                return buscarId;
            }
            throw new Exception("Você não pode mudar seu e-mail para um e-mail não corporativo");
        }

        public async Task<int> BuscarPorEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Email vazio ou nulo.");
            }

            var Usuario = _contexto.Usuarios.FirstOrDefault(x => x.Email == email);

            if (Usuario == null)
            {
                throw new Exception("Nenhum HelpDesk encontrado com o email fornecido.");
            }

            return Usuario.Id;
        }


        public async Task<Usuarios> BuscarUsuario(string nome, string telefone)
        {
            var busca = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Nome == nome || x.Telefone == telefone);
            if (busca != null)
            {
                return busca;
            }
            throw new Exception("Usuário não encontrado.");
        }

        public async Task<Usuarios> BuscarUsuarioPorId(int id)
        {
            Usuarios usuario = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (usuario == null)
            {
                throw new Exception("ID não encontrado.");
            }
            return usuario;
        }

        public async Task<bool> ExcluirUsuario(int id)
        {
            Usuarios usuarioPorId = await BuscarUsuarioPorId(id);

            if (usuarioPorId == null)
            {
                throw new Exception($"HelpDesk de Id:{id} não encontrado na base de dados.");
            }

            Emprestimos emprestimo = _contexto.Emprestimo.FirstOrDefault(x => x.UsuarioId == id);
            if(emprestimo != null)
                _contexto.Remove(emprestimo);

            _contexto.Remove(usuarioPorId);
            await _contexto.SaveChangesAsync();

            return true;
        }

        public async Task<List<Usuarios>> ListarFuncionario()
        {
            var lista = await _contexto.Usuarios.Where(x => x.Cargo == "Funcionario" || x.Cargo == "RH").ToListAsync();
            return lista;
        }


        public async Task<List<Usuarios>> ListarHelpDesk()
        {
            var lista = await _contexto.Usuarios.Where(x => x.Cargo == "HelpDesk").ToListAsync();

            return lista;
        }

        public async Task<Usuarios> Login(string email, string senha)
        {
            var senhaHash = AesOperation.CriarHash(senha);
            var busca = await _contexto.Usuarios.FirstOrDefaultAsync(x => EF.Functions.Like(x.Email, email));

            if (busca != null && busca.Senha == senhaHash)
            {
                return busca;
            }
            throw new Exception("Login inválido");
        }


        public async Task RecuperarSenha(string email)
        {
            int idUsuario = await BuscarPorEmail(email);

            if (idUsuario == null)
            {
                throw new Exception("Nenhum usuário encontrado com o email fornecido.");
            }
            bool possuiTokenAtivo = await UsuarioPossuiTokenAtivo(idUsuario);
            if (possuiTokenAtivo)
            {
                throw new Exception("Já existe uma solicitação de redefinição de senha ativa para este usuário.");
            }

            string tokenRedefinicaoSenha = Guid.NewGuid().ToString();

            DateTime dataHoraGeracaoToken = DateTime.Now;
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Id == idUsuario);
            string dataHoraGeracaoTokenString = dataHoraGeracaoToken.ToString("yyyy-MM-dd HH:mm:ss.fff");
            usuario.TokenRedefinicaoSenha = tokenRedefinicaoSenha;
            usuario.DataHoraGeracaoToken = dataHoraGeracaoTokenString;
            await _contexto.SaveChangesAsync();

            var texto = $@"
                <html>
                    <head>
                        <style>
                            body {{font - family: Arial, sans-serif;
                                border: 1px solid #000000;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                height: 100vh;
                                margin: 0;
                                padding: 0;
                                text-align: center;
                            }}
                            header {{width: 100%;
                                background-color: #1E90FF;
                                color: #F0F8FF;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                            }}
                            .container {{padding: 20px;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                flex-direction: column;
                            }}
                            .link {{color: blue;
                                text-decoration: none;
                            }}
                        </style>
                    </head>
                    <body>
                        <header>
                            <div>
                                <h1>Redefinição de senha - HD-Support</h1>
                            </div>
                        </header>
                        <div class='container'>
                            <p>Olá,</p>
                            <p>Seu link de redefinição de senha é: <a class='link' href='localhost:3000/recuperacao/{idUsuario}?token={tokenRedefinicaoSenha}&geracaoToken={dataHoraGeracaoTokenString}'>Clique aqui</a>.</p>
                            <p>Não compartilhe seu link de redefinição, ele expirará dentro de 15 minutos.</p>
                            <p>Atenciosamente,<br>Equipe HD-Support</p>
                        </div>
                    </body>
                    </html>";

            await _SendEmailRepository.SendEmailAsync(email, "Redefinição de senha HD-Support", texto);
        }



        public async Task<IActionResult> RedefinirSenha(string token, string novaSenha,string confirmacaoSenha)
        {
            if(novaSenha != confirmacaoSenha)
            {
                return new BadRequestObjectResult("As senhas não conferem");
            }
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(novaSenha))
            {
                return new BadRequestObjectResult("Token ou nova senha não fornecidos.");
            }

            var usuario = await BuscarUsuarioPorToken(token);

            if (usuario == null)
            {
                return new BadRequestObjectResult("Token inválido ou expirado.");
            }

            if (TokenRedefinicaoSenhaExpirado(usuario.DataHoraGeracaoToken))
            {
                return new BadRequestObjectResult("Token de redefinição de senha expirado. Solicite um novo token.");
            }
            if(novaSenha == usuario.Senha) {
                return new BadRequestObjectResult("Você não pode utilizar uma senha que ja utilizou anteriormente");
            }
            usuario.Senha = AesOperation.CriarHash(novaSenha);
            usuario.TokenRedefinicaoSenha = null;

            await _contexto.SaveChangesAsync();
            return new OkResult();
        }


        private async Task<Usuarios> BuscarUsuarioPorToken(string token)
        {
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.TokenRedefinicaoSenha == token);
            return usuario;
        }

        private async Task<bool> UsuarioPossuiTokenAtivo(int idUsuario)
        {
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Id == idUsuario);
            if (usuario == null)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(usuario.TokenRedefinicaoSenha) &&
                !TokenRedefinicaoSenhaExpirado(usuario.DataHoraGeracaoToken))
            {
                return true;
            }

            return false;
        }
        private bool TokenRedefinicaoSenhaExpirado(string dataHoraGeracaoTokenString)
        {
            if (!DateTime.TryParse(dataHoraGeracaoTokenString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataHoraGeracaoToken))
            {
                return true;
            }

            DateTime agora = DateTime.Now;
            TimeSpan tempoExpiracao = TimeSpan.FromMinutes(15);
            DateTime tempoExpiracaoToken = dataHoraGeracaoToken.Add(tempoExpiracao);
            bool expirado = agora > tempoExpiracaoToken;
            return expirado;
        }

    }
}
