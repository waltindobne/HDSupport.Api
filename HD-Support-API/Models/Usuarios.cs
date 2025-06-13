using HD_Support_API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("TAB_Usuarios")]
    public class Usuarios
    {
        public int Id { get; set; }
        public string Nme_Usuario { get; set; }
        public string Eml_Usuario { get; set; }

        [Column(TypeName = "nvarchar(MAX)")] 
        public string Sen_Usuario { get; set; }
        public string Tel_Usuario { get; set; }
        public string Cargo_Usuario { get; set; }
        public string? Img_Usuario { get; set; }
        public StatusHelpDesk? Status_Usuario { get; set; }
        public StatusHelpDeskConversa? Status_Conversa { get; set; }
        public string? Token_Redefinicao_Senha { get; set; }
        public string? Dta_Token { get; set; }
        public int Idf_Squad { get; set; }
    }
}
