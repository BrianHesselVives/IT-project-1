using System;
using System.Collections.Generic;
using MassageHuis.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageHuis.Data;

public partial class MassageHuisDbContext : DbContext
{
    public MassageHuisDbContext()
    {
    }

    public MassageHuisDbContext(DbContextOptions<MassageHuisDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Betaling> Betalings { get; set; }

    public virtual DbSet<KostPrijs> KostPrijs { get; set; }

    public virtual DbSet<Masseur> Masseurs { get; set; }

    public virtual DbSet<MasseurTypeMassage> MasseurTypeMassages { get; set; }

    public virtual DbSet<PromotieCode> PromotieCodes { get; set; }

    public virtual DbSet<RegulierTijdslot> RegulierTijdslots { get; set; }

    public virtual DbSet<ReservatiePromotieCode> ReservatiePromotieCodes { get; set; }

    public virtual DbSet<Reservatie> Reservaties { get; set; }

    public virtual DbSet<Schema> Schemas { get; set; }

    public virtual DbSet<TypeMassage> TypeMassages { get; set; }

    public virtual DbSet<UitzonderingTijdslot> UitzonderingTijdslots { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQL24_VIVES; Database=MassageHuis;Trusted_Connection=True; TrustServerCertificate=True;MultipleActiveResultSets=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Geslacht).HasMaxLength(50);
            entity.Property(e => e.Naam).HasMaxLength(100);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.Voornaam).HasMaxLength(100);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Betaling>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Betaling__3214EC077816066A");

            entity.ToTable("Betaling");

