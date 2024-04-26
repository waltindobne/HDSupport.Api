using HD_Support_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HD_Support_API.Data.Map
{
    public class EmprestimoMap : IEntityTypeConfiguration<Emprestimos>
    {
        public void Configure(EntityTypeBuilder<Emprestimos> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Usuario)
                   .WithMany()
                   .HasForeignKey(x => x.UsuarioId)
                   .IsRequired();

            builder.HasOne(x => x.Equipamentos)
                   .WithMany()
                   .HasForeignKey(x => x.EquipamentosId)
                   .IsRequired();
        }
    }
}
