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
using ApplicationCore.Models.Identity;
using Infrastructure.Consts;
using ApplicationCore.Views.IT;
using Infrastructure.Paging;
using ApplicationCore.Authorization;
using System.Collections.Generic;

namespace ITApi.Controllers.Admin;

public class PropertiesController : BaseAdminController
{
   private readonly IPropertyService _propertiesService;
   private readonly ICategoryService _categoryService;
   private readonly ILocationService _locationService;
   private readonly IMapper _mapper;
   public PropertiesController(IPropertyService propertiesService, ICategoryService categoryService,
      ILocationService locationService, IMapper mapper)
   {
      _propertiesService = propertiesService;
      _categoryService = categoryService;
      _locationService = locationService;
      _mapper = mapper;
   }
   PropertyLabels Labels => new PropertyLabels();
   bool CanRemove(Property entity)
   {
      return true;
   }

   [HttpGet("init")]
   public async Task<ActionResult<PropertiesAdminModel>> Init()
   {
      var categories = await _categoryService.FetchAsync(nameof(Property));
      categories = categories.GetOrdered();

      var locations = await _locationService.FetchAsync();

      bool deprecated = false;
      int down = 0;
      int type = -1;
      int category = 0;
      int? location = null;
      int page = 1;
      int pageSize = 10;
      var request = new PropertiesFetchRequest(deprecated, down, type, category, location, page, pageSize);
      var model = new PropertiesAdminModel(request, categories.MapViewModelList(_mapper), locations.MapViewModelList(_mapper));

      return model;
   }

   async Task<IEnumerable<Property>?> FetchAsync(bool deprecated, int down, int type)
   {
      var includes = new List<string>();
      var properties = await _propertiesService.FetchAsync(deprecated, includes);

      if(down < 0) properties = properties.Where(x => !x.Active);
      else if (down > 0) properties = properties.Where(x => x.Active);

      if (type == 0) properties = properties.Where(x => x.PropertyType == PropertyType.Item);
      else if (type == 1) properties = properties.Where(x => x.PropertyType == PropertyType.Property);

      return properties;
   }

