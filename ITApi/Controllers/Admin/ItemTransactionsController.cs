using Microsoft.AspNetCore.Mvc;
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
using Infrastructure.Views;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;

namespace ITApi.Controllers.Admin;

public class ItemTransactionsController : BaseAdminController
{
   private readonly ItemTransactionSettings _settings;
   private readonly IItemService _itemService;
   private readonly IItemTransactionService _transactionService;
   private readonly IItemReportService _itemReportService;
   private readonly IMapper _mapper;

   public ItemTransactionsController(IOptions<ItemTransactionSettings> settings,
      IItemService itemService, IItemReportService itemReportService, IItemTransactionService transactionService,
      IMapper mapper)
   {
      _settings = settings.Value;
      _itemService = itemService;
      _itemReportService = itemReportService;
      _transactionService = transactionService; 
      _mapper = mapper;
   }

   async Task<bool> CanRemove(ItemTransaction entity)
   {
      var lastClosed = await _itemReportService.GetLastClosedAsync();
      if (lastClosed!.GetDate().Value.ToEndDate() > entity.Date) return false;
      return true;
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
      year = year.ROCYearToBC();
      DateTime startDate = DateTimeHelpers.GetFirstDayOfMonth(year, month);
      DateTime endDate = DateTimeHelpers.GetLastDayOfMonth(year, month);
      var trans = await _transactionService.FetchAsync(startDate, endDate, includes);

      if (item.HasValue) trans = trans.Where(x => x.ItemId == item.Value);

      return trans.MapViewModelList(_mapper);
   }
   [HttpGet("create")]
   public async Task<ActionResult<ItemTransactionAddForm>> Create(int? item)
   {
      if (item.HasValue)
      {
         var selectedItem = await _itemService.GetByIdAsync(item.Value);
         if (selectedItem == null) return NotFound();


         var form = new ItemTransactionAddForm() { ItemId = item.Value, Quantity = 1, Date = DateTime.Today.ToDateString() };
         return form;
      }

      return new ItemTransactionAddForm() { Quantity = 1, Date = DateTime.Today.ToDateString() };
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

      form.CanRemove = await CanRemove(entity);
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

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _transactionService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      if (!(await CanRemove(entity)))
      {
         ModelState.AddModelError("active", "此紀錄不允許刪除");
         return BadRequest(ModelState);
      }

      await _transactionService.RemoveAsync(entity, User.Id());

      return NoContent();
   }

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
}
