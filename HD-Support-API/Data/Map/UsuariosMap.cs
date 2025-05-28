using HD_Support_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HD_Support_API.Data.Map
{
    public class UsuariosMap : IEntityTypeConfiguration<Usuarios>
    {
        public void Configure(EntityTypeBuilder<Usuarios> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.nome).IsRequired().HasMaxLength(255);
            builder.Property(x => x.email).IsRequired().HasMaxLength(255);
            builder.Property(x => x.senha).IsRequired().HasMaxLength(255);
            builder.Property(x => x.telefone).IsRequired().HasMaxLength(255);
            builder.Property(x => x.cargo).IsRequired().HasMaxLength(255);
            builder.Property(x => x.tokenredefinicaosenha).HasMaxLength(255);
            builder.Property(x => x.datahorageracaotoken).HasMaxLength(255);
        }
    }
}
