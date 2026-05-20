using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF pour Employee — table T_Employees, schéma dbo.
/// Inclut la self-FK ReportToEmployeeId pour la chaîne d'approbation.
/// </summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("T_Employees", "dbo");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("Id");

        builder.Property(e => e.Matricule).HasMaxLength(50);
        builder.Property(e => e.NumeroEmploye).HasMaxLength(50);
        builder.Property(e => e.NomComplet).IsRequired().HasMaxLength(200);
        builder.Property(e => e.DisplayName).IsRequired().HasMaxLength(200).HasDefaultValue(string.Empty);
        builder.Property(e => e.Prenom).HasMaxLength(100);
        builder.Property(e => e.Nom).HasMaxLength(100);
        builder.Property(e => e.Fonction).HasMaxLength(200);
        builder.Property(e => e.Email).HasMaxLength(200);
        builder.Property(e => e.Telephone).HasMaxLength(50);
        builder.Property(e => e.DepartementNom).HasMaxLength(200);
        builder.Property(e => e.Sources).HasMaxLength(100);

        // Index uniques (nullable filtré pour autoriser plusieurs NULL)
        builder.HasIndex(e => e.Matricule)
            .IsUnique()
            .HasFilter("[Matricule] IS NOT NULL");

        builder.HasIndex(e => e.NumeroEmploye)
            .IsUnique()
            .HasFilter("[NumeroEmploye] IS NOT NULL");

        builder.HasIndex(e => e.ReportToEmployeeId);

        // Self-FK ReportTo
        builder.HasOne(e => e.ReportTo)
            .WithMany(e => e.Subordinates)
            .HasForeignKey(e => e.ReportToEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Compagnie)
            .WithMany(c => c.Employees)
            .HasForeignKey(e => e.CompagnieId);
    }
}
