using HD_Support_API.Models;

namespace HD_Support_API.Repositorios.Interfaces
{
    public interface IEmprestimoRepositorio
    {
        Task<List<Emprestimos>> ListarEmprestimos();
        Task<Emprestimos> BuscarEmprestimos(string idPatrimonio, string nome);
        Task<Emprestimos> BuscarEmprestimosPorID(int id);
        Task<Emprestimos> AdicionarEmprestimo(string idPatrimonio, string email);
        Task<Emprestimos> AtualizarEmprestimo(Emprestimos emprestimo, int id);
        Task<bool> ExcluirEmprestimo(int id);
    }
}
