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

public class DevicesController : BaseAdminController
{
   private readonly IDeviceService _deviceService;
   private readonly ICategoryService _categoryService;
   private readonly IMapper _mapper;
   public DevicesController(IDeviceService deviceService, ICategoryService categoryService, IMapper mapper)
   {
      _deviceService = deviceService;
      _categoryService = categoryService;
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
      var categories = await _categoryService.FetchAsync(nameof(Device));
      categories = categories.GetOrdered();
      var categoryIds = categories.Select(c => c.Id).ToList();
      var root = categories.FirstOrDefault(x => x.IsRootItem);
      root!.Title = "所有設備分類";
      root!.LoadSubItems(categories);
      
      bool fired = false;
      int? category = null;
      var model = new DevicesAdminModel(new DevicesFetchRequest(fired, category), root.MapViewModel(_mapper));

      model.Categories = categories.MapViewModelList(_mapper);

      return model;
   }

   [HttpGet]
   public async Task<ActionResult<PagedList<Device, DeviceViewModel>>> Index(bool fired, int? category, int page = 1, int pageSize = 10)
   {
      var includes = new List<string>();
      IEnumerable<Device> devices;
      
      Category selectedCategory = null;
      if (category.HasValue)
      {
         selectedCategory = await ValidateCategoryAsync(category.Value, subItems: true);
         if (!ModelState.IsValid) return BadRequest(ModelState);
      }
      if (selectedCategory is null)
      {
         devices = await _deviceService.FetchNoneCategoryEntitiesAsync(fired, includes);
      }
      else
      {
         var categories = new List<Category>() { selectedCategory };
         if(selectedCategory.SubItems!.HasItems()) categories.AddRange(selectedCategory.GetAllSubItems());

         devices = await _deviceService.FetchAsync(fired, categories, includes);
      }
     
      var model = new PagedList<Device, DeviceViewModel>(devices, page, pageSize);

      model.SetViewList(model.List.MapViewModelList(_mapper));
      return model;
      //if (!active) return new DevicesIndexModel(devices.MapViewModelList(_mapper));

      //var lastClosed = await _deviceReportService.GetLastClosedAsync();
      //var sheets = await _deviceBalanceSheetService.FetchAsync(lastClosed!);

      //var date = lastClosed!.GetDate();
      //var transactions = await _transactionService.FetchAsync(date!.Value.AddDays(1));
      //var year_transactions = await _transactionService.FetchAsync(DateTime.Today.AddYears(-1));
      //foreach (var device in devices)
      //{
      //   int lastStock = 0;
      //   var sh = sheets.FirstOrDefault(x => x.DeviceId == device.Id);
      //   if (sh != null) lastStock = sh.Stock;

      //   var trans = transactions.Where(x => x.DeviceId == device.Id);
      //   if (trans.IsNullOrEmpty()) device.Stock = lastStock;
      //   else device.Stock = lastStock + trans.Sum(x => x.Quantity);

      //   var deviceYearTransactions = year_transactions.Where(x => x.DeviceId == device.Id && x.Quantity > 0);
      //   if (deviceYearTransactions.IsNullOrEmpty()) device.SaveStock = 0;
      //   else device.SaveStock = deviceYearTransactions.Sum(x => x.Quantity);

      //}

      //devices = devices.OrderByDescending(x => x.SaveStock);

      //var model = new DevicesIndexModel(devices.MapViewModelList(_mapper));
      //model.LastClosed = lastClosed!.MapViewModel(_mapper);
      //return model;
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
