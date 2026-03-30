using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité AuditLog
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("T_AuditLogs", "dbo");

        builder.HasKey(a => a.IdAuditLog);

        builder.Property(a => a.UtilisateurLogin)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.UtilisateurNom)
            .HasMaxLength(200);

        builder.Property(a => a.TypeAction)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Categorie)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.EntiteId)
            .HasMaxLength(100);

        builder.Property(a => a.EntiteType)
            .HasMaxLength(100);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.AdresseIP)
            .HasMaxLength(50);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.Resultat)
            .HasMaxLength(20);

        builder.Property(a => a.MessageErreur)
            .HasMaxLength(2000);

        builder.Property(a => a.Niveau)
            .HasMaxLength(20);

        builder.Property(a => a.CorrelationId)
            .HasMaxLength(100);

        // Index pour les recherches fréquentes
        builder.HasIndex(a => a.DateAction)
            .HasDatabaseName("IX_AuditLogs_DateAction");

        builder.HasIndex(a => a.UtilisateurLogin)
            .HasDatabaseName("IX_AuditLogs_UtilisateurLogin");

        builder.HasIndex(a => a.TypeAction)
            .HasDatabaseName("IX_AuditLogs_TypeAction");

        builder.HasIndex(a => a.Categorie)
            .HasDatabaseName("IX_AuditLogs_Categorie");

        builder.HasIndex(a => new { a.EntiteType, a.EntiteId })
            .HasDatabaseName("IX_AuditLogs_Entite");

        builder.HasIndex(a => a.Niveau)
            .HasDatabaseName("IX_AuditLogs_Niveau");

        builder.HasIndex(a => a.CorrelationId)
            .HasDatabaseName("IX_AuditLogs_CorrelationId");

        // Index composite pour les requêtes de rapport
        builder.HasIndex(a => new { a.DateAction, a.Categorie, a.TypeAction })
            .HasDatabaseName("IX_AuditLogs_Reporting");
    }
}
