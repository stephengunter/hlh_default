using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Helpers.IT;
using ApplicationCore.Services.IT;
using ITApi.Models;
using Infrastructure.Helpers;
using ApplicationCore.Authorization;

namespace ITApi.Controllers.Admin;

public class ItemReportsController : BaseAdminController
{
   private readonly IItemService _itemService;
   private readonly IItemReportService _reportsService;
   private readonly IItemBalanceSheetService _balanceSheetService;
   private readonly IItemTransactionService _transactionService;
   private readonly IMapper _mapper;

   public ItemReportsController(IItemService itemService, IItemReportService reportsService, IItemBalanceSheetService balanceSheetService,
      IItemTransactionService transactionService, IMapper mapper)
   {
      _itemService = itemService;
      _reportsService = reportsService;
      _balanceSheetService = balanceSheetService;
      _transactionService = transactionService;
      _mapper = mapper;
   }
   
   async Task<ItemReport> GetLatestReportAsync()
   {
      // find the latest month report
      var latest = await _reportsService.GetLatestAsync();
      // if not found, get last yearly closed report 
      if (latest == null) latest = await _reportsService.GetLastClosedAsync();
      return latest!;
   }
   async Task<bool> CanDeleteAsync(ItemReport report)
   {
      var latest = await GetLatestReportAsync();
      return latest.Id == report.Id;
   }

   DateTime GetNextReportDate(ItemReport latest) => new DateTime(latest.Year.ROCYearToBC(), latest.Month + 1, 1);

   [HttpGet("init")]
   public async Task<ActionResult<ItemReportsIndexModel>> Init()
   {
      var years = await _reportsService.FetchYearsAsync();
      var request = new ItemReportsFetchRequest(years.FirstOrDefault());

      return new ItemReportsIndexModel(request, years.ToList());
   }

   [HttpGet]
   public async Task<ActionResult<ICollection<ItemReportViewModel>>> Index(int year)
   {
      var list = await _reportsService.FetchAsync(year); 
      var lastYearClose = list.FirstOrDefault(x => x.Month == 0);

      return list.MapViewModelList(_mapper);
   }
   [HttpGet("{id}")]
   public async Task<ActionResult<ItemReportViewModel>> Details(int id)
   {
      var entity = await _reportsService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      bool canDelete = await CanDeleteAsync(entity);

      var includes = new List<string>() { $"{nameof(Item)}" };
      var sheets = await _balanceSheetService.FetchAsync(entity, includes);
      sheets = sheets.Where(x => x.Item!.Active);
      sheets = sheets.OrderByDescending(x => x.OutQty).ThenByDescending(x => x.InQty); ;
      entity.ItemBalanceSheets = sheets.ToList();

      var model = entity.MapViewModel(_mapper);
      model.CanDelete = canDelete;
      return model;

   }
   [HttpGet("create")]
   public async Task<ActionResult<ICollection<ItemReportForm>>> Create()
   {
      var latest = await GetLatestReportAsync();

      var reportDate = GetNextReportDate(latest!);
      var end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

      var forms = new List<ItemReportForm>();
      while (reportDate < end)
      {
         forms.Add(new ItemReportForm
         {
            Year = reportDate.Year.ToROCYear(),
            Month = reportDate.Month,
         });
         reportDate = reportDate.AddMonths(1);
      }
      return Ok(forms);
   }
   [HttpPost]
   public async Task<ActionResult<int>> Store([FromBody] ItemReportForm form)
   {
      var latest = await GetLatestReportAsync();
      await ValidateRequestAsync(latest, form);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var sheets = await _balanceSheetService.FetchAsync(latest!);

      var items = await _itemService.FetchAsync();

      var report = new ItemReport(form.Year, form.Month);
      //取得自latest以來所有交易紀錄
      var transactions = await _transactionService.FetchAsync(latest!.GetDate()!.Value.AddDays(1));

      foreach (var item in items)
      {
         int lastStock = 0;
         var sh = sheets.FirstOrDefault(x => x.ItemId == item.Id);
         if (sh != null) lastStock = sh.Stock;

         var balanceSheet = item.CreateItemBalanceSheet(report.GetDate()!.Value, lastStock, transactions);
         report.ItemBalanceSheets.Add(balanceSheet);
      }

      await _reportsService.CreateAsync(report, User.Id());
      return Ok(report.Id);
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _reportsService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      bool canDelete = await CanDeleteAsync(entity);

      if (!canDelete)
      {
         ModelState.AddModelError("", "此紀錄不允許刪除");
         return BadRequest(ModelState);
      }

      await _reportsService.DeleteAsync(entity);

      return NoContent();
   }

   async Task ValidateRequestAsync(ItemReport latest, ItemReportForm form)
   {
      var reportDate = GetNextReportDate(latest!);

      var requestDate = new DateTime(form.Year.ROCYearToBC(), form.Month, 1);

      if (requestDate.Year != reportDate.Year)
      {
         ModelState.AddModelError("year", "錯誤的報表年度");
      }
      if (requestDate.Month != reportDate.Month)
      {
         ModelState.AddModelError("month", "錯誤的報表月份");
      }

      if (!ModelState.IsValid) return;

      var exist = await _reportsService.FindByYearMonthAsync(requestDate.Year, reportDate.Month);
      if (exist is not null)
      {
         ModelState.AddModelError("", "指定的年度月份已有報表");
      }
   }
}
