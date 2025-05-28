using HD_Support_API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("equipamentos")]
    public class Equipamentos
    {
        public int id { get; set; }
        public string? idpatrimonio { get; set; }
        public string? modelo { get; set; }
        public string? tipo {  get; set; }
        public string? detalhes { get; set; }
        public DateTime dtemeprestimoinicio { get; set; }
        public DateTime dtemeprestimofinal { get; set; }
        public StatusEquipamento statusequipamento { get; set; }
        public string? profissional_hd { get; set; }
    }
}
