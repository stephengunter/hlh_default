using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;

namespace ApplicationCore.Services.IT;

public interface IItemBalanceSheetService
{
   Task<IEnumerable<ItemBalanceSheet>> FetchAsync(Item entity, ICollection<string>? includes = null);
   Task<IEnumerable<ItemBalanceSheet>> FetchAsync(ItemReport report, ICollection<string>? includes = null);
   Task<ItemBalanceSheet?> FindLatestAsync(Item entity);
   Task<ItemBalanceSheet?> GetByIdAsync(int id);
   Task<ItemBalanceSheet> CreateAsync(ItemBalanceSheet entity);
   Task UpdateAsync(ItemBalanceSheet entity);
   Task DeleteAsync(ItemBalanceSheet entity);
   Task AddRangeAsync(ICollection<ItemBalanceSheet> entities);
}

public class ItemBalanceSheetService : IItemBalanceSheetService
{
	private readonly IDefaultRepository<ItemBalanceSheet> _repository;

	public ItemBalanceSheetService(IDefaultRepository<ItemBalanceSheet> repository)
	{
      _repository = repository;
	}
   public async Task<IEnumerable<ItemBalanceSheet>> FetchAsync(Item entity, ICollection<string>? includes = null)
       => await _repository.ListAsync(new ItemBalanceSheetSpecification(entity, includes));

   public async Task<IEnumerable<ItemBalanceSheet>> FetchAsync(ItemReport report, ICollection<string>? includes = null)
       => await _repository.ListAsync(new ItemBalanceSheetSpecification(report, includes));

   public async Task<ItemBalanceSheet?> FindLatestAsync(Item entity)
   {
      var list = await FetchAsync(entity);
      if (list.IsNullOrEmpty()) return null;
      return list?.OrderByDescending(x => x.Date).FirstOrDefault();
   }
   public async Task<ItemBalanceSheet?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<ItemBalanceSheet> CreateAsync(ItemBalanceSheet entity)
      => await _repository.AddAsync(entity);
   public async Task AddRangeAsync(ICollection<ItemBalanceSheet> entities)
       => await _repository.AddRangeAsync(entities);

   public async Task UpdateAsync(ItemBalanceSheet entity)
      => await _repository.UpdateAsync(entity);

   public async Task DeleteAsync(ItemBalanceSheet entity)
      => await _repository.DeleteAsync(entity);
}
