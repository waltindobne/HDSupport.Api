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
        private readonly BancoContext _contexto;

        public EquipamentoRepositorio(BancoContext contexto)
        {
            _contexto = contexto;
        }

        /*usando sql "normal"
        public EquipamentoRepositorio(BancoContext contexto)
        {
            _contexto = contexto;
        }*/
        public async Task<Equipamentos> AdicionarEquipamento(Equipamentos equipamento)
        {
            var verificacao = _contexto.Equipamento.FirstOrDefault(x => x.idpatrimonio == equipamento.idpatrimonio);
            if(verificacao == null)
            {
                Console.WriteLine(DateTime.UtcNow);
                equipamento.dtemeprestimoinicio = DateTime.UtcNow;
                await _contexto.Equipamento.AddAsync(equipamento);
                _contexto.SaveChanges();
                return equipamento;
            }
            throw new Exception($"O patrimônio com id:{equipamento.idpatrimonio} já está cadastrado.");
        }

        public async Task<Equipamentos> AtualizarEquipamento(Equipamentos equipamento, int id)
        {
            Equipamentos equipamentosPorId = await BuscarEquipamentosPorId(id);
            var verificacao = _contexto.Equipamento.FirstOrDefault(x => x.idpatrimonio == equipamento.idpatrimonio);
            if (equipamentosPorId == null)
            {
                throw new Exception($"Equipamento de id:{id} não encontrado na base de dados.");
            }
            if(verificacao == null || verificacao.idpatrimonio == equipamentosPorId.idpatrimonio)
            {
                equipamentosPorId.idpatrimonio = equipamento.idpatrimonio;
                equipamentosPorId.modelo = equipamento.modelo;
                equipamentosPorId.detalhes = equipamento.detalhes;
                equipamentosPorId.statusequipamento = equipamento.statusequipamento;
                equipamentosPorId.dtemeprestimoinicio = equipamentosPorId.dtemeprestimoinicio.ToUniversalTime();

                Console.WriteLine(equipamentosPorId.dtemeprestimoinicio.ToUniversalTime());

                _contexto.Equipamento.Update(equipamentosPorId);
                await _contexto.SaveChangesAsync();

                return equipamentosPorId;
            }
            throw new Exception($"Equipamento de id:{id} já cadastrado na base de dados.");

        }

        public async Task<Equipamentos> BuscarEquipamentos(string idPatrimonio)
        {
            return  _contexto.Equipamento.FirstOrDefault(x => x.idpatrimonio == idPatrimonio);
        }        
        public async Task<Equipamentos> BuscarEquipamentosPorId(int id)
        {
            var busca =  _contexto.Equipamento.FirstOrDefault(x => x.id == id);
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
                throw new Exception($"Equipamento de id:{id} não encontrado na base de dados.");
            }

            var idPatrimonio = Convert.ToInt16(busca.idpatrimonio);

            Emprestimos emprestimo = _contexto.Emprestimo.FirstOrDefault(x => x.equipamentosid == idPatrimonio);
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
            var disponivel = _contexto.Equipamento.Where(x => x.statusequipamento == Enums.StatusEquipamento.Disponivel).Count();
            var ocupado = _contexto.Equipamento.Where(x => x.statusequipamento == Enums.StatusEquipamento.Emprestado).Count();
            var emreparo = _contexto.Equipamento.Where(x => x.statusequipamento == Enums.StatusEquipamento.EmConserto).Count();
            var emconserto = _contexto.Equipamento.Where(x => x.statusequipamento == Enums.StatusEquipamento.Danificado).Count();
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
                    var resultado = _contexto.Equipamento.Where(x => x.tipo == tipo && x.statusequipamento == (StatusEquipamento)j).Count();
                    equipamento.Add(resultado);
                }
                dados.Add(equipamento);
            }

            return dados;
        }
    }
}
