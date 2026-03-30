using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Site (table ref.Sites)
/// </summary>
public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("T_Sites", "dbo");

        builder.HasKey(s => s.Id);

        // Mapping temporaire : la colonne BD est encore "Id" (sera renommée par Phase1_PK_Rename.sql)
        builder.Property(s => s.Id).HasColumnName("Id");

        builder.Property(s => s.Nom)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Code)
            .HasMaxLength(20);

        builder.Property(s => s.Adresse)
            .HasMaxLength(500);

        builder.Property(s => s.TypeSite)
            .HasMaxLength(50);
    }
}
