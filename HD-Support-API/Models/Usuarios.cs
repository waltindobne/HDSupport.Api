using HD_Support_API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(MAX)")] 
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public string Cargo { get; set; }
        public StatusHelpDesk? Status { get; set; }
        public StatusHelpDeskConversa? StatusConversa { get; set; }
        public string? TokenRedefinicaoSenha { get; set; }
        public string? DataHoraGeracaoToken { get; set; }
    }
}
