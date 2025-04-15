using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.IT;
using Ardalis.Specification;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApplicationCore.Services.IT;

public interface ICategoryService
{
   Task<IEnumerable<Category>> FetchAsync(string entityType);
   Task<IEnumerable<Category>> FetchAsync(ICollection<int> ids);
   Task<Category?> GetByIdAsync(int id, bool subItems = false);
   Task<Category> CreateAsync(Category Category);

   Task AddRangeAsync(ICollection<Category> entities);
   Task UpdateAsync(Category entity);
   Task UpdateRangeAsync(ICollection<Category> entities);
   Task RemoveAsync(Category entity);
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

   public async Task<IEnumerable<Category>> FetchAsync(ICollection<int> ids)
      => await _repository.ListAsync(new CategoriesSpecification(ids));

   public async Task<Category?> GetByIdAsync(int id, bool subItems = false)
   { 
      var entity = await _repository.GetByIdAsync(id);
      if (!subItems) return entity;
      if(entity is null) return null;

      var categories = await FetchAsync(entity.EntityType);
      entity.LoadSubItems(categories);
      return entity;
   }
   public async Task AddRangeAsync(ICollection<Category> entities)
      => await _repository.AddRangeAsync(entities);
   public async Task<Category> CreateAsync(Category Category)
      => await _repository.AddAsync(Category);

   public async Task UpdateAsync(Category Category)
      => await _repository.UpdateAsync(Category);

   public async Task UpdateRangeAsync(ICollection<Category> entities)
      => await _repository.UpdateRangeAsync(entities);

   public async Task RemoveAsync(Category entity)
   {
      entity.Removed = true;
      await _repository.UpdateAsync(entity);
   }

}
