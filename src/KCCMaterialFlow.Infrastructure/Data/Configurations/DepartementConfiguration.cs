using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Departement (table shared.Departements)
/// </summary>
public class DepartementConfiguration : IEntityTypeConfiguration<Departement>
{
    public void Configure(EntityTypeBuilder<Departement> builder)
    {
        builder.ToTable("T_Departements", "dbo");

        builder.HasKey(d => d.IdDepartement);

        builder.Property(d => d.IdDepartement)
            .HasColumnName("IdDepartement");

        builder.Property(d => d.CodeDepartement)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("CodeDepartement");

        builder.Property(d => d.NomDepartement)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("NomDepartement");

        builder.Property(d => d.Description)
            .HasMaxLength(500)
            .HasColumnName("Description");

        builder.Property(d => d.ResponsableLogin)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("ResponsableLogin");

        builder.Property(d => d.ResponsableNom)
            .HasMaxLength(200)
            .HasColumnName("ResponsableNom");

        builder.Property(d => d.ResponsableEmail)
            .HasMaxLength(200)
            .HasColumnName("ResponsableEmail");

        builder.Property(d => d.EstActif)
            .HasColumnName("EstActif");

        builder.Property(d => d.DateCreation)
            .HasColumnName("DateCreation");

        builder.Property(d => d.DateModification)
            .HasColumnName("DateModification");

        // Propriétés calculées non-mappées (commodité pour Radzen DropDown)
        builder.Ignore(d => d.Id);
        builder.Ignore(d => d.Nom);

        builder.HasMany(d => d.Employees)
            .WithOne(e => e.Departement)
            .HasForeignKey(e => e.DepartementId);
    }
}
