using ApplicationCore.DataAccess;
using ApplicationCore.Models.Identity;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications.Identity;
using ApplicationCore.Specifications.IT;
using Infrastructure.Helpers;
using System.Text.RegularExpressions;

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
   Task SyncAsync(IEnumerable<Device> sourceDevices);
   Task RefreshAsync();
}

public class DeviceService : IDeviceService
{
   private readonly IDefaultRepository<Device> _repository;
   private readonly IDefaultRepository<Property> _propertyRepository;
   private readonly IDefaultRepository<Location> _locationRepository;
   private readonly IDefaultRepository<Category> _categoryRepository;
   private readonly IDefaultRepository<Profiles> _profilesrepository;
   public DeviceService(IDefaultRepository<Device> repository, IDefaultRepository<Property> propertyRepository, 
      IDefaultRepository<Category> categoryRepository,
      IDefaultRepository<Location> locationRepository, IDefaultRepository<Profiles> profilesrepository)
   {
      _repository = repository;
      _propertyRepository = propertyRepository;
      _categoryRepository = categoryRepository;
      _profilesrepository = profilesrepository;
      _locationRepository = locationRepository;

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
   public async Task SyncAsync(IEnumerable<Device> sourceDevices)
   {
      var list = await _repository.ListAsync();
      var updateList = new List<Device>();
      var addList = new List<Device>();
      foreach (var sourceDevice in sourceDevices)
      {
         var excepts = new List<string> { nameof(Device.Id) };
         var exist = list.FirstOrDefault(x => x.OldId == sourceDevice.OldId);
         if (exist is null)
         {
            var entity = new Device();
            sourceDevice.SetValuesTo(entity, excepts);
            addList.Add(entity);
         }
         else
         {
            sourceDevice.SetValuesTo(exist, excepts);
            updateList.Add(exist);
         }
      }
      if (updateList.HasItems()) await _repository.UpdateRangeAsync(updateList);
      if (addList.HasItems()) await _repository.AddRangeAsync(addList);
   }
   public async Task RefreshAsync()
   {
      var list = await _repository.ListAsync();
      var updateList = new List<Device>();
      foreach (var entity in list)
      {
         if (entity.Fired) entity.Order = -1;
         else entity.Order = 0;

         var properties = await _propertyRepository.ListAsync();
         SetPropertyId(entity, properties.ToList());

         var locations = await _locationRepository.ListAsync(new LocationsSpecification());
         SetLocationId(entity, locations.ToList());

         var categories = await _categoryRepository.ListAsync(new CategoriesSpecification(nameof(Device)));
         SetCategoryId(entity, categories.ToList());

         var profiles = await _profilesrepository.ListAsync();
         SetUserId(entity, profiles.ToList());

         updateList.Add(entity);
      }
      await _repository.UpdateRangeAsync(updateList);
   }
   void SetPropertyId(Device entity, IList<Property> properties)
   {
      entity.PropNum = entity.PropNum.IsNullOrEmpty() ? "" : entity.PropNum.Replace("-", "");
      Property? property = null;
      if (!string.IsNullOrEmpty(entity.PropNum)) property = properties.FirstOrDefault(x => x.Number == entity.PropNum);
      entity.PropertyId = property is null ? null : property.Id;
   }
   void SetLocationId(Device entity, IList<Location> locations)
   {
      Location? location = null;
      if (!string.IsNullOrEmpty(entity.Room))
      {
         location = locations.FirstOrDefault(x => x.Title == entity.Room.Trim());
         if (location == null)
         {
            location = locations.FirstOrDefault(x => x.Name == entity.Room.Trim());
         } 
      }
      entity.LocationId = location is null ? null : location.Id;
   }
   void SetCategoryId(Device entity, IList<Category> categories)
   {
      Category? category = null;
      string key = string.IsNullOrEmpty(entity.No) ? "" : Regex.Match(entity.No.Trim(), @"^[A-Za-z]+").Value;
      if (!string.IsNullOrEmpty(key)) category = categories.FirstOrDefault(x => x.Key == key);
      entity.CategoryId = category is null ? null : category.Id;
   }
   void SetUserId(Device entity, IList<Profiles> profiles)
   {
      Profiles? profile = null;
      if (!string.IsNullOrEmpty(entity.UserName)) profile = profiles.FirstOrDefault(x => x.Name == entity.UserName);
      entity.UserId = profile is null ? null : profile.UserId;
   }
}
