using HD_Support_API.Components;
using HD_Support_API.Enums;
using HD_Support_API.Models;
using HD_Support_API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
                equipamentosPorId.Detalhes = equipamento.Detalhes;

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
            var equipamentos = await _contexto.Equipamento.ToListAsync();
            return equipamentos;
        }

        public async Task<List<int>> DadosEquipamentoPizza()
        {
            var disponivel = _contexto.Equipamento.Where(x => x.StatusEquipamento == Enums.StatusEquipamento.Disponivel).Count();
            var ocupado = _contexto.Equipamento.Where(x => x.StatusEquipamento == Enums.StatusEquipamento.Emprestado).Count();
            var emreparo = _contexto.Equipamento.Where(x => x.StatusEquipamento == Enums.StatusEquipamento.EmConserto).Count();
            var emconserto = _contexto.Equipamento.Where(x => x.StatusEquipamento == Enums.StatusEquipamento.Danificado).Count();
            List<int> dados = [disponivel, ocupado, emreparo, emconserto];
            return dados;
        }

        public async Task<List<List<int>>> DadosEquipamentoBarras()
        {
            List<String> tipos = ["Desktop", "Notebook", "Monitor", "Headset", "Webcam", "Teclado", "Mouse"];
            List<List<int>> dados = [];
            for (int i = 0; i < tipos.Count(); i++)
            {
                List<int> equipamento = [];
                var tipo = tipos[i];
                for(int j = 1; j < 5; j++)
                {
                    var resultado = _contexto.Equipamento.Where(x => x.Tipo == tipo && x.StatusEquipamento == (StatusEquipamento)j).Count();
                    equipamento.Add(resultado);
                }
                dados.Add(equipamento);
            }

            return dados;
        }
    }
}
