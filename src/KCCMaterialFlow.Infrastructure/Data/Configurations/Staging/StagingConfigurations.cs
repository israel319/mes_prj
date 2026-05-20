using KCCMaterialFlow.Domain.Entities.Staging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations.Staging;

public class StagingCompanyConfiguration : IEntityTypeConfiguration<StagingCompany>
{
    public void Configure(EntityTypeBuilder<StagingCompany> builder)
    {
        builder.ToTable("Staging_Company", "stg");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.ImportBatchId);
        builder.HasIndex(x => x.CompanyCode);
    }
}

public class StagingContractConfiguration : IEntityTypeConfiguration<StagingContract>
{
    public void Configure(EntityTypeBuilder<StagingContract> builder)
    {
        builder.ToTable("Staging_Contract", "stg");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.ImportBatchId);
        builder.HasIndex(x => x.PoNumber);
        builder.HasIndex(x => x.CompanyCode);
    }
}
