using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Helpers.IT;
using ApplicationCore.Services.IT;
using ITApi.Models;

namespace ITApi.Controllers.Admin;

public class ItemReportsController : BaseAdminController
{
   private readonly IItemReportService _reportsService;
   private readonly IItemBalanceSheetService _balanceSheetService;
   private readonly IMapper _mapper;

   public ItemReportsController(IItemReportService reportsService, IItemBalanceSheetService balanceSheetService, IMapper mapper)
   {
      _reportsService = reportsService;
      _balanceSheetService = balanceSheetService;
      _mapper = mapper;
   }
   
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

      var includes = new List<string>() { $"{nameof(Item)}" };
      var sheets = await _balanceSheetService.FetchAsync(entity, includes);
      entity.ItemBalanceSheets = sheets.ToList();
      
      return entity.MapViewModel(_mapper);

   }
}
