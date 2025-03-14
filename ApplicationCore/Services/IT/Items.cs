using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.IT;
using Infrastructure.Helpers;

namespace ApplicationCore.Services.IT;

public interface IItemService
{
   Task<IEnumerable<Item>> FetchAsync();
   Task<Item?> FindByCodeAsync(string code);
   Task<Item?> GetByIdAsync(int id);
   Task<Item> CreateAsync(Item entity, string userId);
   Task UpdateAsync(Item entity, string userId);
   Task RemoveAsync(Item entity, string userId);
   Task AddRangeAsync(ICollection<Item> entities);
}

public class ItemService : IItemService
{
	private readonly IDefaultRepository<Item> _repository;

	public ItemService(IDefaultRepository<Item> repository)
	{
      _repository = repository;
	}
   public async Task<IEnumerable<Item>> FetchAsync()
       => await _repository.ListAsync(new ItemSpecification());

   public async Task<Item?> FindByCodeAsync(string code)
      => await _repository.FirstOrDefaultAsync(new ItembyCodeSpecification(code));

   public async Task<Item?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<Item> CreateAsync(Item entity, string userId)
   {
      entity.SetCreated(userId);
      return await _repository.AddAsync(entity);
   }
   public async Task AddRangeAsync(ICollection<Item> entities)
   {
      
       await _repository.AddRangeAsync(entities);
   }

   public async Task UpdateAsync(Item entity, string userId)
   {
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }

   public async Task RemoveAsync(Item entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }

}
