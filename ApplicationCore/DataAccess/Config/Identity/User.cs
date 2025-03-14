using ApplicationCore.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationCore.DataAccess;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{

      builder.HasOne(u => u.Profiles)
               .WithOne(p => p.User)
               .HasForeignKey<Profiles>(rt => rt.UserId);
		
   }
}
