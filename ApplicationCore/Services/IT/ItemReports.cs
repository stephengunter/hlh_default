using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.IT;
using Infrastructure.Helpers;

namespace ApplicationCore.Services.IT;

public interface IItemReportService
{
   Task<IEnumerable<int>> FetchYearsAsync();
   Task<IEnumerable<ItemReport>> FetchAsync(int year);
   Task<ItemReport?> GetByIdAsync(int id);
   Task<ItemReport> CreateAsync(ItemReport entity, string userId);
   Task UpdateAsync(ItemReport entity, string userId);
   Task DeleteAsync(ItemReport entity);
   Task AddRangeAsync(ICollection<ItemReport> entities);
}

public class ItemReportService : IItemReportService
{
	private readonly IDefaultRepository<ItemReport> _repository;

	public ItemReportService(IDefaultRepository<ItemReport> repository)
	{
      _repository = repository;
	}
   public async Task<IEnumerable<ItemReport>> FetchAsync(int year)
       => await _repository.ListAsync(new ItemReportSpecification(year));

   public async Task<IEnumerable<int>> FetchYearsAsync()
   {
      var list = await _repository.ListAsync();
      return list.Select(x => x.Year).Distinct().OrderByDescending(year => year);
   }

   public async Task<ItemReport?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<ItemReport> CreateAsync(ItemReport entity, string userId)
   {
      entity.SetCreated(userId);
      return await _repository.AddAsync(entity);
   }
   public async Task AddRangeAsync(ICollection<ItemReport> entities)
   {
      
       await _repository.AddRangeAsync(entities);
   }

   public async Task UpdateAsync(ItemReport entity, string userId)
   {
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }

   public async Task DeleteAsync(ItemReport entity)
     => await _repository.DeleteAsync(entity);

}
