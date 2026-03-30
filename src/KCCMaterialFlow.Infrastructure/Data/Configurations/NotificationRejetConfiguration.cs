using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité NotificationRejet (table dbo.NotificationsRejet)
/// </summary>
public class NotificationRejetConfiguration : IEntityTypeConfiguration<NotificationRejet>
{
    public void Configure(EntityTypeBuilder<NotificationRejet> builder)
    {
        builder.ToTable("T_NotificationsRejet", "dbo");

        builder.HasKey(n => n.Id);

        // Mapping temporaire : la colonne BD est encore "Id" (sera renommée par Phase1_PK_Rename.sql)
        builder.Property(n => n.Id).HasColumnName("Id");

        builder.Property(n => n.BonType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(n => n.NumeroReference)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(n => n.EtapeRejet)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(n => n.ApprobateurNom)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.ApprobateurLogin)
            .HasMaxLength(100);

        builder.Property(n => n.MotifRejet)
            .IsRequired();

        builder.Property(n => n.DemandeurNom)
            .HasMaxLength(200);
    }
}
