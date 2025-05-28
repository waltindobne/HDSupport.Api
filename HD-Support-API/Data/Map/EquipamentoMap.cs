using HD_Support_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HD_Support_API.Data.Map
{
    public class EquipamentoMap : IEntityTypeConfiguration<Equipamentos>
    {
        public void Configure(EntityTypeBuilder<Equipamentos> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.idpatrimonio).IsRequired().HasMaxLength(255);
            builder.Property(x => x.modelo).IsRequired().HasMaxLength(255);
            builder.Property(x => x.detalhes);
            builder.Property(x => x.tipo);
            builder.Property(x => x.dtemeprestimoinicio);
            builder.Property(x => x.dtemeprestimofinal);
            builder.Property(x => x.statusequipamento);
            builder.Property(x => x.profissional_hd);
        }
    }
}
