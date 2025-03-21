using ApplicationCore.DataAccess;
using ApplicationCore.Models.Identity;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.Identity;
using ApplicationCore.Specifications.IT;
using ApplicationCore.Views.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using System.Collections.Generic;

namespace ApplicationCore.Services.IT;

public interface IPropertyService
{
   Task<IEnumerable<Property>> FetchAsync(bool deprecated, ICollection<string>? includes = null);
   Task<Property?> FetchByNumberAsync(string num);
   Task<Property?> FindByNumberAsync(string num, bool deprecated, PropertyType type);
   Task<Property?> GetByIdAsync(int id);
   Task<Property> CreateAsync(Property entity, string userId);
   Task UpdateAsync(Property entity, string userId);
   Task RemoveAsync(Property entity, string userId);
   Task AddRangeAsync(ICollection<Property> entities);
   Task SyncAsync(IList<SourcePropertyModel> sourceProperties);
   Task RefreshAsync();
}

public class PropertyService : IPropertyService
{
	private readonly IDefaultRepository<Property> _repository;
   private readonly IDefaultRepository<Location> _locationRepository;
   private readonly IDefaultRepository<Category> _categoryRepository;
   private readonly IDefaultRepository<Profiles> _profilesrepository;
   public PropertyService(IDefaultRepository<Property> repository, IDefaultRepository<Category> categoryRepository,
      IDefaultRepository<Location> locationRepository, IDefaultRepository<Profiles> profilesrepository)
	{
      _repository = repository;
      _categoryRepository = categoryRepository;
      _profilesrepository = profilesrepository;
      _locationRepository = locationRepository;

   }
   public async Task<IEnumerable<Property>> FetchAsync(bool fired, ICollection<string>? includes = null)
       => await _repository.ListAsync(new PropertySpecification(fired, includes));
   public async Task<Property?> FetchByNumberAsync(string num)
      => await _repository.FirstOrDefaultAsync(new PropertyNumberSpecification(num));
   public async Task<Property?> FindByNumberAsync(string num, bool deprecated, PropertyType type)
      => await _repository.FirstOrDefaultAsync(new PropertyNumberSpecification(num, deprecated, type));

   public async Task<Property?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<Property> CreateAsync(Property entity, string userId)
   {
      entity.SetCreated(userId);
      return await _repository.AddAsync(entity);
   }
   public async Task AddRangeAsync(ICollection<Property> entities)
      => await _repository.AddRangeAsync(entities);

   public async Task UpdateAsync(Property entity, string userId)
   {
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }

   public async Task RemoveAsync(Property entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _repository.UpdateAsync(entity);
   }

   public async Task SyncAsync(IList<SourcePropertyModel> sourceProperties)
   {
      var list = await _repository.ListAsync();
      var updateList = new List<Property>();
      var addList = new List<Property>();
      foreach (var sourceProperty in sourceProperties)
      {
         var exist = list.FirstOrDefault(x => x.PropertyType == sourceProperty.PropertyType && x.Number == sourceProperty.Number);
         if (exist is null)
         {
            var entity = new Property();
            sourceProperty.SetValuesTo(entity);
            entity.LocationName = $"{sourceProperty.LocationCode},{sourceProperty.LocationName}";
            entity.UserName = sourceProperty.UserName;


            addList.Add(entity);
         }
         else
         {
            sourceProperty.SetValuesTo(exist);
            exist.LocationName = $"{sourceProperty.LocationCode},{sourceProperty.LocationName}";
            exist.UserName = sourceProperty.UserName;
            updateList.Add(exist);
         }
      }
      if (updateList.HasItems()) await _repository.UpdateRangeAsync(updateList);
      if (addList.HasItems()) await _repository.AddRangeAsync(addList);
   }
   public async Task RefreshAsync()
   {
      var list = await _repository.ListAsync();
      var updateList = new List<Property>();
      foreach (var entity in list)
      {
         var locations = await _locationRepository.ListAsync(new LocationsSpecification());
         SetLocationId(entity, locations.ToList());

         var categories = await _categoryRepository.ListAsync(new CategoriesSpecification(nameof(Property)));
         SetCategoryId(entity, categories.ToList());

         var profiles = await _profilesrepository.ListAsync();
         SetUserId(entity, profiles.ToList());

         updateList.Add(entity);
      }
      await _repository.UpdateRangeAsync(updateList);
   }
   void SetLocationId(Property entity, IList<Location> locations)
   {
      if (string.IsNullOrEmpty(entity.LocationName)) entity.LocationId = null;
      else
      {
         string locationCode = entity.LocationName.SplitToList()[0];
         var location = locations.FirstOrDefault(x => x.Code == locationCode);
         if (location == null) entity.LocationId = null;
         else entity.LocationId = location.Id;
      }
   }
   void SetCategoryId(Property entity, IList<Category> categories)
   {
      if (string.IsNullOrEmpty(entity.CategoryName)) entity.CategoryId = null;
      else
      {
         var category = categories.FirstOrDefault(x => x.Title == entity.CategoryName);
         if (category == null) entity.CategoryId = null;
         else entity.CategoryId = category.Id;
      }
   }
   void SetUserId(Property entity, IList<Profiles> profiles)
   {
      if (string.IsNullOrEmpty(entity.UserName)) entity.UserId = null;
      else
      {
         var profile = profiles.FirstOrDefault(x => x.Name == entity.UserName);
         if (profile == null) entity.UserId = null;
         else entity.UserId = profile.UserId;
      }
   }
}


