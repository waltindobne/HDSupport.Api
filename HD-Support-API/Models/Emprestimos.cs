namespace HD_Support_API.Models
{
    public class Emprestimos
    {
        public int Id { get; set; }

        // Relacionamento com Funcionarios
        public Usuarios? Usuario { get; set; }
        public int UsuarioId { get; set; }

        // Relacionamento com Equipamentos
        public Equipamentos? Equipamentos { get; set; }
        public int EquipamentosId { get; set; }
    }
}
