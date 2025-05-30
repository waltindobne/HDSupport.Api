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
                   .HasForeignKey(x => x.Idf_Funcionario)
                   .IsRequired();

            builder.HasOne(x => x.cliente)
                   .WithMany()
                   .HasForeignKey(x => x.Idf_Cliente)
                   .IsRequired();
            builder.Property(x => x.Criptografia_Conversa).IsRequired();
            builder.Property(x => x.Stt_Conversa).IsRequired();
            builder.Property(x => x.Dta_Inicio_Conversa).IsRequired();
            builder.Property(x => x.Dta_Conclusao_Conversa);
        }
    }
}
