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
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Senha).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Telefone).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Cargo).IsRequired().HasMaxLength(255);
            builder.Property(x => x.TokenRedefinicaoSenha).HasMaxLength(255);
            builder.Property(x => x.DataHoraGeracaoToken).HasMaxLength(255);
        }
    }
}
