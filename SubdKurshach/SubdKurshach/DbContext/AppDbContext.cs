using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SubdKurshach.Models.Employees;
using SubdKurshach.Models.Families;
using SubdKurshach.Models.Info;
using SubdKurshach.Models.Users;

namespace SubdKurshach.DbContext
{
    public class AppDbContext:IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();

            if (usersRoles.ToList().Count() == 0)
            {
                UserRole userRoleAdmin = new UserRole() { RoleName = "Администратор" };
                usersRoles.Add(userRoleAdmin);
                UserRole userRoleEmployee = new UserRole() { RoleName = "Сотрудник" };
                usersRoles.Add(userRoleEmployee);
                UserRole userRoleUser = new UserRole() { RoleName = "Пользователь" };
                usersRoles.Add(userRoleUser);
                SaveChanges();
            }
        }
        public DbSet<UpdatedUserPassport> updatedUserPassports { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<UserAddress> userAddresses { get; set; }
        public DbSet<UserPassport> usersPassports { get; set; }
        public DbSet<UserRole> usersRoles { get; set; }
        public DbSet<Child> children { get; set; }
        public DbSet<AllChildrens> allChildrens { get; set; }
        public DbSet<Wife> wives { get; set; }
        public DbSet<Husband> husbands { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<Divorce> divorces { get; set; }
        public DbSet<Family> families { get; set; }
        public DbSet<Marriage> marriages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // equivalent of modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                // and modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
                entityType.GetForeignKeys()
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
            }
            base.OnModelCreating(builder);
        }
    }
}
