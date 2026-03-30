using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Compagnie (table ref.Compagnies)
/// </summary>
public class CompagnieConfiguration : IEntityTypeConfiguration<Compagnie>
{
    public void Configure(EntityTypeBuilder<Compagnie> builder)
    {
        builder.ToTable("T_Compagnies", "dbo");

        builder.HasKey(c => c.Id);

        // Mapping temporaire : la colonne BD est encore "Id" (sera renommée par Phase1_PK_Rename.sql)
        builder.Property(c => c.Id).HasColumnName("Id");

        builder.Property(c => c.Nom)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Code)
            .HasMaxLength(20);

        builder.Property(c => c.Email)
            .HasMaxLength(200);

        builder.Property(c => c.Telephone)
            .HasMaxLength(50);

        builder.Property(c => c.SiteManager)
            .HasMaxLength(200);

        builder.HasMany(c => c.Contrats)
            .WithOne(ct => ct.Compagnie)
            .HasForeignKey(ct => ct.CompagnieId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Employees)
            .WithOne(e => e.Compagnie)
            .HasForeignKey(e => e.CompagnieId);
    }
}
