using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using ApplicationCore.Authorization;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using Ardalis.Specification;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Services.IT;
using ITApi.Models;
using Azure.Core;
using ApplicationCore.Models.Identity;
using ApplicationCore.Services.Identity;
using ApplicationCore.Views.Identity;
using Infrastructure.Paging;
using System.Drawing.Printing;
using Infrastructure.Views;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ITApi.Controllers.Admin;

public class ItemTransactionsController : BaseAdminController
{
   private readonly ItemTransactionSettings _settings;
   private readonly IItemService _itemService;
   private readonly IItemTransactionService _transactionService;
   private readonly IMapper _mapper;

   public ItemTransactionsController(IOptions<ItemTransactionSettings> settings,
      IItemService itemService, IItemTransactionService transactionService,
      IMapper mapper)
   {
      _settings = settings.Value;
      _itemService = itemService;
      _transactionService = transactionService; 
      _mapper = mapper;
   }
   [HttpGet("init")]
   public async Task<ActionResult<ItemTransactionsIndexModel>> Init()
   {
      var date = DateTime.Today;
      var request = new ItemTransactionsFetchRequest(date.Year.ToROCYear(), date.Month);
      var model = new ItemTransactionsIndexModel(request);

      var minYear = _settings.MinYear;
      var yearOptions = new List<BaseOption<int>>();
      var years = new List<int>();
      for (int i = minYear; i <= date.Year.ToROCYear(); i++)
      {
         yearOptions.Add(new BaseOption<int>(i, i.ToString()));
      }
      model.YearOptions = yearOptions;

      var items = await _itemService.FetchAsync();
      items = items.Where(x => x.Active).ToList();
      model.ItemOptions = items.Select(item => item.ToOption()).ToList();

      return model;
   }
   [HttpGet]   
   public async Task<ActionResult<ICollection<ItemTransactionViewModel>>> Index(int year, int month, int? item)
   {
      var includes = new List<string>() { nameof(Item) };
      var trans = await _transactionService.FetchAsync(year.ROCYearToBC(), month, includes);

      if (item.HasValue) trans = trans.Where(x => x.ItemId == item.Value);

      return trans.MapViewModelList(_mapper);
   }
   [HttpGet("create")]
   public async Task<ActionResult<ItemTransactionAddForm>> Create(int item)
   {
      var selectedItem = await _itemService.GetByIdAsync(item);
      if (selectedItem == null) return NotFound();

      
      var form = new ItemTransactionAddForm() { ItemId = item, Quantity = 1, Date = DateTime.Today.ToDateString() };
      return form;
   }

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] ItemTransactionAddForm form)
   {
      ValidateRequest(form);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new ItemTransaction();
      form.SetValuesTo(entity);

      var date = form.Date.ToDatetimeOrNull();
      entity.Date = date!.Value;

      if (!form.In)
      {
         entity.Quantity = 0 - form.Quantity;
      }


      await _transactionService.CreateAsync(entity, User.Id());

      return Ok();
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<ItemTransactionEditForm>> Edit(int id)
   {
      var entity = await _transactionService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var form = new ItemTransactionEditForm();
      entity.SetValuesTo(form);

      form.Date = entity.Date.ToDateString();

      form.In = entity.Quantity > 0;
      if (!form.In) form.Quantity = 0 - entity.Quantity;

      return form;
   }
   [HttpPut("{id}")]
   public async Task<IActionResult> Update(int id, [FromBody] ItemTransactionEditForm form)
   {
      var entity = await _transactionService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      ValidateRequest(form, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      
      form.SetValuesTo(entity);

      var date = form.Date.ToDatetimeOrNull();
      entity.Date = date!.Value;

      if (!form.In)
      {
         entity.Quantity = 0 - form.Quantity;
      }

      await _transactionService.UpdateAsync(entity, User.Id());
      return NoContent();
   }
   //[HttpPut("reset-client-secret/{id}")]
   //public async Task<IActionResult> ResetClientSecret(int id)
   //{
   //   var app = await _appService.GetByIdAsync(id);
   //   if (app == null) return NotFound();

   //   if (!app.Type.EqualTo(AppTypes.Api))
   //   {
   //      ModelState.AddModelError("type", "AppType Not Valid.");
   //      return BadRequest(ModelState);
   //   }

   //   await _appService.ResetClientSecretAsync(app);
   //   return NoContent();
   //}
   //[HttpDelete("{id}")]
   //public async Task<IActionResult> Remove(int id)
   //{
   //   var app = await _appService.GetByIdAsync(id);
   //   if (app == null) return NotFound();

   //   await _appService.RemoveAsync(app, User.Id());

   //   return NoContent();
   //}

   void ValidateRequest(BaseItemTransactionForm model, int id = 0)
   {
      var labels = new ItemTransactionLabels();
      var date = model.Date.ToDatetimeOrNull();
      if (date is null) ModelState.AddModelError(nameof(model.Date), ValidationMessages.Required(labels.Date));

      if (model.ItemId < 1)
      {
         ModelState.AddModelError(nameof(model.ItemId), ValidationMessages.Required(labels.Item));
      }
      if (model.Quantity == 0)
      {
         ModelState.AddModelError(nameof(model.Quantity), ValidationMessages.Required(labels.Quantity));
      }

      if (model.In) return;

      if (model.UserId is null)
      {
         ModelState.AddModelError(nameof(model.UserId), ValidationMessages.Required(labels.UserId));
      }
      if (model.DepartmentId is null)
      {
         ModelState.AddModelError(nameof(model.DepartmentId), ValidationMessages.Required(labels.DepartmentId));
      }
   }

   //void ValidateType(string type)
   //{
   //   if (string.IsNullOrEmpty(type))
   //   {
   //      ModelState.AddModelError("type", ValidationMessages.Required(type));
   //   }

   //   if (type.EqualTo(AppTypes.Spa) || type.EqualTo(AppTypes.Api)) return;
   //   ModelState.AddModelError("type", ValidationMessages.NotExist("type"));

   //}
   //void ValidateUrl(BaseAppForm model)
   //{
   //   if (string.IsNullOrEmpty(model.Url))
   //   {
   //      ModelState.AddModelError(nameof(model.Url), ValidationMessages.Required("Url"));
   //   }
   //   if (!model.Url!.IsValidUrl())
   //   {
   //      ModelState.AddModelError(nameof(model.Url), ValidationMessages.WrongFormatOf("Url"));
   //   }

   //}
}
