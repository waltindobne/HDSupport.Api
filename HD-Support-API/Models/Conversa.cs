using HD_Support_API.Enums;

namespace HD_Support_API.Models
{
    public class Conversa
    {
		public int Id {  get; set; }

        // Relacionamento com Funcionarios empresa
        public Usuarios? Funcionarios { get; set; }
        public int? FuncionariosId { get; set; }

        // Relacionamento com clientes
        public Usuarios Cliente { get; set; }
        public int ClienteId { get; set; }
        public TipoConversa? TipoConversa { get; set; }
        public string Criptografia { get; set; }
        public StatusConversa Status {  get; set; }
        public DateTime Data_inicio {  get; set; }
        public DateTime? Data_conclusao { get; set; }
    }
}