            entity.Property(e => e.Id)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasComment("UUID");
            entity.Property(e => e.Betaalmethode)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IdReservaties).HasColumnName("Id_Reservaties");
            entity.Property(e => e.Opmerking)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TransactieReferentie)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdReservatiesNavigation).WithMany(p => p.Betalings)
                .HasForeignKey(d => d.IdReservaties)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKBetaling230728");
        });

        modelBuilder.Entity<KostPrijs>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prijs__3214EC07411AC5FA");

            entity.Property(e => e.IdTypeMassage).HasColumnName("Id_TypeMassage");

            entity.HasOne(d => d.IdTypeMassageNavigation).WithMany(p => p.KostPrijs)
                .HasForeignKey(d => d.IdTypeMassage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPrijs356693");
        });

        modelBuilder.Entity<Masseur>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Masseur__3214EC07D33499EF");

            entity.ToTable("Masseur");

            entity.Property(e => e.Beschrijving)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IdAspNetUsers)
                .HasMaxLength(450)
                .HasColumnName("Id_AspNetUsers");

            entity.HasOne(d => d.IdAspNetUsersNavigation).WithMany(p => p.Masseurs)
                .HasForeignKey(d => d.IdAspNetUsers)
                .HasConstraintName("FK_Masseur_AspNetUsers");
        });

        modelBuilder.Entity<MasseurTypeMassage>(entity =>
        {
            entity.HasKey(e => new { e.IdMasseur, e.IdTypeMassage }).HasName("PK__Masseur___80E85080449674C6");

            entity.ToTable("Masseur_TypeMassage");

            entity.Property(e => e.IdMasseur).HasColumnName("Id_Masseur");
            entity.Property(e => e.IdTypeMassage).HasColumnName("Id_TypeMassage");

            entity.HasOne(d => d.IdMasseurNavigation).WithMany(p => p.MasseurTypeMassages)
                .HasForeignKey(d => d.IdMasseur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKMasseur_Ty373364");

            entity.HasOne(d => d.IdTypeMassageNavigation).WithMany(p => p.MasseurTypeMassages)
                .HasForeignKey(d => d.IdTypeMassage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKMasseur_Ty870180");
        });

        modelBuilder.Entity<PromotieCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promotie__3214EC074E2CCBFC");

            entity.ToTable("PromotieCode");

            entity.HasIndex(e => e.Code, "UQ__Promotie__A25C5AA72EB1EDE9").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IdAspNetUsers)
                .HasMaxLength(450)
                .HasColumnName("Id_AspNetUsers");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAspNetUsersNavigation).WithMany(p => p.PromotieCodes)
                .HasForeignKey(d => d.IdAspNetUsers)
                .HasConstraintName("FK_PromotieCode_AspNetUsers");
        });

        modelBuilder.Entity<RegulierTijdslot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Regulier__3214EC0771C1235F");

            entity.ToTable("RegulierTijdslot");

            entity.Property(e => e.IdSchema).HasColumnName("Id_Schema");

            entity.HasOne(d => d.IdSchemaNavigation).WithMany(p => p.RegulierTijdslots)
                .HasForeignKey(d => d.IdSchema)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKRegulierTi511075");
        });

        modelBuilder.Entity<ReservatiePromotieCode>(entity =>
        {
            entity.HasKey(e => new { e.IdReservaties, e.IdPromotieCode }).HasName("PK__Reservat__4AA863BE68267C6D");

            entity.ToTable("Reservatie_PromotieCode");

            entity.Property(e => e.IdReservaties).HasColumnName("Id_Reservaties");
            entity.Property(e => e.IdPromotieCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Id_PromotieCode");

            entity.HasOne(d => d.IdPromotieCodeNavigation).WithMany(p => p.ReservatiePromotieCodes)
                .HasForeignKey(d => d.IdPromotieCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKReservatie795620");
        });

        modelBuilder.Entity<Reservatie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservat__3214EC070AE0EE6F");

            entity.Property(e => e.IdAspNetUsers)
                .HasMaxLength(450)
                .HasColumnName("Id_AspNetUsers");
            entity.Property(e => e.IdPrijs).HasColumnName("Id_Prijs");
            entity.Property(e => e.IdPromotieCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Id_PromotieCode");
            entity.Property(e => e.IdRegulierTijdslot).HasColumnName("Id_RegulierTijdslot");
            entity.Property(e => e.IdTypeMassage).HasColumnName("Id_TypeMassage");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAspNetUsersNavigation).WithMany(p => p.Reservaties)
                .HasForeignKey(d => d.IdAspNetUsers)
                .HasConstraintName("FK_Reservaties_AspNetUsers");

            entity.HasOne(d => d.IdPrijsNavigation).WithMany(p => p.Reservaties)
                .HasForeignKey(d => d.IdPrijs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKReservatie487046");

            entity.HasOne(d => d.IdPromotieCodeNavigation).WithMany(p => p.Reservaties)
                .HasForeignKey(d => d.IdPromotieCode)
                .HasConstraintName("FKReservatie307222");

            entity.HasOne(d => d.IdRegulierTijdslotNavigation).WithMany(p => p.Reservaties)
                .HasForeignKey(d => d.IdRegulierTijdslot)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKReservatie515549");

            entity.HasOne(d => d.IdTypeMassageNavigation).WithMany(p => p.Reservaties)
                .HasForeignKey(d => d.IdTypeMassage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKReservatie952270");
        });

        modelBuilder.Entity<Schema>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schema__3214EC07A0AD3C59");

            entity.ToTable("Schema");

            entity.Property(e => e.IdMasseur).HasColumnName("Id_Masseur");
            entity.Property(e => e.Naam)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.IdMasseurNavigation).WithMany(p => p.Schemas)
                .HasForeignKey(d => d.IdMasseur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchema133092");
        });

        modelBuilder.Entity<TypeMassage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TypeMass__3214EC07EC691C22");

            entity.ToTable("TypeMassage");

            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UitzonderingTijdslot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Uitzonde__3214EC07D33295B0");

            entity.ToTable("UitzonderingTijdslot");

            entity.Property(e => e.IdSchema).HasColumnName("Id_Schema");
            entity.Property(e => e.TypeUitzondering)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdSchemaNavigation).WithMany(p => p.UitzonderingTijdslots)
                .HasForeignKey(d => d.IdSchema)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUitzonderi594035");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
