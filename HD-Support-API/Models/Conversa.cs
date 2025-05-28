using HD_Support_API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("conversa")]
    public class Conversa
    {
		public int Id {  get; set; }

        // Relacionamento com funcionarios empresa
        public Usuarios? funcionarios { get; set; }
        public int? funcionariosid { get; set; }

        // Relacionamento com clientes
        public Usuarios cliente { get; set; }
        public int clienteid { get; set; }
        public TipoConversa? tipoconversa { get; set; }
        public string criptografia { get; set; }
        public StatusConversa status {  get; set; }
        public DateTime data_inicio {  get; set; }
        public DateTime? data_conclusao { get; set; }
    }
}
