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
        private readonly BancoContext _contexto;
        private readonly string cripto = "criptografiahdsuportcriptografia";

        public ConversaRepositorio(BancoContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<Conversa> IniciarConversa(Conversa conversa)
        {
            conversa.criptografia = AesOperation.gerarChave(32);
            conversa.criptografia = AesOperation.Encriptar(cripto, conversa.criptografia);
            Usuarios cliente = await _contexto.Usuarios.FindAsync(conversa.clienteid);
            conversa.cliente = cliente;
            /*HelpDesk funcionario = await _contexto.HelpDesk.FindAsync(conversa.funcionariosid);
            //atualizando status do funcionário
            if (conversa.tipoconversa != tipoconversa.Conversa) {
                funcionario.status = StatusHelpDesk.Ocupado;
                _contexto.HelpDesk.Update(funcionario);
            }
            conversa.Funcionario = funcionario;*/
            conversa.tipoconversa = (TipoConversa)conversa.tipoconversa;
            if (conversa.status != null)
            {
                conversa.status = (StatusConversa)conversa.status;
            }
            else
            {
                conversa.status = StatusConversa.NaoAceito;
            }
            if (conversa.funcionariosid == null || conversa.funcionariosid == conversa.clienteid)
            {
                conversa.funcionarios = cliente;
            }
            else
            {
                Usuarios funcionario = await _contexto.Usuarios.FindAsync(conversa.funcionariosid);
                conversa.funcionarios = funcionario;
            }
            conversa.data_inicio = DateTime.UtcNow;
            await _contexto.Conversa.AddAsync(conversa);
            await _contexto.SaveChangesAsync();
            return conversa;
        }
        public async Task<Conversa> EnviarMensagem(int idConversa, Mensagens mensagem)
        {
            Conversa conversa = await BuscarConversaPorId(idConversa);
            string criptografia = AesOperation.Descriptar(cripto, conversa.criptografia);
            mensagem.mensagem = AesOperation.Encriptar(criptografia, mensagem.mensagem);
            mensagem.conversaid = idConversa;
            mensagem.usuario = await _contexto.Usuarios.FindAsync(mensagem.usuarioid);
            mensagem.data_envio = DateTime.UtcNow;
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
                throw new Exception($"Conversa de id:{id} não encontrado na base de dados.");
            }
            conversaPorId.status = StatusConversa.Encerrado;
            if(conversaPorId.tipoconversa != TipoConversa.Conversa)
            {
                Usuarios funcionario = conversaPorId.funcionarios;
                funcionario.status = StatusHelpDesk.Disponivel;
                _contexto.Usuarios.Update(funcionario);
            }
            conversaPorId.data_conclusao = DateTime.UtcNow;

            _contexto.Conversa.Update(conversaPorId);
            await _contexto.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ExcluirMensagem(int id)
        {
            Mensagens mensagem = await _contexto.Mensagens.FindAsync(id);

            if (mensagem == null)
            {
                throw new Exception($"mensagem de id:{id} não encontrado na base de dados.");
            }
            _contexto.Mensagens.Remove(mensagem);
            await _contexto.SaveChangesAsync();

            return true;
        }

        public async Task<List<Mensagens>> ListarMensagens(int id)
        {
            Conversa conversaPorId = await BuscarConversaPorId(id);
            List<Mensagens> MensagensLista = await _contexto.Mensagens.Where(x => x.conversaid == id).ToListAsync();
            string criptografia = AesOperation.Descriptar(cripto, conversaPorId.criptografia);
            for (int i = 0; i < MensagensLista.Count;i++)
            {
                MensagensLista[i].mensagem = AesOperation.Descriptar(criptografia, MensagensLista[i].mensagem);
                MensagensLista[i].usuario = await _contexto.Usuarios.FindAsync(MensagensLista[i].usuarioid);
                MensagensLista[i].data_envio = MensagensLista[i].data_envio.AddHours(-3);
            }
            return MensagensLista;
        }

        public async Task<bool> VerificarMensagemNova(int idConversa, int qtdMensagensAtual)
        {
            int qtdMensagens = _contexto.Mensagens.Where(x => x.conversaid == idConversa).Count();
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
                throw new Exception($"Funcionário de id:{idFuncionario} não encontrado na base de dados.");
            }
            
            Conversa conversa = await BuscarConversaPorId(idConversa);
            if (conversa == null)
            {
                throw new Exception($"Conversa de id:{idConversa} não encontrado na base de dados.");
            }

            conversa.funcionarios = funcionario;
            conversa.funcionariosid = idFuncionario;
            if(conversa.tipoconversa == TipoConversa.HelpDesk)
            {
                funcionario.status = StatusHelpDesk.Ocupado;
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
                ConversaLista = await _contexto.Conversa.Where(x => x.tipoconversa == tipoConversa && x.status == StatusConversa.NaoAceito).ToListAsync();
            }
            else
            {
                ConversaLista = await _contexto.Conversa.Where(x => x.tipoconversa == tipoConversa && x.status != StatusConversa.NaoAceito).ToListAsync();
            }

            for(var i = 0; i < ConversaLista.Count(); i++)
            {
                ConversaLista[i].cliente = await _contexto.Usuarios.FindAsync(ConversaLista[i].clienteid);
                ConversaLista[i].funcionarios = await _contexto.Usuarios.FindAsync(ConversaLista[i].funcionariosid);
            }
            
            return ConversaLista;
        }

        public async Task<List<Conversa>> ListarConversas(int idUsuario)
        {
            List<Conversa> ConversaLista = await _contexto.Conversa.Where(x => x.funcionariosid == idUsuario || x.clienteid == idUsuario).ToListAsync();

            for (var i = 0; i < ConversaLista.Count(); i++)
            {
                ConversaLista[i].cliente = await _contexto.Usuarios.FindAsync(ConversaLista[i].clienteid);
                if (ConversaLista[i].funcionariosid != null)
                    ConversaLista[i].funcionarios = await _contexto.Usuarios.FindAsync(ConversaLista[i].funcionariosid);
            }

            return ConversaLista;
        }

        public async Task<bool> AtualizarStatusConversa(int idConversa, int status)
        {
            Conversa conversa = await BuscarConversaPorId(idConversa);
            if (conversa == null)
            {
                throw new Exception($"Conversa de id:{idConversa} não encontrado na base de dados.");
            }
            StatusConversa StatusCorrigido = (StatusConversa)status;
            conversa.status = StatusCorrigido;
            if (StatusCorrigido == StatusConversa.EmAndamento && conversa.funcionariosid != null)
            {
                Usuarios funcionario = await _contexto.Usuarios.FindAsync(conversa.funcionariosid);
                funcionario.status = StatusHelpDesk.Disponivel;
                _contexto.Usuarios.Update(funcionario);
            }
            _contexto.Conversa.Update(conversa);
            await _contexto.SaveChangesAsync();

            return true;
        }

        public async Task<List<int>> DadosChamadosDashboard()
        {
            var aberto = _contexto.Conversa.Where(x => x.status == Enums.StatusConversa.NaoAceito && x.tipoconversa == TipoConversa.HelpDesk).Count();
            var pendente = _contexto.Conversa.Where(x => x.status == Enums.StatusConversa.EmAndamento && x.tipoconversa == TipoConversa.HelpDesk).Count();
            var concluido = _contexto.Conversa.Where(x => x.status == Enums.StatusConversa.Encerrado && x.tipoconversa == TipoConversa.HelpDesk).Count();
            List<int> dados = [aberto, pendente, concluido];
            return dados;
        }
    }
}
