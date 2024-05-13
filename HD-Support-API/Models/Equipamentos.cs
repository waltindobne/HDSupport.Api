using HD_Support_API.Enums;

namespace HD_Support_API.Models
{
    public class Equipamentos
    {
        public int Id { get; set; }
        public int? IdPatrimonio { get; set; }
        public string? Modelo { get; set; }
        public string? Tipo {  get; set; }
        public string? Detalhes { get; set; }
        public DateTime DtEmeprestimoInicio { get; set; }
        public DateTime DtEmeprestimoFinal { get; set; }
        public StatusEquipamento statusEquipamento { get; set; }
        public string? profissional_HD { get; set; }
    }
}
