using EncryptionDecryptionUsingSymmetricKey;
using HD_Support_API.Components;
using HD_Support_API.Enums;
using HD_Support_API.Metodos;
using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;

namespace HD_Support_API.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly BancoContext _contexto;
        private readonly IEmailSender _SendEmailRepository;

        public UsuarioRepositorio(BancoContext contexto, IEmailSender sendEmailRepository)
        {
            _contexto = contexto;
            _SendEmailRepository = sendEmailRepository;
        }

        public async Task<Usuarios> AdicionarUsuario(Usuarios usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "O usuário não pode ser nulo");
            }

            if (string.IsNullOrEmpty(usuario.Eml_Usuario))
            {
                throw new ArgumentException("O email não pode ser vazio", nameof(usuario.Eml_Usuario));
            }

            string paternInvalidoEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if(!Regex.IsMatch(usuario.Eml_Usuario, paternInvalidoEmail))
            {
                throw new Exception("Endereço de e-mail inválido.");
            }

            if(usuario.Cargo_Usuario.Contains("Gerente") || usuario.Cargo_Usuario.Contains("Funcionario")  || usuario.Cargo_Usuario.Contains("RH") || usuario.Cargo_Usuario.Contains("HelpDesk")){
                if (usuario.Eml_Usuario.Contains("@employer.com.br") || usuario.Eml_Usuario.Contains("@bne-empregos.com.br"))
                {
                    var usuarioBancoComEmail = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Eml_Usuario == usuario.Eml_Usuario);
                    if(usuarioBancoComEmail == null)
                    {
                        usuario.Sen_Usuario = AesOperation.CriarHash(usuario.Sen_Usuario);
                        var emailEnvio = usuario.Eml_Usuario;
                        usuario.Eml_Usuario = "naoConfirmado";
                        usuario.Status_Usuario = StatusHelpDesk.naoConfirmado;
                        await _contexto.Usuarios.AddAsync(usuario);
                        await _contexto.SaveChangesAsync();
                        var idNovo = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Eml_Usuario == usuario.Eml_Usuario);
                        
                        try 
                        {
                            await RedefinirEmail(emailEnvio, idNovo.Id);
                        }
                        catch (Exception ex)
                        {
                            // Se falhar ao enviar o email, removemos o usuário criado
                            _contexto.Usuarios.Remove(usuario);
                            await _contexto.SaveChangesAsync();
                            throw new Exception($"Erro ao enviar email de confirmação: {ex.Message}");
                        }
                        
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
            busca.Status_Conversa = statusConversa;
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
                throw new Exception($"id:{id} não encontrado na base de dados.");
            }
            if (!Regex.IsMatch(usuario.Eml_Usuario, paternInvalidoEmail))
            {
                throw new Exception("Endereço de e-mail inválido.");
            }

            buscarId.Nme_Usuario = usuario.Nme_Usuario;
            buscarId.Tel_Usuario = usuario.Tel_Usuario;
            buscarId.Img_Usuario = usuario.Img_Usuario;

            _contexto.Usuarios.Update(buscarId);
            await _contexto.SaveChangesAsync();

            return buscarId;
        }

        public async Task<int> BuscarPorEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Email vazio ou nulo.");
            }

            var Usuario = _contexto.Usuarios.FirstOrDefault(x => x.Eml_Usuario == email);

            if (Usuario == null)
            {
                throw new Exception("Nenhum usuário encontrado com o email fornecido.");
            }

            return Usuario.Id;
        }


        public async Task<Usuarios> BuscarUsuario(string nome, string telefone)
        {
            var busca = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Nme_Usuario == nome || x.Tel_Usuario == telefone);
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
                throw new Exception($"HelpDesk de id:{id} não encontrado na base de dados.");
            }

            Emprestimos emprestimo = _contexto.Emprestimo.FirstOrDefault(x => x.Idf_Usuario == id);
            if(emprestimo != null)
                _contexto.Remove(emprestimo);

            _contexto.Remove(usuarioPorId);
            await _contexto.SaveChangesAsync();

            return true;
        }

        public async Task<List<Usuarios>> ListarFuncionario()
        {
            var lista = await _contexto.Usuarios.Where(x => x.Cargo_Usuario == "Funcionario" && x.Status_Usuario != StatusHelpDesk.naoConfirmado || x.Cargo_Usuario == "RH" && x.Status_Usuario != StatusHelpDesk.naoConfirmado).ToListAsync();
            return lista;
        }


        public async Task<List<Usuarios>> ListarHelpDesk()
        {
            var lista = await _contexto.Usuarios.Where(x => x.Cargo_Usuario == "HelpDesk" && x.Status_Usuario != StatusHelpDesk.naoConfirmado).ToListAsync();

            return lista;
        }

        public async Task<Usuarios> Login(string email, string senha)
        {
            var senhaHash = AesOperation.CriarHash(senha);
            var busca = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Eml_Usuario == email);

            if (busca != null && busca.Sen_Usuario == senhaHash)
            {
                if(busca.Status_Usuario == StatusHelpDesk.naoConfirmado)
                {
                    throw new Exception("Confirme seu email");
                }
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
            usuario.Token_Redefinicao_Senha = tokenRedefinicaoSenha;
            usuario.Dta_Token = dataHoraGeracaoTokenString;
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
                            <p>Token para confirmar email {tokenRedefinicaoSenha} </p>
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

            if (TokenRedefinicaoSenhaExpirado(usuario.Dta_Token))
            {
                return new BadRequestObjectResult("Token de redefinição de senha expirado. Solicite um novo token.");
            }
            if(novaSenha == usuario.Sen_Usuario) {
                return new BadRequestObjectResult("Você não pode utilizar uma senha que ja utilizou anteriormente");
            }
            usuario.Sen_Usuario = AesOperation.CriarHash(novaSenha);
            usuario.Token_Redefinicao_Senha = null;

            await _contexto.SaveChangesAsync();
            return new OkResult();
        }

        public async Task<IActionResult> RedefinirEmail(string email, int id)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("O email não pode ser vazio", nameof(email));
            }

            var usuario = await BuscarUsuarioPorId(id);
            if (usuario == null)
            {
                throw new Exception($"Usuário com ID {id} não encontrado");
            }

            string paternInvalidoEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, paternInvalidoEmail))
            {
                throw new Exception("Endereço de e-mail inválido.");
            }

            var busca = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Eml_Usuario == email);
            if(busca != null)
            {
                throw new Exception("Endereço de e-mail já cadastrado.");
            }

            if (email.Contains("@employer.com.br") || email.Contains("@bne-empregos.com.br"))
            {
                string tokenRedefinicaoSenha = Guid.NewGuid().ToString();

                DateTime dataHoraGeracaoToken = DateTime.Now;
                string dataHoraGeracaoTokenString = dataHoraGeracaoToken.ToString("yyyy-MM-dd HH:mm:ss.fff");
                usuario.Token_Redefinicao_Senha = tokenRedefinicaoSenha;
                usuario.Dta_Token = dataHoraGeracaoTokenString;
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
                                    <h1>Confirmação de email - HD-Support</h1>
                                </div>
                            </header>
                            <div class='container'>
                                <p>Olá,</p>
                                <p>Seu link de confirmação de email é: <a class='link' href='localhost:3000/ConfirmarEmail?token={usuario.Token_Redefinicao_Senha}&email={email}'>Clique aqui</a>.</p>
                                <p><b>Token de confirmação:</b> {usuario.Token_Redefinicao_Senha}</p>
                                <p>Não compartilhe seu link de redefinição, ele expirará dentro de 15 minutos.</p>
                                <p>Atenciosamente,<br>Equipe HD-Support</p>
                            </div>
                        </body>
                        </html>";

                try
                {
                    await _SendEmailRepository.SendEmailAsync(email, "Confirmação de email HD-Support", texto);
                    return new OkResult();
                }
                catch (Exception ex)
                {
                    // Se falhar ao enviar o email, removemos o token gerado
                    usuario.Token_Redefinicao_Senha = null;
                    usuario.Dta_Token = null;
                    await _contexto.SaveChangesAsync();
                    throw new Exception($"Erro ao enviar email de confirmação: {ex.Message}");
                }
            }

            return new BadRequestObjectResult("O Email precisa ser da Employer");
        }

        public async Task<IActionResult> ConfirmarEmail(string token, string email)
        {
            var usuario = await BuscarUsuarioPorToken(token);

            if (usuario == null)
            {
                return new BadRequestObjectResult("Token inválido ou expirado.");
            }

            if (TokenRedefinicaoSenhaExpirado(usuario.Dta_Token))
            {
                return new BadRequestObjectResult("Token de confirmação de email expirado. Solicite um novo token.");
            }

            usuario.Token_Redefinicao_Senha = null;

            if(usuario.Eml_Usuario != email && email != null)
            {
                usuario.Eml_Usuario = email;
            }

            usuario.Status_Usuario = StatusHelpDesk.Disponivel;
            _contexto.Usuarios.Update(usuario);


            await _contexto.SaveChangesAsync();
            return new OkResult();
        }

        private async Task<Usuarios> BuscarUsuarioPorToken(string token)
        {
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Token_Redefinicao_Senha == token);
            return usuario;
        }

        private async Task<bool> UsuarioPossuiTokenAtivo(int idUsuario)
        {
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Id == idUsuario);
            if (usuario == null)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(usuario.Token_Redefinicao_Senha) &&
                !TokenRedefinicaoSenhaExpirado(usuario.Dta_Token))
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

        public async Task<Usuarios> BuscarPorTokenJWT(string token)
        {
            var email = TokenService.ReadJWT(token);
            email.Replace(" ", "");
            var busca = await _contexto.Usuarios.FirstOrDefaultAsync(x => x.Eml_Usuario == email);
            return busca;
        }
    }
}
