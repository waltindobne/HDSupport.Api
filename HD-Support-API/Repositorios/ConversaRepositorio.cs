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
            conversa.Criptografia_Conversa = AesOperation.gerarChave(32);
            conversa.Criptografia_Conversa = AesOperation.Encriptar(cripto, conversa.Criptografia_Conversa);
            Usuarios cliente = await _contexto.Usuarios.FindAsync(conversa.Idf_Cliente);
            conversa.cliente = cliente;
            /*HelpDesk funcionario = await _contexto.HelpDesk.FindAsync(conversa.funcionariosid);
            //atualizando status do funcionário
            if (conversa.tipoconversa != tipoconversa.Conversa) {
                funcionario.status = StatusHelpDesk.Ocupado;
                _contexto.HelpDesk.Update(funcionario);
            }
            conversa.Funcionario = funcionario;*/
            conversa.Tipo_Conversa = (TipoConversa)conversa.Tipo_Conversa;
            if (conversa.Stt_Conversa != null)
            {
                conversa.Stt_Conversa = (StatusConversa)conversa.Stt_Conversa;
            }
            else
            {
                conversa.Stt_Conversa = StatusConversa.NaoAceito;
            }
            if (conversa.Idf_Funcionario == null || conversa.Idf_Funcionario == conversa.Idf_Cliente)
            {
                conversa.funcionarios = cliente;
            }
            else
            {
                Usuarios funcionario = await _contexto.Usuarios.FindAsync(conversa.Idf_Funcionario);
                conversa.funcionarios = funcionario;
            }
            conversa.Dta_Inicio_Conversa = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            await _contexto.Conversa.AddAsync(conversa);
            await _contexto.SaveChangesAsync();
            return conversa;
        }
        public async Task<Conversa> EnviarMensagem(int idConversa, Mensagens mensagem)
        {
            Conversa conversa = await BuscarConversaPorId(idConversa);
            string criptografia = AesOperation.Descriptar(cripto, conversa.Criptografia_Conversa);
            mensagem.Msg_Mensagem = AesOperation.Encriptar(criptografia, mensagem.Msg_Mensagem);
            mensagem.Idf_Conversa = idConversa;
            mensagem.usuario = await _contexto.Usuarios.FindAsync(mensagem.Idf_Usuario);
            mensagem.Dta_Envio = DateTime.UtcNow;
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
            conversaPorId.Stt_Conversa = StatusConversa.Encerrado;
            if(conversaPorId.Tipo_Conversa != TipoConversa.Conversa)
            {
                Usuarios funcionario = conversaPorId.funcionarios;
                funcionario.Status_Usuario = StatusHelpDesk.Disponivel;
                _contexto.Usuarios.Update(funcionario);
            }
            conversaPorId.Dta_Conclusao_Conversa = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            conversaPorId.Dta_Inicio_Conversa = DateTime.SpecifyKind(conversaPorId.Dta_Inicio_Conversa, DateTimeKind.Utc);

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
            List<Mensagens> MensagensLista = await _contexto.Mensagens.Where(x => x.Idf_Conversa == id).ToListAsync();
            string criptografia = AesOperation.Descriptar(cripto, conversaPorId.Criptografia_Conversa);
            for (int i = 0; i < MensagensLista.Count;i++)
            {
                MensagensLista[i].Msg_Mensagem = AesOperation.Descriptar(criptografia, MensagensLista[i].Msg_Mensagem);
                MensagensLista[i].usuario = await _contexto.Usuarios.FindAsync(MensagensLista[i].Idf_Usuario);
                MensagensLista[i].Dta_Envio = MensagensLista[i].Dta_Envio.AddHours(-3);
            }
            return MensagensLista;
        }

        public async Task<bool> VerificarMensagemNova(int idConversa, int qtdMensagensAtual)
        {
            int qtdMensagens = _contexto.Mensagens.Where(x => x.Idf_Conversa == idConversa).Count();
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
            conversa.Idf_Funcionario = idFuncionario;
            if(conversa.Tipo_Conversa == TipoConversa.HelpDesk)
            {
                funcionario.Status_Usuario = StatusHelpDesk.Ocupado;
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
                ConversaLista = await _contexto.Conversa.Where(x => x.Tipo_Conversa == tipoConversa && x.Stt_Conversa == StatusConversa.NaoAceito).ToListAsync();
            }
            else
            {
                ConversaLista = await _contexto.Conversa.Where(x => x.Tipo_Conversa == tipoConversa && x.Stt_Conversa != StatusConversa.NaoAceito).ToListAsync();
            }

            for(var i = 0; i < ConversaLista.Count(); i++)
            {
                ConversaLista[i].cliente = await _contexto.Usuarios.FindAsync(ConversaLista[i].Idf_Cliente);
                ConversaLista[i].funcionarios = await _contexto.Usuarios.FindAsync(ConversaLista[i].Idf_Funcionario);
            }
            
            return ConversaLista;
        }
        public async Task<List<Conversa>> ListarTodosChamados()
        {
            var ConversaLista = await _contexto.Conversa.ToListAsync();
            for (var i = 0; i < ConversaLista.Count(); i++)
            {
                ConversaLista[i].cliente = await _contexto.Usuarios.FindAsync(ConversaLista[i].Idf_Cliente);
                if (ConversaLista[i].Idf_Funcionario != null)
                    ConversaLista[i].funcionarios = await _contexto.Usuarios.FindAsync(ConversaLista[i].Idf_Funcionario);
            }
            return ConversaLista;
        }

        public async Task<List<Conversa>> ListarConversas(int idUsuario)
        {
            List<Conversa> ConversaLista = await _contexto.Conversa.Where(x => x.Idf_Funcionario == idUsuario || x.Idf_Cliente == idUsuario).ToListAsync();

            for (var i = 0; i < ConversaLista.Count(); i++)
            {
                ConversaLista[i].cliente = await _contexto.Usuarios.FindAsync(ConversaLista[i].Idf_Cliente);
                if (ConversaLista[i].Idf_Funcionario != null)
                    ConversaLista[i].funcionarios = await _contexto.Usuarios.FindAsync(ConversaLista[i].Idf_Funcionario);
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
            conversa.Stt_Conversa = StatusCorrigido;
            
            if (StatusCorrigido == StatusConversa.Encerrado)
            {
                conversa.Dta_Conclusao_Conversa = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                conversa.Dta_Inicio_Conversa = DateTime.SpecifyKind(conversa.Dta_Inicio_Conversa, DateTimeKind.Utc);
            }
            
            if (StatusCorrigido == StatusConversa.EmAndamento && conversa.Idf_Funcionario != null)
            {
                Usuarios funcionario = await _contexto.Usuarios.FindAsync(conversa.Idf_Funcionario);
                funcionario.Status_Usuario = StatusHelpDesk.Disponivel;
                _contexto.Usuarios.Update(funcionario);
            }
            _contexto.Conversa.Update(conversa);
            await _contexto.SaveChangesAsync();

            return true;
        }

        public async Task<List<int>> DadosChamadosDashboard()
        {
            var aberto = _contexto.Conversa.Where(x => x.Stt_Conversa == Enums.StatusConversa.NaoAceito && x.Tipo_Conversa == TipoConversa.HelpDesk).Count();
            var pendente = _contexto.Conversa.Where(x => x.Stt_Conversa == Enums.StatusConversa.EmAndamento && x.Tipo_Conversa == TipoConversa.HelpDesk).Count();
            var concluido = _contexto.Conversa.Where(x => x.Stt_Conversa == Enums.StatusConversa.Encerrado && x.Tipo_Conversa == TipoConversa.HelpDesk).Count();
            List<int> dados = [aberto, pendente, concluido];
            return dados;
        }
    }
}
