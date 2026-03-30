using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Checkpoint (table ref.Checkpoints)
/// </summary>
public class CheckpointConfiguration : IEntityTypeConfiguration<Checkpoint>
{
    public void Configure(EntityTypeBuilder<Checkpoint> builder)
    {
        builder.ToTable("T_Checkpoints", "dbo");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.SiteId)
            .IsRequired();

        builder.Property(c => c.Nom)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Code)
            .HasMaxLength(20);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.HasOne(c => c.Site)
            .WithMany()
            .HasForeignKey(c => c.SiteId);
    }
}
