using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;

namespace ApplicationCore.Services.IT;

public interface IItemTransactionService
{
   Task<IEnumerable<ItemTransaction>> FetchAsync(Item entity, ICollection<string>? includes = null);
   Task<IEnumerable<ItemTransaction>> FetchAsync(DateTime sinceDate, ICollection<string>? includes = null);
   Task<IEnumerable<ItemTransaction>> FetchAsync(DateTime sinceDate, DateTime endDate, ICollection<string>? includes = null);
   Task<IEnumerable<ItemTransaction>> FetchAsync(DateTime sinceDate, DateTime endDate, Item entity, ICollection<string>? includes = null);
   Task<ItemTransaction?> GetByIdAsync(int id);
   Task<ItemTransaction> CreateAsync(ItemTransaction entity, string userId);
   Task UpdateAsync(ItemTransaction entity, string userId);
   Task RemoveAsync(ItemTransaction entity, string userId);
   Task AddRangeAsync(ICollection<ItemTransaction> entities);
   Task UpdateRangeAsync(ICollection<ItemTransaction> entities);
}

public class ItemTransactionService : IItemTransactionService
{
	private readonly IDefaultRepository<ItemTransaction> _repository;

	public ItemTransactionService(IDefaultRepository<ItemTransaction> repository)
	{
      _repository = repository;
	}
   public async Task<IEnumerable<ItemTransaction>> FetchAsync(Item entity, ICollection<string>? includes = null)
       => await _repository.ListAsync(new ItemTransactionSpecification(entity, includes));

   public async Task<IEnumerable<ItemTransaction>> FetchAsync(DateTime sinceDate, ICollection<string>? includes = null)
      => await _repository.ListAsync(new ItemTransactionDateSpecification(sinceDate, includes));

   public async Task<IEnumerable<ItemTransaction>> FetchAsync(DateTime sinceDate, DateTime endDate, Item entity, ICollection<string>? includes = null)
      => await _repository.ListAsync(new ItemTransactionDateSpecification(sinceDate.ToStartDate(), endDate.ToEndDate(), entity, includes));

   public async Task<IEnumerable<ItemTransaction>> FetchAsync(DateTime sinceDate, DateTime endDate, ICollection<string>? includes = null)
     => await _repository.ListAsync(new ItemTransactionDateSpecification(sinceDate.ToStartDate(), endDate.ToEndDate(), includes));


   public async Task<ItemTransaction?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<ItemTransaction> CreateAsync(ItemTransaction entity, string userId)
   {
      entity.SetCreated(userId);
      return await _repository.AddAsync(entity);
   }
   public async Task AddRangeAsync(ICollection<ItemTransaction> entities)
      => await _repository.AddRangeAsync(entities);

   public async Task UpdateAsync(ItemTransaction entity, string userId)
   {
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }
   public async Task UpdateRangeAsync(ICollection<ItemTransaction> entities)
    => await _repository.UpdateRangeAsync(entities);

   public async Task RemoveAsync(ItemTransaction entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }
}
