using Microsoft.AspNetCore.Mvc;
using Infrastructure.Helpers;
using ApplicationCore.Authorization;
using ApplicationCore.Models.IT;
using Ardalis.Specification;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Helpers.IT;
using ApplicationCore.Services.IT;
using ITApi.Models;
using ApplicationCore.Services.Identity;
using ApplicationCore.DataAccess;
using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using Infrastructure.Paging;
using System.Drawing.Printing;
using ApplicationCore.Views.IT;

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
      if (entity.Active) return false;
      return true;
   }

   [HttpGet("init")]
   public async Task<ActionResult<PropertiesAdminModel>> Init()
   {
      var categories = await _categoryService.FetchAsync(nameof(Property));
      categories = categories.GetOrdered();

      var locations = await _locationService.FetchAsync();

      bool deprecated = false;
      int? category = null;
      var model = new PropertiesAdminModel(new PropertiesFetchRequest(deprecated, category), categories.MapViewModelList(_mapper),
         locations.MapViewModelList(_mapper));

      return model;
   }

   [HttpGet]
   public async Task<ActionResult<PagedList<Property, PropertyViewModel>>> Index(bool deprecated, int? category, int page = 1, int pageSize = 10)
   {
      var includes = new List<string>();
      
      Category? selectedCategory = null;
      if (category.HasValue)
      {
         selectedCategory = await ValidateCategoryAsync(category.Value);
         if (!ModelState.IsValid) return BadRequest(ModelState);
      }

      var properties = await _propertiesService.FetchAsync(deprecated, includes);
      if (selectedCategory != null) properties = properties.Where(x => x.CategoryId == category!.Value);
         

      var model = new PagedList<Property, PropertyViewModel>(properties, page, pageSize);

      model.SetViewList(model.List.MapViewModelList(_mapper));
      return model;
      //if (!active) return new PropertysIndexModel(devices.MapViewModelList(_mapper));

      //var lastClosed = await _deviceReportService.GetLastClosedAsync();
      //var sheets = await _deviceBalanceSheetService.FetchAsync(lastClosed!);

      //var date = lastClosed!.GetDate();
      //var transactions = await _transactionService.FetchAsync(date!.Value.AddDays(1));
      //var year_transactions = await _transactionService.FetchAsync(DateTime.Today.AddYears(-1));
      //foreach (var device in devices)
      //{
      //   int lastStock = 0;
      //   var sh = sheets.FirstOrDefault(x => x.PropertyId == device.Id);
      //   if (sh != null) lastStock = sh.Stock;

      //   var trans = transactions.Where(x => x.PropertyId == device.Id);
      //   if (trans.IsNullOrEmpty()) device.Stock = lastStock;
      //   else device.Stock = lastStock + trans.Sum(x => x.Quantity);

      //   var deviceYearTransactions = year_transactions.Where(x => x.PropertyId == device.Id && x.Quantity > 0);
      //   if (deviceYearTransactions.IsNullOrEmpty()) device.SaveStock = 0;
      //   else device.SaveStock = deviceYearTransactions.Sum(x => x.Quantity);

      //}

      //devices = devices.OrderByDescending(x => x.SaveStock);

      //var model = new PropertysIndexModel(devices.MapViewModelList(_mapper));
      //model.LastClosed = lastClosed!.MapViewModel(_mapper);
      //return model;
   }
   async Task<Category?> ValidateCategoryAsync(int categoryId)
   {
      var category = await _categoryService.GetByIdAsync(categoryId);
      if (category is null)
      {
         ModelState.AddModelError(nameof(Category), ValidationMessages.NotExist(Labels.Category));
      }
      return category!;
   }
}
