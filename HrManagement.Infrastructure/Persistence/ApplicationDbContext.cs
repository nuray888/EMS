using HrManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();
        public DbSet<SalaryPayment> SalaryPayments => Set<SalaryPayment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(200);
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Age).IsRequired();
                entity.Property(u => u.Salary).IsRequired().HasColumnType("decimal(18,2)");

                entity.HasOne(u => u.Department)
                      .WithMany(d => d.Employees)
                      .HasForeignKey(u => u.DepartmentId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired();
                entity.HasIndex(rt => new { rt.UserId, rt.Token }).IsUnique();
            });

            builder.Entity<PasswordHistory>(entity =>
            {
                entity.HasKey(ph => ph.Id);
                entity.Property(ph => ph.PasswordHash).IsRequired();
                entity.HasIndex(ph => ph.UserId);
            });

            builder.Entity<SalaryPayment>(entity =>
            {
                entity.HasKey(sp => sp.Id);
                entity.Property(sp => sp.Amount).HasColumnType("decimal(18,2)");
                entity.HasIndex(sp => sp.UserId);
            });
        }
    }
}