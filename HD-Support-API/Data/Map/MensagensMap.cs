using HD_Support_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HD_Support_API.Data.Map
{
    public class MensagensMap : IEntityTypeConfiguration<Mensagens>
    {
        public void Configure(EntityTypeBuilder<Mensagens> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Msg_Mensagem).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Idf_Conversa).IsRequired();
            builder.HasOne(x => x.usuario)
               .WithMany()
               .HasForeignKey(x => x.Idf_Usuario)
               .IsRequired();
            builder.Property(x => x.Dta_Envio).IsRequired();
        }
    }
}
