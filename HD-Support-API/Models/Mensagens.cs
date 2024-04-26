namespace HD_Support_API.Models
{
    public class Mensagens
    {
        public int Id { get; set; }
        public string Mensagem {  get; set; }
        public int ConversaId { get; set; }

        // Relacionamento com Funcionarios
        public Usuarios Usuario { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Data_envio { get; set; }

    }
}