   [HttpGet]
   public async Task<ActionResult<PropertiesIndexModel>> Index(bool deprecated, int down, int type, int category, int? location, int page = 1, int pageSize = 10)
   {
      Category? selectedCategory = null;
      if (category > 0)
      {
         selectedCategory = await _categoryService.GetByIdAsync(category);
         if (selectedCategory is null)
         {
            ModelState.AddModelError(nameof(Category), ValidationMessages.NotExist(Labels.Category));
            return BadRequest(ModelState);
         }
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

      var properties = await FetchAsync(deprecated, down, type);
      if (selectedCategory != null) properties = properties!.Where(x => x.CategoryId == selectedCategory!.Id);
      else if (category < 0) properties = properties!.Where(x => !x.CategoryId.HasValue);

      if (selectedLocation != null) properties = properties!.Where(x => x.LocationId == selectedLocation!.Id);

      properties = properties!.GetOrdered();

      var groupViews = new List<PropertiesGroupView>();
      if (category == 0 && properties!.HasItems())
      {
         groupViews = properties!.GetPropertiesGroupViews();
      }

      var pageList = properties!.GetPagedList(_mapper, page, pageSize);
      return new PropertiesIndexModel(pageList, groupViews);
   }
   [HttpPost("upload")]
   public async Task<ActionResult<PagedList<SourcePropertyModel>>> Upload([FromForm] PropertiesUploadRequest request)
   {
      var propType = (PropertyType)request.PropertyType;
      if (propType == PropertyType.UnKnown)
      {
         ModelState.AddModelError(nameof(PropertyType), ValidationMessages.NotExist(Labels.Type));
         return BadRequest(ModelState);
      }
      var file = request.File;
      var errors = ValidateExcelFile(file!);
      AddErrors(errors);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var sourceList = new List<SourcePropertyModel>();
      using (var stream = new MemoryStream())
      {
         await file!.CopyToAsync(stream);
         sourceList = PropertyExcelHelpers.GetPropertyListFromFile(stream, propType);
      }
      sourceList = sourceList.OrderBy(item => item.CategoryName).ToList();
      return sourceList.GetPagedList();
   }

   async Task AddPropertyCategoriesAsync(IEnumerable<string> categoryNames)
   {
      var categories = await _categoryService.FetchAsync(nameof(Property));
      var existCategoryNames = categories.Select(x => x.Title);
      var newCategoryNames = categoryNames.Where(x => !existCategoryNames.Contains(x));
      if (newCategoryNames.HasItems())
      {
         var newCategories = newCategoryNames.Select(name => new Category { Title = name, EntityType = nameof(Property) });
         await _categoryService.AddRangeAsync(newCategories.ToList());
      }
   }

   [HttpPost("imports")]
   public async Task<ActionResult> Imports([FromBody] PropertiesImportRequest request)
   {
      var sourceList = request.List;
      var categoryNames = sourceList.Select(item => item.CategoryName).Distinct();
      await AddPropertyCategoriesAsync(categoryNames);
      await _propertiesService.SyncAsync(sourceList);
      await _propertiesService.RefreshAsync();
      return NoContent();
   }

   [HttpPost("reports")]
   public async Task<IActionResult> Reports(PropertiesFetchRequest request)
   {
      bool deprecated = false;
      int down = 0;
      int type = -1;
      if (!request.Location.HasValue)
      {
         ModelState.AddModelError(nameof(Location), ValidationMessages.Required(Labels.Location));
         return BadRequest(ModelState);
      }

      Location? selectedLocation = null;
      selectedLocation = await _locationService.GetByIdAsync(request.Location!.Value);
      if (selectedLocation is null)
      {
         ModelState.AddModelError(nameof(Location), ValidationMessages.NotExist(Labels.Location));
         return BadRequest(ModelState);
      }

      var properties = await FetchAsync(deprecated, down, type);
      properties = properties!.Where(x => x.LocationId == selectedLocation!.Id);

      if (properties.IsNullOrEmpty())
      {
         ModelState.AddModelError("", "查無財產可輸出.");
         return BadRequest(ModelState);
      }

      properties = properties!.GetOrdered();

      var groupViews = properties!.GetPropertiesGroupViews();
      var views = properties!.MapViewModelList(_mapper);
      var categories = await _categoryService.FetchAsync(nameof(Property));
      var locations = await _locationService.FetchAsync();
      foreach (var view in views) 
      {
         var category = categories.FirstOrDefault(x => x.Id == view.CategoryId);
         if (category != null) view.CategoryName = category.Title;

         var location = locations.FirstOrDefault(x => x.Id == view.LocationId);
         if (location != null) view.LocationName = location.Title;
      }
      foreach (var groupView in groupViews)
      {
         var category = categories.FirstOrDefault(x => x.Id == groupView.CategoryId);
         if (category != null) groupView.CategoryName = category.Title;
      }
      string title = $"存置地點 : {selectedLocation.Title}          製表日期 : {DateTime.Today.ToRocDateString()}";
      var stream = PropertyExcelHelpers.CreateReportExcel(title, views, groupViews);

      string excelFileName = "properties.xlsx";
      string contentType = FileContentType.Excel;
      return File(stream, contentType, excelFileName);
   }
   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _propertiesService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      if (!CanRemove(entity))
      {
         ModelState.AddModelError("active", "此紀錄不允許刪除");
         return BadRequest(ModelState);
      }

      await _propertiesService.RemoveAsync(entity, User.Id());

      return NoContent();
   }

   [HttpGet("editCategory/{id}")]
   public async Task<ActionResult<CategoryEditRequest>> EditCategory(int id)
   {
      var category = await _categoryService.GetByIdAsync(id);
      if (category == null) return NotFound();

      var props = await _propertiesService.FetchAsync(category);
      bool canRemove = props.IsNullOrEmpty();
      var form = new CategoryEditForm();
      category.SetValuesTo(form);

      return new CategoryEditRequest(form, canRemove);
   }
   [HttpDelete("removeCategory/{id}")]
   public async Task<IActionResult> RemoveCategory(int id)
   {
      var category = await _categoryService.GetByIdAsync(id);
      if (category == null) return NotFound();

      var props = await _propertiesService.FetchAsync(category);
      bool canRemove = props.IsNullOrEmpty();

      if (!canRemove)
      {
         ModelState.AddModelError("", "此紀錄不允許刪除");
         return BadRequest(ModelState);
      }

      await _categoryService.RemoveAsync(category);
      return NoContent();
   }


   async Task<Category?> ValidateCategoryAsync(int? categoryId)
   {
      Category? category  = null;
      if (categoryId.HasValue) category = await _categoryService.GetByIdAsync(categoryId.Value);
      
      if (category is null) ModelState.AddModelError(nameof(Category), ValidationMessages.NotExist(Labels.Category));
      return category!;
   }
   async Task<Location?> ValidateLocationAsync(int? locationId)
   {
      Location? location = null;
      if (locationId.HasValue) location = await _locationService.GetByIdAsync(locationId.Value);

      if (location is null) ModelState.AddModelError(nameof(Category), ValidationMessages.NotExist(Labels.Location));
      return location!;
   }
   

}
