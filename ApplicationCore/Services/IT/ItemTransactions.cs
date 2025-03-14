using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.IT;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;

namespace ApplicationCore.Services.IT;

public interface IItemTransactionService
{
   Task<int?> FindMinYearAsync();
   Task<IEnumerable<ItemTransaction>> FetchAsync(int year, int month, ICollection<string>? includes = null);
   Task<IEnumerable<ItemTransaction>> FetchAsync(DateTime sinceDate, DateTime endDate, ICollection<string>? includes = null);
   Task<IEnumerable<ItemTransaction>> FetchAsync(Item entity, ICollection<string>? includes = null);
   Task<IEnumerable<ItemTransaction>> FetchAsync(Item entity, DateTime sinceDate, DateTime endDate, ICollection<string>? includes = null);
   Task<ItemTransaction?> GetByIdAsync(int id);
   Task<ItemTransaction> CreateAsync(ItemTransaction entity, string userId);
   Task UpdateAsync(ItemTransaction entity, string userId);
   Task RemoveAsync(ItemTransaction entity, string userId);
   Task AddRangeAsync(ICollection<ItemTransaction> entities);
}

public class ItemTransactionService : IItemTransactionService
{
	private readonly IDefaultRepository<ItemTransaction> _repository;

	public ItemTransactionService(IDefaultRepository<ItemTransaction> repository)
	{
      _repository = repository;
	}
   public async Task<int?> FindMinYearAsync()
      => await _repository.FirstOrDefaultAsync(new ItemTransactionsMinYearSpecification());

   public async Task<IEnumerable<ItemTransaction>> FetchAsync(int year, int month, ICollection<string>? includes = null)
       => await _repository.ListAsync(new ItemTransactionYearMonthSpecification(year, month, includes));
   public async Task<IEnumerable<ItemTransaction>> FetchAsync(Item entity, ICollection<string>? includes = null)
       => await _repository.ListAsync(new ItemTransactionSpecification(entity, includes));

   public async Task<IEnumerable<ItemTransaction>> FetchAsync(Item entity, DateTime sinceDate, DateTime endDate, ICollection<string>? includes = null)
      => await _repository.ListAsync(new ItemTransactionSpecification(entity, sinceDate.ToStartDate(), endDate.ToEndDate(), includes));

   public async Task<IEnumerable<ItemTransaction>> FetchAsync(DateTime sinceDate, DateTime endDate, ICollection<string>? includes = null)
     => await _repository.ListAsync(new ItemTransactionSpecification(sinceDate.ToStartDate(), endDate.ToEndDate(), includes));


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

   public async Task RemoveAsync(ItemTransaction entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }
}
