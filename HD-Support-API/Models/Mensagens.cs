using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("TAB_Mensagens")]
    public class Mensagens
    {
        public int Id { get; set; }
        public string Msg_Mensagem {  get; set; }
        public int Idf_Conversa { get; set; }

        // Relacionamento com funcionarios
        public Usuarios usuario { get; set; }
        public int Idf_Usuario { get; set; }
        public DateTime Dta_Envio { get; set; }

    }
}
