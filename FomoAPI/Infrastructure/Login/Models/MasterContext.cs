using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FomoApi.Models
{
    public partial class MasterContext: IdentityDbContext<IdentityUser>
    {
        public MasterContext()
        {
        }

        public MasterContext(DbContextOptions<MasterContext> options)
            : base(options)
        {
        }

        //public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        //public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        //public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        //public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        //public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        //public virtual DbSet<Portfolios> Portfolios { get; set; }
        //public virtual DbSet<PortfolioSymbols> PortfolioSymbols { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasMaxLength(128)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            //modelBuilder.Ignore(typeof(AspNetUserRoles));
            //modelBuilder.Ignore(typeof(AspNetUserClaims));
            //modelBuilder.Ignore(typeof(AspNetUserLogins));
            //modelBuilder.Ignore(typeof(AspNetUserRoles));
            //modelBuilder.Ignore(typeof(AspNetUsers));
            //modelBuilder.Entity<IdentityUserClaim>(entity =>
            //{
            //    entity.HasKey(e => e.Id);

            //    entity.HasIndex(e => e.UserId)
            //        .HasName("IX_FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId");

            //    entity.Property(e => e.UserId)
            //        .IsRequired()
            //        .HasMaxLength(128);

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.AspNetUserClaims)
            //        .HasForeignKey(d => d.UserId)
            //        .HasConstraintName("FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId");
            //});

            //modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            //{
            //    entity.HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId });

            //    entity.HasIndex(e => e.UserId)
            //        .HasName("IX_FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId");

            //    entity.Property(e => e.LoginProvider).HasMaxLength(128);

            //    entity.Property(e => e.ProviderKey).HasMaxLength(128);

            //    entity.Property(e => e.UserId).HasMaxLength(128);

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.AspNetUserLogins)
            //        .HasForeignKey(d => d.UserId)
            //        .HasConstraintName("FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId");
            //});

            //modelBuilder.Entity<AspNetUserRoles>(entity =>
            //{
            //    entity.HasKey(e => new { e.RoleId, e.UserId });

            //    entity.Property(e => e.RoleId).HasMaxLength(128);

            //    entity.Property(e => e.UserId).HasMaxLength(128);
            //});

            //modelBuilder.Entity<AspNetUsers>(entity =>
            //{
            //    entity.HasKey(e => e.Id);

            //    entity.Property(e => e.Id)
            //        .HasMaxLength(128)
            //        .ValueGeneratedNever();

            //    entity.Property(e => e.Email).HasMaxLength(256);

            //    entity.Property(e => e.LockoutEndDateUtc).HasColumnType("datetime");

            //    entity.Property(e => e.UserName)
            //        .IsRequired()
            //        .HasMaxLength(256);
            //});

            //modelBuilder.Entity<Portfolios>(entity =>
            //{
            //    entity.HasKey(e => e.Id);

            //    entity.HasIndex(e => e.UserId)
            //        .HasName("IX_FK__Portfolio__UserI__239E4DCF");

            //    entity.Property(e => e.Id)
            //        .HasMaxLength(128)
            //        .ValueGeneratedNever();

            //    entity.Property(e => e.CreateDate).HasColumnType("datetime");

            //    entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

            //    entity.Property(e => e.Name)
            //        .IsRequired()
            //        .HasMaxLength(128);

            //    entity.Property(e => e.UserId).HasMaxLength(128);

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.Portfolios)
            //        .HasForeignKey(d => d.UserId)
            //        .HasConstraintName("FK__Portfolio__UserI__239E4DCF");
            //});

            //modelBuilder.Entity<PortfolioSymbols>(entity =>
            //{
            //    entity.HasKey(e => new { e.PortfolioId, e.Name });

            //    entity.HasIndex(e => e.UserId)
            //        .HasName("IX_FK_PortfolioSymbolsUserId");

            //    entity.Property(e => e.PortfolioId).HasMaxLength(128);

            //    entity.Property(e => e.Name).HasMaxLength(12);

            //    entity.Property(e => e.UserId)
            //        .IsRequired()
            //        .HasMaxLength(128);

            //    entity.HasOne(d => d.Portfolio)
            //        .WithMany(p => p.PortfolioSymbols)
            //        .HasForeignKey(d => d.PortfolioId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK__Portfolio__Portf__276EDEB3");

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.PortfolioSymbols)
            //        .HasForeignKey(d => d.UserId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_PortfolioSymbolsUserId");
            //});
        }
    }
}
