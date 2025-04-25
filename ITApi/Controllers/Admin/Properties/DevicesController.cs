using Microsoft.AspNetCore.Mvc;
using Infrastructure.Helpers;
using ApplicationCore.Models.IT;
using Ardalis.Specification;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Helpers.IT;
using ApplicationCore.Services.IT;
using ITApi.Models;
using ApplicationCore.Services.Identity;
using Infrastructure.Paging;
using ApplicationCore.Views.IT;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.Models.Identity;

namespace ITApi.Controllers.Admin;

public class DevicesController : BaseAdminController
{
   private readonly HLHDBSettings _HLHDBSettings;
   private readonly IDeviceService _deviceService;
   private readonly IPropertyService _propertiesService;
   private readonly ICategoryService _categoryService;
   private readonly ILocationService _locationService;
   private readonly IProfilesService _profilesService;
   private readonly IMapper _mapper;
   public DevicesController(IOptions<HLHDBSettings> HLHDBSettings, 
      IDeviceService deviceService, IPropertyService propertiesService, ICategoryService categoryService,
      ILocationService locationService, IProfilesService profilesService, IMapper mapper)
   {
      _HLHDBSettings = HLHDBSettings.Value;
      _deviceService = deviceService;
      _propertiesService = propertiesService;
      _categoryService = categoryService;
      _locationService = locationService;
      _profilesService = profilesService;
      _mapper = mapper;
   }
   DeviceLabels Labels => new DeviceLabels();
   bool CanRemove(Device entity)
   {
      if (entity.Active) return false;
      return true;
   }

   [HttpGet("init")]
   public async Task<ActionResult<DevicesAdminModel>> Init()
   {
      var locations = await _locationService.FetchAsync();

      var categories = await _categoryService.FetchAsync(nameof(Device));
      categories = categories.GetOrdered();
      var categoryIds = categories.Select(c => c.Id).ToList();
      var root = categories.FirstOrDefault(x => x.IsRootItem);
      root!.Title = "所有設備分類";
      root!.LoadSubItems(categories);
      
      var request = new DevicesFetchRequest();
      request.Category = root.Id;
      request.Page = 1;
      request.PageSize = 10;
      var model = new DevicesAdminModel(request, root.MapViewModel(_mapper), 
         categories.MapViewModelList(_mapper), locations.MapViewModelList(_mapper));

      model.Categories = categories.MapViewModelList(_mapper);

      return model;
   }
   async Task<IEnumerable<Device>> FetchAsync(bool fired, Category? selectedCategory, Location? selectedLocation)
   {
      var includes = new List<string>();
      IEnumerable<Device> devices;
      if (selectedCategory is null)
      {
         devices = await _deviceService.FetchNoneCategoryEntitiesAsync(fired, includes);
      }
      else
      {
         var categories = new List<Category>() { selectedCategory };
         if (selectedCategory.SubItems!.HasItems()) categories.AddRange(selectedCategory.GetAllSubItems());

         devices = await _deviceService.FetchAsync(fired, categories, includes);
      }

      if (selectedLocation != null) devices = devices.Where(x => x.LocationId == selectedLocation!.Id);
      devices = devices.GetOrdered();

      return devices;
   }

   [HttpGet]
   public async Task<ActionResult<DevicesIndexModel>> Index(bool fired, int category, int? location, int page = 1, int pageSize = 10)
   {
      Category? selectedCategory = null;
      if (category > 0)
      {
         selectedCategory = await ValidateCategoryAsync(category, subItems: true);
         if (!ModelState.IsValid) return BadRequest(ModelState);
      }

      Location? selectedLocation = null;
      if (location.HasValue)
      {
         selectedLocation = await _locationService.GetByIdAsync(location.Value);
         if (selectedLocation is null)
         {
            ModelState.AddModelError(nameof(Location), ValidationMessages.NotExist(Labels.Location));
            return BadRequest(ModelState);
         }
      }
      var devices = await FetchAsync(fired, selectedCategory, selectedLocation);

      devices = devices!.GetOrdered();

      var groupViews = new List<DevicesGroupView>();
      if (selectedCategory != null && selectedCategory.IsRootItem && devices!.HasItems())
      {
         groupViews = devices!.GetPropertiesGroupViews();
      }

      var pageList = devices!.GetPagedList(_mapper, page, pageSize);
      return new DevicesIndexModel(pageList, groupViews);
   }

