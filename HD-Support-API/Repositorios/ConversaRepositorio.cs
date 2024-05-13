using EncryptionDecryptionUsingSymmetricKey;
using HD_Support_API.Components;
using HD_Support_API.Enums;
using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace HD_Support_API.Repositorios
{
    public class ConversaRepositorio : IConversaRepositorio
    {
        private readonly BancoContext _contexto = new BancoContext();
        private readonly string cripto = "criptografiahdsuportcriptografia";

        /*usando sql "normal"
        public ConversaRepositorio(BancoContext contexto)
        {
            _contexto = contexto;
        }*/
        public async Task<Conversa> IniciarConversa(Conversa conversa)
        {
            conversa.Criptografia = AesOperation.gerarChave(32);
            conversa.Criptografia = AesOperation.Encriptar(cripto, conversa.Criptografia);
            Usuarios cliente = await _contexto.Usuarios.FindAsync(conversa.ClienteId);
            conversa.Cliente = cliente;
            /*HelpDesk funcionario = await _contexto.HelpDesk.FindAsync(conversa.FuncionariosId);
            //atualizando status do funcionário
            if (conversa.TipoConversa != TipoConversa.Conversa) {
                funcionario.Status = StatusHelpDesk.Ocupado;
                _contexto.HelpDesk.Update(funcionario);
            }
            conversa.Funcionario = funcionario;*/
            conversa.TipoConversa = (TipoConversa)conversa.TipoConversa;
            if (conversa.Status != null)
            {
                conversa.Status = (StatusConversa)conversa.Status;
            }
            else
            {
                conversa.Status = StatusConversa.NaoAceito;
            }
            if (conversa.FuncionariosId == null || conversa.FuncionariosId == conversa.ClienteId)
            {
                Usuarios funcionario = new Usuarios { Id=-1,Nome="base",Email="base",Senha="base"};
                conversa.Funcionarios = funcionario;
            }
            else
            {
                Usuarios funcionario = await _contexto.Usuarios.FindAsync(conversa.FuncionariosId);
                conversa.Funcionarios = funcionario;
            }
            conversa.Data_inicio = DateTime.Now.AddHours(-3);
            await _contexto.Conversa.AddAsync(conversa);
            await _contexto.SaveChangesAsync();
            return conversa;
        }
        public async Task<Conversa> EnviarMensagem(int idConversa, Mensagens mensagem)
        {
            Conversa conversa = await BuscarConversaPorId(idConversa);
            string criptografia = AesOperation.Descriptar(cripto, conversa.Criptografia);
            mensagem.Mensagem = AesOperation.Encriptar(criptografia, mensagem.Mensagem);
            mensagem.ConversaId = idConversa;
            mensagem.Usuario = await _contexto.Usuarios.FindAsync(mensagem.UsuarioId);
            mensagem.Data_envio = DateTime.Now.AddHours(-3);
            await _contexto.Mensagens.AddAsync(mensagem);
            await _contexto.SaveChangesAsync();
            return conversa;
        }

        public async Task<Conversa> BuscarConversaPorId(int Id)
        {
            return await _contexto.Conversa.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<bool> TerminarConversa(int id)
        {
            Conversa conversaPorId = await BuscarConversaPorId(id);

            if (conversaPorId == null)
            {
                throw new Exception($"Conversa de Id:{id} não encontrado na base de dados.");
            }
            conversaPorId.Status = StatusConversa.Encerrado;
            if(conversaPorId.TipoConversa != TipoConversa.Conversa)
            {
                Usuarios funcionario = conversaPorId.Funcionarios;
                funcionario.Status = StatusHelpDesk.Disponivel;
                _contexto.Usuarios.Update(funcionario);
            }
            conversaPorId.Data_conclusao = DateTime.Now.AddHours(-3);

            _contexto.Conversa.Update(conversaPorId);
            await _contexto.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ExcluirMensagem(int id)
        {
            Mensagens mensagem = await _contexto.Mensagens.FindAsync(id);

            if (mensagem == null)
            {
                throw new Exception($"Mensagem de Id:{id} não encontrado na base de dados.");
            }
            _contexto.Mensagens.Remove(mensagem);
            await _contexto.SaveChangesAsync();

            return true;
        }

        public async Task<List<Mensagens>> ListarMensagens(int id)
        {
            Conversa conversaPorId = await BuscarConversaPorId(id);
            List<Mensagens> MensagensLista = await _contexto.Mensagens.Where(x => x.ConversaId == id).ToListAsync();
            string criptografia = AesOperation.Descriptar(cripto, conversaPorId.Criptografia);
            for (int i = 0; i < MensagensLista.Count;i++)
            {
                MensagensLista[i].Mensagem = AesOperation.Descriptar(criptografia, MensagensLista[i].Mensagem);
            }
            return MensagensLista;
        }

        public async Task<bool> VerificarMensagemNova(int idConversa, int qtdMensagensAtual)
        {
            int qtdMensagens = _contexto.Mensagens.Where(x => x.ConversaId == idConversa).Count();
            if(qtdMensagens > qtdMensagensAtual)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> AceitarChamado(int idConversa, int idFuncionario)
        {
            Usuarios funcionario = await _contexto.Usuarios.FindAsync(idFuncionario);
            if (funcionario == null)
            {
                throw new Exception($"Funcionário de Id:{idFuncionario} não encontrado na base de dados.");
            }
            
            Conversa conversa = await BuscarConversaPorId(idConversa);
            if (conversa == null)
            {
                throw new Exception($"Conversa de Id:{idConversa} não encontrado na base de dados.");
            }

            conversa.Funcionarios = funcionario;
            conversa.FuncionariosId = idFuncionario;
            if(conversa.TipoConversa == TipoConversa.HelpDesk)
            {
                funcionario.Status = StatusHelpDesk.Ocupado;
            }

            _contexto.Usuarios.Update(funcionario);
            _contexto.Conversa.Update(conversa);
            await _contexto.SaveChangesAsync();
            return true;
        }

        public async Task<List<Conversa>> ListarChamados(int tipo, bool aceito = false)
        {
            TipoConversa tipoConversa = (TipoConversa)tipo;
            List<Conversa> ConversaLista;
            if (!aceito)
            {
                ConversaLista = await _contexto.Conversa.Where(x => x.TipoConversa == tipoConversa && x.Status == StatusConversa.NaoAceito).ToListAsync();
            }
            else
            {
                ConversaLista = await _contexto.Conversa.Where(x => x.TipoConversa == tipoConversa && x.Status != StatusConversa.NaoAceito).ToListAsync();
            }
            
            return ConversaLista;
        }

        public async Task<List<Conversa>> ListarConversas(int idUsuario)
        {
            List<Conversa> ConversaLista = await _contexto.Conversa.Where(x => x.FuncionariosId == idUsuario || x.ClienteId == idUsuario).ToListAsync();

            return ConversaLista;
        }

        public async Task<bool> AtualizarStatusConversa(int idConversa, int status)
        {
            Conversa conversa = await BuscarConversaPorId(idConversa);
            if (conversa == null)
            {
                throw new Exception($"Conversa de Id:{idConversa} não encontrado na base de dados.");
            }
            StatusConversa StatusCorrigido = (StatusConversa)status;
            conversa.Status = StatusCorrigido;
            if (StatusCorrigido == StatusConversa.EmAndamento && conversa.FuncionariosId != null)
            {
                Usuarios funcionario = await _contexto.Usuarios.FindAsync(conversa.FuncionariosId);
                funcionario.Status = StatusHelpDesk.Disponivel;
                _contexto.Usuarios.Update(funcionario);
            }
            _contexto.Conversa.Update(conversa);
            await _contexto.SaveChangesAsync();

            return true;
        }

        public async Task<List<int>> DadosChamadosDashboard()
        {
            var aberto = _contexto.Conversa.Where(x => x.Status == Enums.StatusConversa.NaoAceito && x.TipoConversa == TipoConversa.HelpDesk).Count();
            var pendente = _contexto.Conversa.Where(x => x.Status == Enums.StatusConversa.EmAndamento && x.TipoConversa == TipoConversa.HelpDesk).Count();
            var concluido = _contexto.Conversa.Where(x => x.Status == Enums.StatusConversa.Encerrado && x.TipoConversa == TipoConversa.HelpDesk).Count();
            List<int> dados = [aberto, pendente, concluido];
            return dados;
        }
    }
}
