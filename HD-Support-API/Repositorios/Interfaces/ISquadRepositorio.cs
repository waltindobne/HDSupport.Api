using HD_Support_API.Models;

namespace HD_Support_API.Repositorios.Interfaces
{
    public interface ISquadRepositorio
    {
        Task<List<Squad>> ListarSquads();
        Task<(Squad squad, List<Usuarios> usuarios)> BuscarSquadPorId(int id);
        Task<List<Usuarios>> BuscarUsuarios(int id_squad);
        Task<Squad> AdicionarSquad(Squad squad);
        Task<Squad> AtualizarSquad(Squad squad, int id);
        Task<bool> ExcluirSquad(int id);
    }
}
