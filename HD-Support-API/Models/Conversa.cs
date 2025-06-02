using HD_Support_API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("TAB_Conversas")]
    public class Conversa
    {
		public int Id {  get; set; }

        // Relacionamento com funcionarios empresa
        public Usuarios? funcionarios { get; set; }
        public int? Idf_Funcionario { get; set; }

        public Usuarios cliente { get; set; }
        public int Idf_Cliente { get; set; }
        public TipoConversa? Tipo_Conversa { get; set; }
        public string Criptografia_Conversa { get; set; }
        public StatusConversa Stt_Conversa {  get; set; }
        public DateTime Dta_Inicio_Conversa {  get; set; }
        public DateTime? Dta_Conclusao_Conversa { get; set; }
    }
}
