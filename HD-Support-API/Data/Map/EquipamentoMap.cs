using HD_Support_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HD_Support_API.Data.Map
{
    public class EquipamentoMap : IEntityTypeConfiguration<Equipamentos>
    {
        public void Configure(EntityTypeBuilder<Equipamentos> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Idf_Patrimonio).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Modelo_Equipamento).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Dtl_Equipamento);
            builder.Property(x => x.Tpo_Equipamento);
            builder.Property(x => x.Dta_Emprestimo_Inicio);
            builder.Property(x => x.Dta_Emprestimo_Final);
            builder.Property(x => x.Stt_Equipamento);
            builder.Property(x => x.Profissional_Hd);
        }
    }
}
