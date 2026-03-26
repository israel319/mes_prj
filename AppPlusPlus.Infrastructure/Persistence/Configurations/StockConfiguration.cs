using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppPlusPlus.Domain.Entities.Stock;

namespace AppPlusPlus.Infrastructure.Persistence.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(s => s.Id);

        // Stock : Index unique (Article, Localisation)
        builder.HasIndex(s => new { s.IdArticle, s.IdLocalisation })
            .IsUnique();
    }
}
