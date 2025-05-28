using HD_Support_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HD_Support_API.Data.Map
{
    public class ConversaMap : IEntityTypeConfiguration<Conversa>
    {
        public void Configure(EntityTypeBuilder<Conversa> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.funcionarios)
                   .WithMany()
                   .HasForeignKey(x => x.funcionariosid)
                   .IsRequired();

            builder.HasOne(x => x.cliente)
                   .WithMany()
                   .HasForeignKey(x => x.clienteid)
                   .IsRequired();
            builder.Property(x => x.criptografia).IsRequired();
            builder.Property(x => x.status).IsRequired();
            builder.Property(x => x.data_inicio).IsRequired();
            builder.Property(x => x.data_conclusao);
        }
    }
}
