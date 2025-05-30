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
            var verificacao = _contexto.Equipamento.FirstOrDefault(x => x.Idf_Patrimonio == equipamento.Idf_Patrimonio);
            if(verificacao == null)
            {
                Console.WriteLine(DateTime.UtcNow);
                equipamento.Dta_Emprestimo_Inicio = DateTime.UtcNow;
                await _contexto.Equipamento.AddAsync(equipamento);
                _contexto.SaveChanges();
                return equipamento;
            }
            throw new Exception($"O patrimônio com id:{equipamento.Idf_Patrimonio} já está cadastrado.");
        }

        public async Task<Equipamentos> AtualizarEquipamento(Equipamentos equipamento, int id)
        {
            Equipamentos equipamentosPorId = await BuscarEquipamentosPorId(id);
            var verificacao = _contexto.Equipamento.FirstOrDefault(x => x.Idf_Patrimonio == equipamento.Idf_Patrimonio);
            if (equipamentosPorId == null)
            {
                throw new Exception($"Equipamento de id:{id} não encontrado na base de dados.");
            }
            if(verificacao == null || verificacao.Idf_Patrimonio == equipamentosPorId.Idf_Patrimonio)
            {
                equipamentosPorId.Idf_Patrimonio = equipamento.Idf_Patrimonio;
                equipamentosPorId.Modelo_Equipamento = equipamento.Modelo_Equipamento;
                equipamentosPorId.Dtl_Equipamento = equipamento.Dtl_Equipamento;
                equipamentosPorId.Stt_Equipamento = equipamento.Stt_Equipamento;
                equipamentosPorId.Dta_Emprestimo_Inicio = equipamentosPorId.Dta_Emprestimo_Inicio.ToUniversalTime();

                Console.WriteLine(equipamentosPorId.Dta_Emprestimo_Inicio.ToUniversalTime());

                _contexto.Equipamento.Update(equipamentosPorId);
                await _contexto.SaveChangesAsync();

                return equipamentosPorId;
            }
            throw new Exception($"Equipamento de id:{id} já cadastrado na base de dados.");

        }

        public async Task<Equipamentos> BuscarEquipamentos(string idPatrimonio)
        {
            return  _contexto.Equipamento.FirstOrDefault(x => x.Idf_Patrimonio == idPatrimonio);
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
                throw new Exception($"Equipamento de id:{id} não encontrado na base de dados.");
            }

            var idPatrimonio = Convert.ToInt16(busca.Idf_Patrimonio);

            Emprestimos emprestimo = _contexto.Emprestimo.FirstOrDefault(x => x.Idf_Equipamentos == idPatrimonio);
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
            var disponivel = _contexto.Equipamento.Where(x => x.Stt_Equipamento == Enums.StatusEquipamento.Disponivel).Count();
            var ocupado = _contexto.Equipamento.Where(x => x.Stt_Equipamento == Enums.StatusEquipamento.Emprestado).Count();
            var emreparo = _contexto.Equipamento.Where(x => x.Stt_Equipamento == Enums.StatusEquipamento.EmConserto).Count();
            var emconserto = _contexto.Equipamento.Where(x => x.Stt_Equipamento == Enums.StatusEquipamento.Danificado).Count();
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
                    var resultado = _contexto.Equipamento.Where(x => x.Tpo_Equipamento == tipo && x.Stt_Equipamento == (StatusEquipamento)j).Count();
                    equipamento.Add(resultado);
                }
                dados.Add(equipamento);
            }

            return dados;
        }
    }
}
