﻿using HD_Support_API.Models;

namespace HD_Support_API.Repositorios.Interfaces
{
    public interface IConversaRepositorio
    {
        Task<List<Mensagens>> ListarMensagens(int idConversa);
        Task<Conversa> EnviarMensagem(int idConversa, Mensagens Mensagem);
        Task<Conversa> IniciarConversa(Conversa conversa);
        Task<bool> TerminarConversa(int id);
        Task<bool> ExcluirMensagem(int id);
        Task<Conversa> BuscarConversaPorId(int id);
        Task<bool> AtualizarStatusConversa(int idConversa, int status);
        Task<bool> VerificarMensagemNova(int idConversa, int qtMensagensAtual);
        Task<bool> AceitarChamado(int idConversa, int idFuncionario);
        Task<List<Conversa>> ListarConversas(int idUsuario);
        Task<List<Conversa>> ListarAllChamados();
        Task<List<Conversa>> ListarChamados(int tipo, bool aceito = false);
        Task<List<Conversa>> ListarTodosChamados();
        Task<List<int>> DadosChamadosDashboard();
    }
}
