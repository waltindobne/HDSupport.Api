using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("emprestimos")]
    public class Emprestimos
    {
        public int Id { get; set; }

        // Relacionamento com funcionarios
        public Usuarios? usuario { get; set; }
        public int Idf_Usuario { get; set; }

        // Relacionamento com equipamentos
        public Equipamentos? equipamentos { get; set; }
        public int Idf_Equipamentos { get; set; }
    }
}
