using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppPlusPlus.Domain.Entities.Vente;

namespace AppPlusPlus.Infrastructure.Persistence.Configurations;

public class FactConfiguration : IEntityTypeConfiguration<Fact>
{
    public void Configure(EntityTypeBuilder<Fact> builder)
    {
        // Relations Fact -> FactDetails
        builder.HasMany(f => f.Details)
            .WithOne(d => d.Fact)
            .HasForeignKey(d => d.IdFact);

        // Relations Fact -> Payments
        builder.HasMany(f => f.Payments)
            .WithOne(p => p.Fact)
            .HasForeignKey(p => p.IdFact);
    }
}
