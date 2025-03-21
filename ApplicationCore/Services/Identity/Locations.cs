using ApplicationCore.DataAccess;
using ApplicationCore.Models.Identity;
using ApplicationCore.Specifications.Identity;

namespace ApplicationCore.Services.Identity;

public interface ILocationService
{
   Task<IEnumerable<Location>> FetchAsync();
   Task<Location?> GetByIdAsync(int id);
   Task<Location> CreateAsync(Location entity);
   Task UpdateAsync(Location entity);
   Task RemoveAsync(Location entity);
}

public class LocationService : ILocationService
{
   private readonly IDefaultRepository<Location> _repository;
   public LocationService(IDefaultRepository<Location> repository)
   {
      _repository = repository;
   }
   public async Task<IEnumerable<Location>> FetchAsync()
       => await _repository.ListAsync(new LocationsSpecification());

   public async Task<Location?> GetByIdAsync(int id)
      => await _repository.FirstOrDefaultAsync(new LocationsSpecification(id));

   public async Task<Location> CreateAsync(Location entity)
      => await _repository.AddAsync(entity);
   
   public async Task UpdateAsync(Location entity)
     => await _repository.UpdateAsync(entity);

   public async Task RemoveAsync(Location entity)
   {
      entity.Removed = true;
      await _repository.UpdateAsync(entity);
   }
}
