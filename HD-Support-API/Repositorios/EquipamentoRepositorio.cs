using HD_Support_API.Components;
using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HD_Support_API.Repositorios
{
    public class EquipamentoRepositorio : IEquipamentosRepositorio
    {
        private readonly BancoContext _contexto = new BancoContext();

        /*usando sql "normal"
        public EquipamentoRepositorio(BancoContext contexto)
        {
            _contexto = contexto;
        }*/
        public async Task<Equipamentos> AdicionarEquipamento(Equipamentos equipamento)
        {
            var verificacao = _contexto.Equipamento.FirstOrDefault(x => x.IdPatrimonio == equipamento.IdPatrimonio);
            if(verificacao == null)
            {
                Console.WriteLine(DateTime.UtcNow);
                equipamento.DtEmeprestimoInicio = DateTime.UtcNow;
                await _contexto.Equipamento.AddAsync(equipamento);
                _contexto.SaveChanges();
                return equipamento;
            }
            throw new Exception($"O patrimônio com Id:{equipamento.IdPatrimonio} já está cadastrado.");
        }

        public async Task<Equipamentos> AtualizarEquipamento(Equipamentos equipamento, int id)
        {
            Equipamentos equipamentosPorId = await BuscarEquipamentosPorId(id);
            var verificacao = _contexto.Equipamento.FirstOrDefault(x => x.IdPatrimonio == equipamento.IdPatrimonio);
            if (equipamentosPorId == null)
            {
                throw new Exception($"Equipamento de Id:{id} não encontrado na base de dados.");
            }
            if(verificacao == null)
            {
                equipamentosPorId.IdPatrimonio = equipamento.IdPatrimonio;
                equipamentosPorId.Modelo = equipamento.Modelo;
                equipamentosPorId.Processador = equipamento.Processador;
                equipamentosPorId.HeadSet = equipamento.HeadSet;

                _contexto.Equipamento.Update(equipamentosPorId);
                await _contexto.SaveChangesAsync();

                return equipamentosPorId;
            }
            throw new Exception($"Equipamento de Id:{id} já cadastrado na base de dados.");

        }

        public async Task<Equipamentos> BuscarEquipamentos(int idPatrimonio)
        {
            return  _contexto.Equipamento.FirstOrDefault(x => x.IdPatrimonio == idPatrimonio);
        }        
        public async Task<Equipamentos> BuscarEquipamentosPorId(int id)
        {
            var busca =  _contexto.Equipamento.FirstOrDefault(x => x.Id == id);
            if(busca == null)
            {
                throw new Exception($"Equipamento com o ID {id} não encontrado");
            }
            return busca;
        }

        public async Task<bool> ExcluirEquipamento(int id)
        {
            var busca = await BuscarEquipamentosPorId(id);

            if (busca == null)
            {
                throw new Exception($"Equipamento de Id:{id} não encontrado na base de dados.");
            }

            Emprestimos emprestimo = _contexto.Emprestimo.FirstOrDefault(x => x.EquipamentosId == busca.IdPatrimonio);
            if (emprestimo != null)
                _contexto.Remove(emprestimo);

            _contexto.Remove(busca);
            await _contexto.SaveChangesAsync();

            return true;

        }

        public async Task<List<Equipamentos>> ListarEquipamentos()
        {
            return await _contexto.Equipamento.ToListAsync();
        }

        public async Task<List<int>> DadosEquipamentoPizza()
        {
            var disponivel = _contexto.Equipamento.Where(x => x.statusEquipamento == Enums.StatusEquipamento.Disponivel).Count();
            var ocupado = _contexto.Equipamento.Where(x => x.statusEquipamento == Enums.StatusEquipamento.Emprestado).Count();
            var emreparo = _contexto.Equipamento.Where(x => x.statusEquipamento == Enums.StatusEquipamento.EmConserto).Count();
            var emconserto = _contexto.Equipamento.Where(x => x.statusEquipamento == Enums.StatusEquipamento.Danificado).Count();
            List<int> dados = [disponivel, ocupado, emreparo, emconserto];
            return dados;
        }

        public async Task<List<int>> DadosEquipamentoBarras()
        {
            var notebook = _contexto.Equipamento.Where(x => x.Tipo == "Desktop").Count();
            var desktop = _contexto.Equipamento.Where(x => x.Tipo == "Notebook").Count();
            var monitor = _contexto.Equipamento.Where(x => x.Tipo == "Monitor").Count();
            var headset = _contexto.Equipamento.Where(x => x.Tipo == "Headset").Count();
            var webcam = _contexto.Equipamento.Where(x => x.Tipo == "Webcam").Count();
            var teclado = _contexto.Equipamento.Where(x => x.Tipo == "Teclado").Count();
            var mouse = _contexto.Equipamento.Where(x => x.Tipo == "Mouse").Count();
            List<int> dados = [notebook, desktop, monitor, headset, webcam, teclado, mouse];
            return dados;
        }
    }
}
