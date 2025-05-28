using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("emprestimos")]
    public class Emprestimos
    {
        public int id { get; set; }

        // Relacionamento com funcionarios
        public Usuarios? usuario { get; set; }
        public int usuarioid { get; set; }

        // Relacionamento com equipamentos
        public Equipamentos? equipamentos { get; set; }
        public int equipamentosid { get; set; }
    }
}
