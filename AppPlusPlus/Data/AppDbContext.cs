using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Models;

namespace AppPlusPlus.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── Tables de référence ──
    public DbSet<Localisation> Localisations => Set<Localisation>();
    public DbSet<ArticleType> ArticleTypes => Set<ArticleType>();
    public DbSet<ArticleMarque> ArticleMarques => Set<ArticleMarque>();
    public DbSet<ArticleCategory> ArticleCategories => Set<ArticleCategory>();
    public DbSet<Mesure> Mesures => Set<Mesure>();
    public DbSet<Money> Moneys => Set<Money>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<StatusFactDetail> StatusFactDetails => Set<StatusFactDetail>();
    public DbSet<StatusCmd> StatusCmds => Set<StatusCmd>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Fonction> Fonctions => Set<Fonction>();
    public DbSet<Reduction> Reductions => Set<Reduction>();
    public DbSet<ExpenseSource> ExpenseSources => Set<ExpenseSource>();
    public DbSet<SupplierService> SupplierServices => Set<SupplierService>();

    // ── Entités métier ──
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerType> CustomerTypes => Set<CustomerType>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserFonction> UserFonctions => Set<UserFonction>();
    public DbSet<UserLocalisation> UserLocalisations => Set<UserLocalisation>();
    public DbSet<UserCaisse> UserCaisses => Set<UserCaisse>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<UserActivity> UserActivities => Set<UserActivity>();

    // ── Facturation / Commandes / Livraisons ──
    public DbSet<Fact> Facts => Set<Fact>();
    public DbSet<FactDetail> FactDetails => Set<FactDetail>();
    public DbSet<Cmd> Cmds => Set<Cmd>();
    public DbSet<CmdDetail> CmdDetails => Set<CmdDetail>();
    public DbSet<Commande> Commandes => Set<Commande>();
    public DbSet<CommandeDetail> CommandeDetails => Set<CommandeDetail>();
    public DbSet<Livraison> Livraisons => Set<Livraison>();
    public DbSet<LivraisonDetail> LivraisonDetails => Set<LivraisonDetail>();

    // ── Approvisionnement / Finance ──
    public DbSet<Appro> Appros => Set<Appro>();
    public DbSet<ApproDetail> ApproDetails => Set<ApproDetail>();
    public DbSet<ApproExpense> ApproExpenses => Set<ApproExpense>();
    public DbSet<Transformation> Transformations => Set<Transformation>();
    public DbSet<Taux> TauxChanges => Set<Taux>();
    public DbSet<Periode> Periodes => Set<Periode>();
    public DbSet<Caisse> Caisses => Set<Caisse>();
    public DbSet<Audit> Audits => Set<Audit>();
    public DbSet<Payment> Payments => Set<Payment>();

    // ── Stock / Mouvements ──
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<MouvementStock> MouvementsStock => Set<MouvementStock>();

    // ── Paramètres application ──
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<ShopProfile> ShopProfiles => Set<ShopProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Currency : PK explicite
        modelBuilder.Entity<Currency>().HasKey(c => c.CurrencyId);

        // Caisse : PK par convention + colonne calculée
        modelBuilder.Entity<Caisse>().HasKey(c => c.CaisseId);
        modelBuilder.Entity<Caisse>()
            .Property(c => c.TotalRestDay)
            .HasComputedColumnSql();

        // UserLocalisation : PK par convention
        modelBuilder.Entity<UserLocalisation>().HasKey(u => u.Id);

        // User -> UserLocalisations (1:N) — un utilisateur peut avoir plusieurs localisations
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserLocalisations)
            .WithOne(ul => ul.User)
            .HasForeignKey(ul => ul.UserId)
            .HasPrincipalKey(u => u.Login);

        // Localisation -> UserLocalisations (1:N) — une localisation peut avoir plusieurs utilisateurs
        modelBuilder.Entity<Localisation>()
            .HasMany(l => l.UserLocalisations)
            .WithOne(ul => ul.Localisation)
            .HasForeignKey(ul => ul.LocalisationId);

        // Article : colonne calculée TotalPrice
        modelBuilder.Entity<Article>()
            .Property(a => a.TotalPrice)
            .HasComputedColumnSql();

        // Relations Fact -> FactDetails
        modelBuilder.Entity<Fact>()
            .HasMany(f => f.Details)
            .WithOne(d => d.Fact)
            .HasForeignKey(d => d.IdFact);

        // Relations Fact -> Payments
        modelBuilder.Entity<Fact>()
            .HasMany(f => f.Payments)
            .WithOne(p => p.Fact)
            .HasForeignKey(p => p.IdFact);

        // ── Commande ──

        // Commande -> CommandeDetails (1:N)
        modelBuilder.Entity<Commande>()
            .HasMany(c => c.Details)
            .WithOne(d => d.Commande)
            .HasForeignKey(d => d.CommandeId);

        // Commande -> Livraisons (1:N)
        modelBuilder.Entity<Commande>()
            .HasMany(c => c.Livraisons)
            .WithOne(l => l.Commande)
            .HasForeignKey(l => l.CommandeId);

        // Commande -> Factures (1:N via Fact.CommandeId, ref directe)
        modelBuilder.Entity<Commande>()
            .HasMany(c => c.Factures)
            .WithOne(f => f.Commande)
            .HasForeignKey(f => f.CommandeId);

        // ── Livraison ──

        // Livraison -> LivraisonDetails (1:N)
        modelBuilder.Entity<Livraison>()
            .HasMany(l => l.Details)
            .WithOne(d => d.Livraison)
            .HasForeignKey(d => d.LivraisonId);

        // Livraison -> Fact (1:0..1 — au paiement)
        modelBuilder.Entity<Livraison>()
            .HasOne(l => l.Fact)
            .WithOne(f => f.Livraison)
            .HasForeignKey<Fact>(f => f.LivraisonId);

        // ── Cmd (commande interne) ──

        // Cmd -> CmdDetails (1:N)
        modelBuilder.Entity<Cmd>()
            .HasMany(c => c.Details)
            .WithOne(d => d.Cmd)
            .HasForeignKey(d => d.IdCmd);

        // ── Appro (approvisionnement) ──

        // Appro -> ApproDetails (1:N)
        modelBuilder.Entity<Appro>()
            .HasMany(a => a.Details)
            .WithOne(d => d.Appro)
            .HasForeignKey(d => d.IdAppro);

        // ApproDetail : PK
        modelBuilder.Entity<ApproDetail>().HasKey(a => a.Id);

        // ── Stock ──

        // Stock : PK
        modelBuilder.Entity<Stock>().HasKey(s => s.Id);

        // Stock : Index unique (Article, Localisation)
        modelBuilder.Entity<Stock>()
            .HasIndex(s => new { s.IdArticle, s.IdLocalisation })
            .IsUnique();

        // ── MouvementStock ──

        // MouvementStock : PK
        modelBuilder.Entity<MouvementStock>().HasKey(m => m.Id);

        // MouvementStock : Colonne calculée
        modelBuilder.Entity<MouvementStock>()
            .Property(m => m.ValeurTotale)
            .HasComputedColumnSql();

        // MouvementStock : Index pour recherche
        modelBuilder.Entity<MouvementStock>()
            .HasIndex(m => m.IdArticle);

        modelBuilder.Entity<MouvementStock>()
            .HasIndex(m => m.IdLocalisation);

        modelBuilder.Entity<MouvementStock>()
            .HasIndex(m => m.DateMouvement);

        modelBuilder.Entity<MouvementStock>()
            .HasIndex(m => new { m.TypeDocument, m.IdDocument });

        // ── RBAC : Role / Permission / UserRole ──

        // Role -> Permissions (1:N)
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithOne(p => p.Role)
            .HasForeignKey(p => p.RoleId);

        // Fonction -> Permissions (1:N)
        modelBuilder.Entity<Fonction>()
            .HasMany(f => f.Permissions)
            .WithOne(p => p.Fonction)
            .HasForeignKey(p => p.FonctionId);

        // Role -> Users (1:N) — un utilisateur a un seul rôle
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId);

        // Contrainte unique (RoleId, FonctionId) sur Permission
        modelBuilder.Entity<Permission>()
            .HasIndex(p => new { p.RoleId, p.FonctionId })
            .IsUnique();

        // Fonction -> Activities (1:N)
        modelBuilder.Entity<Fonction>()
            .HasMany(f => f.Activities)
            .WithOne(a => a.Fonction)
            .HasForeignKey(a => a.FonctionId);

        // User -> UserActivities (1:N)
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserActivities)
            .WithOne(ua => ua.User)
            .HasForeignKey(ua => ua.UserLogin)
            .HasPrincipalKey(u => u.Login);

        // Activity -> UserActivities (1:N)
        modelBuilder.Entity<Activity>()
            .HasMany(a => a.UserActivities)
            .WithOne(ua => ua.Activity)
            .HasForeignKey(ua => ua.ActivityId);

        // ShopProfile -> AppSetting (N:1)
        modelBuilder.Entity<ShopProfile>()
            .HasOne(p => p.AppNameSetting)
            .WithMany(a => a.ShopProfiles)
            .HasForeignKey(p => p.AppNameSettingKey)
            .HasPrincipalKey(a => a.Key)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Activity>()
            .HasIndex(a => a.Code)
            .IsUnique();

        modelBuilder.Entity<Activity>()
            .HasIndex(a => new { a.FonctionId, a.DescriptionActivity })
            .IsUnique();

        modelBuilder.Entity<UserActivity>()
            .HasIndex(ua => new { ua.UserLogin, ua.ActivityId })
            .IsUnique();
    }
}
