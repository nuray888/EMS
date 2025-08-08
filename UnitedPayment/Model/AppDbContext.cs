using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace UnitedPayment.Model
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Configuration.GetConnectionString("ApiDatabase"));

        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Department> Department { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany(x => x.Departments)
                .WithMany(y => y.Employees)
                .UsingEntity(j => j.ToTable("Employee_Department"));
            //modelBuilder.Entity<Department>()
            //    .HasOne(d => d.Manager)
            //    .WithOne(m => m.Department)
            //    .HasForeignKey<Manager>(m => m.DepartmentId);

            modelBuilder.Entity<User>()
            .Property(m => m.Role)
            .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }

    }
}
