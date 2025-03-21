using ApplicationCore.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using ApplicationCore.Models.IT;

namespace ApplicationCore.DataAccess;
public class DefaultContext : IdentityDbContext<User, Role, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
{
  
   public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
	{
      
   }
   protected override void OnModelCreating(ModelBuilder builder)
   {
      base.OnModelCreating(builder);
      builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
      
      builder.UseOpenIddict();
   }
   public DbSet<App> Apps => Set<App>();
   public DbSet<Profiles> Profiles => Set<Profiles>(); 
   public DbSet<Department> Departments => Set<Department>();
   public DbSet<Location> Locations => Set<Location>();

   public DbSet<Item> Items => Set<Item>();
   public DbSet<ItemTransaction> ItemTransactions => Set<ItemTransaction>();
   public DbSet<ItemReport> ItemReports => Set<ItemReport>();
   public DbSet<ItemBalanceSheet> ItemBalanceSheets => Set<ItemBalanceSheet>();
   public DbSet<Category> Categories => Set<Category>();
   public DbSet<Brand> Brands => Set<Brand>();
   public DbSet<CategoryEntity> CategoryEntities => Set<CategoryEntity>();
   public DbSet<Property> Properties => Set<Property>();
   public DbSet<Device> Devices => Set<Device>();

   public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

}
