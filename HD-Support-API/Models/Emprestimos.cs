using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("TAB_Emprestimos")]
    public class Emprestimos
    {
        public int Id { get; set; }

        [ForeignKey("equipamentos")]
        public int Idf_Equipamento { get; set; }
        public Equipamentos? equipamentos { get; set; }

        [ForeignKey("squad")]
        public int Idf_Squad { get; set; }
        public Squad? squad { get; set; }

        [ForeignKey("usuario")]
        public int Idf_Usuario { get; set; }
        public Usuarios? usuario { get; set; }
    }
}
