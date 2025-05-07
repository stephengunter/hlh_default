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
using ApplicationCore.Views.IT;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ITApi.Controllers.Admin;

public class ItemsController : BaseAdminController
{
   private readonly IItemService _itemService;
   private readonly IItemTransactionService _transactionService;
   private readonly IItemReportService _itemReportService;
   private readonly IItemBalanceSheetService _itemBalanceSheetService;
   private readonly IMapper _mapper;
   private readonly int DAYS_OUTCOUNT = 180;

   public ItemsController(IItemService itemService, IDepartmentsService departmentsService,
      IItemTransactionService transactionService, IItemReportService itemReportService,
      IItemBalanceSheetService itemBalanceSheetService, IMapper mapper)
   {
      _itemService = itemService;
      _transactionService = transactionService;
      _itemReportService = itemReportService;
      _itemBalanceSheetService = itemBalanceSheetService;
      _mapper = mapper;
   }

   bool CanRemove(Item entity)
   {
      if (entity.Active) return false;
      return true;
   }

   [HttpGet("init")]
   public async Task<ActionResult<ItemsAdminModel>> Init()
   {
      var model = new ItemsAdminModel(new ItemsFetchRequest(true));

      var items = await _itemService.FetchAsync();
      items = items.Where(x => x.Active).ToList();
      model.ItemOptions = items.Select(item => item.ToOption()).ToList();

      return model;
   }
   async Task<ItemReport> GetLatestReportAsync()
   {
      // find the latest month report
      var latest = await _itemReportService.GetLatestAsync();
      // if not found, get last yearly closed report 
      if (latest == null) latest = await _itemReportService.GetLastClosedAsync();
      return latest!;
   }
   [HttpGet]   
   public async Task<ActionResult<ItemsIndexModel>> Index(bool active)
   {
      var items = await _itemService.FetchAsync();
      items = items.Where(x => x.Active == active);

      if (!active) return new ItemsIndexModel(items.MapViewModelList(_mapper));

      //取得最近一次結算庫存
      var latestReport = await GetLatestReportAsync();
      var sheets = await _itemBalanceSheetService.FetchAsync(latestReport!);

      //取得自最近一次結算庫存以來所有交易紀錄
      var transactions = await _transactionService.FetchAsync(latestReport!.GetDate()!.Value.AddDays(1));
      foreach (var item in items)
      {
         int lastStock = 0;
         var sh = sheets.FirstOrDefault(x => x.ItemId == item.Id);
         if (sh != null) lastStock = sh.Stock;

         var trans = transactions.Where(x => x.ItemId == item.Id);
         if (trans.IsNullOrEmpty()) item.Stock = lastStock;
         else item.Stock = lastStock + trans.Sum(x => x.Quantity);
      }

      items = items.OrderByDescending(x => x.SaveStock);
      var views = items.MapViewModelList(_mapper);
      await LoadOutCountAsync(views);
      var model = new ItemsIndexModel(views);
      model.LastReport = latestReport!.MapViewModel(_mapper);
      return model;
   }
   async Task LoadOutCountAsync(ICollection<ItemViewModel> itemViews)
   {
      var date = DateTime.Today.AddDays(0 - DAYS_OUTCOUNT);
      //取得近 DAYS_OUTCOUNT 天以來所有交易紀錄
      var transactions = await _transactionService.FetchAsync(date);
      foreach (var itemView in itemViews)
      { 
         var outTrans = transactions.Where(x => x.ItemId == itemView.Id && x.Quantity < 0);
         itemView.OutCount = Math.Abs(outTrans.Sum(x => x.Quantity));
      }
   }


   [HttpGet("create")]
   public ActionResult<ItemAddForm> Create()
      => new ItemAddForm();

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] ItemAddForm form)
   {
      await ValidateRequestAsync(form);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new Item();
      form.SetValuesTo(entity);
      entity.SetActive(form.Active);

      await _itemService.CreateAsync(entity, User.Id());
      return Ok();
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<ItemEditForm>> Edit(int id)
   {
      var entity = await _itemService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var form = new ItemEditForm();
      entity.SetValuesTo(form);
      form.Active = entity.Active;

      form.CanRemove = CanRemove(entity);
      return form;
   }
   [HttpPut("{id}")]
   public async Task<IActionResult> Update(int id, [FromBody] ItemEditForm form)
   {
      var entity = await _itemService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(form, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      form.SetValuesTo(entity);
      entity.SetActive(form.Active);

      await _itemService.UpdateAsync(entity, User.Id());
      return NoContent();
   }
   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _itemService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      if (!CanRemove(entity))
      {
         ModelState.AddModelError("active", "此紀錄不允許刪除");
         return BadRequest(ModelState);
      }

      await _itemService.RemoveAsync(entity, User.Id());

      return NoContent();
   }

   async Task ValidateRequestAsync(BaseItemForm model, int id = 0)
   {
      var labels = new ItemLabels();
      if (string.IsNullOrEmpty(model.Name))
      {
         ModelState.AddModelError(nameof(model.Name), ValidationMessages.Required(labels.Name));
      }
      if (string.IsNullOrEmpty(model.Code))
      {
         ModelState.AddModelError(nameof(model.Code), ValidationMessages.Required(labels.Code));
      }

      if (!ModelState.IsValid) return;
      if (id == 0) return;

      var exist = await _itemService.FindByCodeAsync(model.Code);
      if (exist is not null && exist.Id != id)
      {
         ModelState.AddModelError(nameof(model.Code), ValidationMessages.Duplicate(labels.Code));
      }
   }
}
