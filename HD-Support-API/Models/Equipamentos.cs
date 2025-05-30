using HD_Support_API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("equipamentos")]
    public class Equipamentos
    {
        public int Id { get; set; }
        public string? Idf_Patrimonio { get; set; }
        public string? Modelo_Equipamento { get; set; }
        public string? Tpo_Equipamento {  get; set; }
        public string? Dtl_Equipamento { get; set; }
        public DateTime Dta_Emprestimo_Inicio { get; set; }
        public DateTime Dta_Emprestimo_Final { get; set; }
        public StatusEquipamento Stt_Equipamento { get; set; }
        public string? Profissional_Hd { get; set; }
    }
}
