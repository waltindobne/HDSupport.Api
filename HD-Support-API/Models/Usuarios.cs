using HD_Support_API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("usuarios")]
    public class Usuarios
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string email { get; set; }

        [Column(TypeName = "nvarchar(MAX)")] 
        public string senha { get; set; }
        public string telefone { get; set; }
        public string cargo { get; set; }
        public string? imagem { get; set; }
        public StatusHelpDesk? status { get; set; }
        public StatusHelpDeskConversa? statusconversa { get; set; }
        public string? tokenredefinicaosenha { get; set; }
        public string? datahorageracaotoken { get; set; }
    }
}