   [HttpPost("imports")]
   public async Task<ActionResult> Imports([FromForm] DevicesImportRequest request)
   {
      
      //var file = request.File;
      //var errors = ValidateExcelFile(file!);
      //AddErrors(errors);
      //if (!ModelState.IsValid) return BadRequest(ModelState);

      ////var list = new List<Device>();
      ////using (var stream = new MemoryStream())
      ////{
      ////   await file!.CopyToAsync(stream);
      ////   list = DeviceExcelHelpers.GetDeviceListFromFile(stream);
      ////}
      ////await _deviceService.SyncAsync(list);
      await _deviceService.RefreshAsync();
      return NoContent();
   }

   //[HttpGet("create")]
   //public ActionResult<DeviceAddForm> Create()
   //   => new DeviceAddForm();

   //[HttpPost]
   //public async Task<ActionResult> Store([FromBody] DeviceAddForm form)
   //{
   //   await ValidateRequestAsync(form);
   //   if (!ModelState.IsValid) return BadRequest(ModelState);

   //   var entity = new Device();
   //   form.SetValuesTo(entity);
   //   entity.SetActive(form.Active);

   //   await _deviceService.CreateAsync(entity, User.Id());
   //   return Ok();
   //}
   //[HttpGet("edit/{id}")]
   //public async Task<ActionResult<DeviceEditForm>> Edit(int id)
   //{
   //   var entity = await _deviceService.GetByIdAsync(id);
   //   if (entity == null) return NotFound();

   //   var form = new DeviceEditForm();
   //   entity.SetValuesTo(form);
   //   form.Active = entity.Active;

   //   form.CanRemove = CanRemove(entity);
   //   return form;
   //}
   //[HttpPut("{id}")]
   //public async Task<IActionResult> Update(int id, [FromBody] DeviceEditForm form)
   //{
   //   var entity = await _deviceService.GetByIdAsync(id);
   //   if (entity == null) return NotFound();

   //   await ValidateRequestAsync(form, id);
   //   if (!ModelState.IsValid) return BadRequest(ModelState);

   //   form.SetValuesTo(entity);
   //   entity.SetActive(form.Active);

   //   await _deviceService.UpdateAsync(entity, User.Id());
   //   return NoContent();
   //}
   //[HttpDelete("{id}")]
   //public async Task<IActionResult> Remove(int id)
   //{
   //   var entity = await _deviceService.GetByIdAsync(id);
   //   if (entity == null) return NotFound();

   //   if (!CanRemove(entity))
   //   {
   //      ModelState.AddModelError("active", "此紀錄不允許刪除");
   //      return BadRequest(ModelState);
   //   }

   //   await _deviceService.RemoveAsync(entity, User.Id());

   //   return NoContent();
   //}

   //async Task ValidateRequestAsync(BaseDeviceForm model, int id = 0)
   //{
   //   var labels = new DeviceLabels();
   //   if (string.IsNullOrEmpty(model.Name))
   //   {
   //      ModelState.AddModelError(nameof(model.Name), ValidationMessages.Required(labels.Name));
   //   }
   //   if (string.IsNullOrEmpty(model.Code))
   //   {
   //      ModelState.AddModelError(nameof(model.Code), ValidationMessages.Required(labels.Code));
   //   }

   //   if (!ModelState.IsValid) return;
   //   if (id == 0) return;

   //   var exist = await _deviceService.FindByCodeAsync(model.Code);
   //   if (exist is not null && exist.Id != id)
   //   {
   //      ModelState.AddModelError(nameof(model.Code), ValidationMessages.Duplicate(labels.Code));
   //   }
   //}
   async Task<Category> ValidateCategoryAsync(int categoryId, bool subItems)
   {
      var category = await _categoryService.GetByIdAsync(categoryId, subItems);
      if (category is null)
      {
         ModelState.AddModelError(nameof(Category), ValidationMessages.NotExist(Labels.Category));
      }
      return category!;
   }
   
}
