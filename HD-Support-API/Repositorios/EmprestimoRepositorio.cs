using HD_Support_API.Components;
using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HD_Support_API.Repositorios
{
    public class EmprestimoRepositorio : IEmprestimoRepositorio
    {
        private readonly BancoContext _contexto;

        public EmprestimoRepositorio(BancoContext contexto)
        {
            _contexto = contexto;
        }

        /*usando sql "normal"
        public EmprestimoRepositorio(BancoContext contexto)
        {
            _contexto = contexto;
        }*/
        public async Task<Emprestimos> AdicionarEmprestimo(string idPatrimonio, string email)
        {
            var verificarEquipamento = _contexto.Emprestimo.FirstOrDefault(x => x.equipamentos.Idf_Patrimonio == idPatrimonio);
            var verificarFuncionario = _contexto.Emprestimo.FirstOrDefault(x => x.usuario.Eml_Usuario == email && x.equipamentos.Tpo_Equipamento == "Desktop" || x.usuario.Eml_Usuario == email && x.equipamentos.Tpo_Equipamento == "Notebook");
            if(verificarEquipamento == null && verificarFuncionario == null)
            {
                Emprestimos emprestimo = new Emprestimos();
                emprestimo.equipamentos = _contexto.Equipamento.FirstOrDefault(x => x.Idf_Patrimonio == idPatrimonio);
                emprestimo.usuario = _contexto.Usuarios.FirstOrDefault(x => x.Eml_Usuario == email);
                if(emprestimo.usuario== null)
                {
                    throw new Exception("Nenhum usuário encontrado com esse email.");
                }
                emprestimo.Idf_Equipamentos = emprestimo.equipamentos.Id;
                emprestimo.Idf_Usuario = emprestimo.usuario.Id;
                emprestimo.equipamentos.Dta_Emprestimo_Inicio = DateTime.UtcNow;

                await _contexto.Emprestimo.AddAsync(emprestimo);
                await _contexto.SaveChangesAsync();
                return emprestimo;
            }
            throw new Exception("Equipamento em emprestimo ou funcionario já possui um emprestimo.");
           
            
        }

        public async Task<Emprestimos> AtualizarEmprestimo(Emprestimos emprestimo, int id)
        {
            var busca = await BuscarEmprestimosPorID(id);
            var verificarEquipamento = _contexto.Emprestimo.FirstOrDefault(x => x.equipamentos.Idf_Patrimonio == emprestimo.equipamentos.Idf_Patrimonio);
            var verificarFuncionario = _contexto.Emprestimo.FirstOrDefault(x => x.usuario.Eml_Usuario == emprestimo.usuario.Eml_Usuario);
            if (busca == null)
            {
                throw new Exception($"Equipamento de id:{emprestimo.Id} não encontrado na base de dados.");
            }
            if(verificarEquipamento == null && verificarFuncionario == null)
            {
                var equipamento = await _contexto.Equipamento.FirstOrDefaultAsync(x => x.Idf_Patrimonio == Convert.ToString(emprestimo.Idf_Equipamentos));
                busca.Idf_Equipamentos = equipamento.Id;
                busca.Idf_Usuario = emprestimo.Idf_Usuario;

                _contexto.Emprestimo.Update(busca);
                await _contexto.SaveChangesAsync();

                return busca;
            }
            if(verificarEquipamento != null || verificarFuncionario != null)
            {
                if (verificarEquipamento != null && verificarFuncionario == null)
                {
                    throw new Exception($"Não foi possivel atualizar o emprestimo, pois o equipamento com patrimônio:{emprestimo.equipamentos.Idf_Patrimonio} possui um emprestimo em andamento");
                }
                if (verificarFuncionario != null && verificarEquipamento == null)
                {
                    throw new Exception($"Não foi possivel atualizar o emprestimo, pois o funcionario de E-mail:{emprestimo.usuario.Eml_Usuario} possui um emprestimo cadastrado.");
                }
                
            }
            throw new Exception($"Não foi possivel atualizar o emprestimo");
        }

        public async Task<Emprestimos> BuscarEmprestimos(string idPatrimonio, string email)
        {
            var emprestimo = _contexto.Emprestimo.FirstOrDefault(
                x => x.equipamentos.Idf_Patrimonio == idPatrimonio ||
                x.usuario.Eml_Usuario == email);
            return emprestimo;
        }

        public async Task<Emprestimos> BuscarEmprestimosPorID(int id)
        {
            return _contexto.Emprestimo.FirstOrDefault(x => x.Id == id);
        }

        public async Task<bool> ExcluirEmprestimo(int id)
        {
            var buscar = await BuscarEmprestimosPorID(id);

            if (buscar == null)
            {
                throw new Exception($"Emprestimo de id:{id} não encontrado na base de dados.");
            }
            _contexto.Remove(buscar);
            await _contexto.SaveChangesAsync();

            return true;
        }

        public async Task<List<Emprestimos>> ListarEmprestimos()
        {
            var lista =  await _contexto.Emprestimo.ToListAsync();
            if(lista == null)
            {
                throw new Exception("Ainda não temos nenhum emprestimo registrado");
            }
            for(var i = 0; i < lista.Count;i++)
            {
                var emp = lista[i];
                var equipamento = await _contexto.Equipamento.FindAsync(emp.Idf_Equipamentos);
                var usuario = await _contexto.Usuarios.FindAsync(emp.Idf_Usuario);
                lista[i].equipamentos = equipamento;
                lista[i].usuario = usuario;
            }

            return await _contexto.Emprestimo.ToListAsync();
        }
    }
}
