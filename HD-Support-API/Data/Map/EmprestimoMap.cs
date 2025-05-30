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

            builder.HasOne(x => x.usuario)
                   .WithMany()
                   .HasForeignKey(x => x.Idf_Usuario)
                   .IsRequired();

            builder.HasOne(x => x.equipamentos)
                   .WithMany()
                   .HasForeignKey(x => x.Idf_Equipamentos)
                   .IsRequired();
        }
    }
}
