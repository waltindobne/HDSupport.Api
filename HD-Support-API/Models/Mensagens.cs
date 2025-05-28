using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("mensagens")]
    public class Mensagens
    {
        public int id { get; set; }
        public string mensagem {  get; set; }
        public int conversaid { get; set; }

        // Relacionamento com funcionarios
        public Usuarios usuario { get; set; }
        public int usuarioid { get; set; }
        public DateTime data_envio { get; set; }

    }
}
