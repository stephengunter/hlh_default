using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.IT;
using Ardalis.Specification;
using Infrastructure.Interfaces;

namespace ApplicationCore.Services.IT;

public interface ICategoryService
{
   Task<IEnumerable<Category>> FetchAsync(string entityType);
   Task<Category?> GetByIdAsync(int id, bool subItems = false);
   Task<Category> CreateAsync(Category Category);
   Task UpdateAsync(Category Category);
   Task UpdateRangeAsync(ICollection<Category> entities);
}

public class CategorysService : ICategoryService
{
   private readonly IDefaultRepository<Category> _repository;

   public CategorysService(IDefaultRepository<Category> repository)
   {
      _repository = repository;
   }
   public async Task<IEnumerable<Category>> FetchAsync(string entityType)
      => await _repository.ListAsync(new CategoriesSpecification(entityType));

   public async Task<Category?> GetByIdAsync(int id, bool subItems = false)
   { 
      var entity = await _repository.GetByIdAsync(id);
      if (!subItems) return entity;
      if(entity is null) return null;

      var categories = await FetchAsync(entity.EntityType);
      entity.LoadSubItems(categories);
      return entity;
   }

   public async Task<Category> CreateAsync(Category Category)
      => await _repository.AddAsync(Category);

   public async Task UpdateAsync(Category Category)
      => await _repository.UpdateAsync(Category);

   public async Task UpdateRangeAsync(ICollection<Category> entities)
      => await _repository.UpdateRangeAsync(entities);

}
