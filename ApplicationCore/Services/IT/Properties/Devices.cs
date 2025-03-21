using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.IT;
using Infrastructure.Helpers;

namespace ApplicationCore.Services.IT;

public interface IDeviceService
{
   Task<IEnumerable<Device>> FetchAsync(bool fired, ICollection<string>? includes = null);
   Task<IEnumerable<Device>> FetchAsync(bool fired, ICollection<Category> categories, ICollection<string>? includes = null);
   Task<IEnumerable<Device>> FetchNoneCategoryEntitiesAsync(bool fired, ICollection<string>? includes = null);
   Task<Device?> FindByCodeAsync(string code);
   Task<Device?> GetByIdAsync(int id);
   Task<Device> CreateAsync(Device entity, string userId);
   Task UpdateAsync(Device entity, string userId);
   Task RemoveAsync(Device entity, string userId);
   Task AddRangeAsync(ICollection<Device> entities);
   Task Sync();
}

public class DeviceService : IDeviceService
{
	private readonly IDefaultRepository<Device> _repository;

	public DeviceService(IDefaultRepository<Device> repository)
	{
      _repository = repository;
	}
   public async Task<IEnumerable<Device>> FetchAsync(bool fired, ICollection<string>? includes = null)
       => await _repository.ListAsync(new DeviceSpecification(fired, includes));

   public async Task<IEnumerable<Device>> FetchAsync(bool fired, ICollection<Category> categories, ICollection<string>? includes = null)
      => await _repository.ListAsync(new DeviceCategorySpecification(fired, categories.Select(x => x.Id).ToList(), includes));

   public async Task<IEnumerable<Device>> FetchNoneCategoryEntitiesAsync(bool fired, ICollection<string>? includes = null)
      => await _repository.ListAsync(new DeviceCategorySpecification(fired, includes));
   public async Task<Device?> FindByCodeAsync(string code)
      => await _repository.FirstOrDefaultAsync(new DevicebyCodeSpecification(code));

   public async Task<Device?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<Device> CreateAsync(Device entity, string userId)
   {
      entity.SetCreated(userId);
      return await _repository.AddAsync(entity);
   }
   public async Task AddRangeAsync(ICollection<Device> entities)
   {
      
       await _repository.AddRangeAsync(entities);
   }

   public async Task UpdateAsync(Device entity, string userId)
   {
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }

   public async Task RemoveAsync(Device entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }
   public async Task Sync()
   { 
      //var nonProperty = 
   }
}
