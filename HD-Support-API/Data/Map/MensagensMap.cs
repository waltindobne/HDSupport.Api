using HD_Support_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HD_Support_API.Data.Map
{
    public class MensagensMap : IEntityTypeConfiguration<Mensagens>
    {
        public void Configure(EntityTypeBuilder<Mensagens> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.mensagem).IsRequired().HasMaxLength(255);
            builder.Property(x => x.conversaid).IsRequired();
            builder.HasOne(x => x.usuario)
               .WithMany()
               .HasForeignKey(x => x.usuarioid)
               .IsRequired();
            builder.Property(x => x.data_envio).IsRequired();
        }
    }
}
