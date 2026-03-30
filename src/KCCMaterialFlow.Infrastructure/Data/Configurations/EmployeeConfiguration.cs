using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Employee (table ref.Employees)
/// </summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("T_Employees", "dbo");

        builder.HasKey(e => e.Id);

        // Mapping temporaire : la colonne BD est encore "Id" (sera renommée par Phase1_PK_Rename.sql)
        builder.Property(e => e.Id).HasColumnName("Id");

        builder.Property(e => e.Matricule)
            .HasMaxLength(20);

        builder.Property(e => e.NomComplet)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Prenom)
            .HasMaxLength(100);

        builder.Property(e => e.Nom)
            .HasMaxLength(100);

        builder.Property(e => e.Fonction)
            .HasMaxLength(150);

        builder.Property(e => e.Email)
            .HasMaxLength(200);

        builder.Property(e => e.Telephone)
            .HasMaxLength(50);

        builder.Property(e => e.Login)
            .HasMaxLength(100);

        builder.HasOne(e => e.Departement)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartementId);

        builder.HasOne(e => e.Compagnie)
            .WithMany(c => c.Employees)
            .HasForeignKey(e => e.CompagnieId);
    }
}
