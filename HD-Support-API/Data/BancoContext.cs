using HD_Support_API.Data.Map;
using HD_Support_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HD_Support_API.Components

{
    public class BancoContext:DbContext
    {
        //public BancoContext(DbContextOptions<BancoContext> options) : base(options){ }
        public DbSet<Equipamentos> Equipamento { get; set; }
        public DbSet<Emprestimos> Emprestimo { get; set; }
        public DbSet<Conversa> Conversa { get; set; }
        public DbSet<Mensagens> Mensagens { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }


        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmprestimoMap());
            modelBuilder.ApplyConfiguration(new UsuariosMap());
            modelBuilder.ApplyConfiguration(new EquipamentoMap());
            modelBuilder.ApplyConfiguration(new ConversaMap());
            modelBuilder.ApplyConfiguration(new MensagensMap());
            base.OnModelCreating(modelBuilder);
        }*/

        //postgresql conexão
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(
                "");
    }
}
