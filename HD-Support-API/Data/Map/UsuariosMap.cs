using HD_Support_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HD_Support_API.Data.Map
{
    public class UsuariosMap : IEntityTypeConfiguration<Usuarios>
    {
        public void Configure(EntityTypeBuilder<Usuarios> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Nme_Usuario).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Eml_Usuario).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Sen_Usuario).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Tel_Usuario).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Cargo_Usuario).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Token_Redefinicao_Senha).HasMaxLength(255);
            builder.Property(x => x.Dta_Token).HasMaxLength(255);
        }
    }
}
