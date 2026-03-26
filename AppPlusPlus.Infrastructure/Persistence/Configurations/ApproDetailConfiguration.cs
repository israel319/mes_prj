using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppPlusPlus.Domain.Entities.Approvisionnement;

namespace AppPlusPlus.Infrastructure.Persistence.Configurations;

public class ApproDetailConfiguration : IEntityTypeConfiguration<ApproDetail>
{
    public void Configure(EntityTypeBuilder<ApproDetail> builder)
    {
        builder.HasKey(a => a.Id);
    }
}
