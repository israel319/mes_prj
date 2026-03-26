using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppPlusPlus.Domain.Entities.Approvisionnement;

namespace AppPlusPlus.Infrastructure.Persistence.Configurations;

public class ApproConfiguration : IEntityTypeConfiguration<Appro>
{
    public void Configure(EntityTypeBuilder<Appro> builder)
    {
        // Appro -> ApproDetails (1:N)
        builder.HasMany(a => a.Details)
            .WithOne(d => d.Appro)
            .HasForeignKey(d => d.IdAppro);
    }
}
