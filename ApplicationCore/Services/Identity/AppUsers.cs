using ApplicationCore.DataAccess;
using ApplicationCore.Models.Identity;
using ApplicationCore.Specifications.Identity;

namespace ApplicationCore.Services.Identity;

public interface IAppUsersService
{
   Task<IEnumerable<User>> FetchAllAsync(bool includeRoles = false);
}

public class AppUsersService : IAppUsersService
{
   private readonly IDefaultRepository<User> _usersRepository;

   public AppUsersService(IDefaultRepository<User> usersRepository)
   {
      _usersRepository = usersRepository;
   }
   public async Task<IEnumerable<User>> FetchAllAsync(bool includeRoles = false)
    => await _usersRepository.ListAsync(new UsersSpecification(includeRoles));

}
