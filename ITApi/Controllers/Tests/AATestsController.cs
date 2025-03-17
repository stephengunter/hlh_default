using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Web.Controllers;
using AutoMapper;
using ApplicationCore.Models.IT;
using ApplicationCore.DataAccess;
using ApplicationCore.Services.IT;
using ApplicationCore.Helpers.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;
using OfficeOpenXml;
using Infrastructure.Helpers;
using ApplicationCore.Services.Identity;
using ApplicationCore.Services;

namespace ITApi.Controllers.Tests;

public class AATestsController : BaseTestController
{
   private readonly IAppUsersService _usersService;
   private readonly IDepartmentsService _departmentsService;
   private readonly IItemReportService _itemReportService;
   private readonly IItemService _itemService;
   private readonly IItemTransactionService _transactionService;
   private readonly IMapper _mapper;
   public AATestsController(IAppUsersService usersService, IDepartmentsService departmentsService, IItemReportService itemReportService, IItemService itemService,
      IItemTransactionService transactionService, IMapper mapper)
   {
      _usersService = usersService;
      _departmentsService = departmentsService;
      _itemReportService = itemReportService;
      _itemService = itemService;
      _transactionService = transactionService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      return Ok();
   }

   //[HttpGet]
   //public async Task<ActionResult> Index()
   //{
   //   var departments = await _departmentsService.FetchRootsAsync();
   //   var items = await _itemService.FetchAsync();
   //   string filePath = @"C:\temp\20250317\trans.xlsx";
   //   ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
   //   var list = new List<ItemTransaction>();
   //   using (var package = new ExcelPackage(new FileInfo(filePath)))
   //   {
   //      var worksheet = package.Workbook.Worksheets[0];
   //      if (worksheet == null)
   //      {
   //         throw new InvalidOperationException("No worksheet found in the Excel file.");
   //      }
   //      int rowCount = worksheet.Dimension.Rows; // Total rows
   //      for (int row = 2; row <= rowCount; row++)
   //      {
   //         string itemCode = worksheet.Cells[row, 2].Text.Trim();
   //         var item = items.FirstOrDefault(x => x.Code == itemCode);
   //         if (item == null) continue;

   //         string dep = worksheet.Cells[row, 6].Text.Trim();
   //         var department = departments.FirstOrDefault(x => x.Title == dep);

   //         bool isIn = worksheet.Cells[row, 4].Text.Trim() == "¤J®w";
   //         int qty = worksheet.Cells[row, 5].Text.Trim().ToInt();
   //         var tran = new ItemTransaction()
   //         {
   //            Date = worksheet.Cells[row, 1].Text.Trim().ToDatetimeOrNull().Value,
   //            ItemId = item.Id,
   //            DepartmentId = department.Id,
   //            UpdatedBy = worksheet.Cells[row, 2].Text.Trim(),
   //            CreatedBy = worksheet.Cells[row, 6].Text.Trim(),
   //            Quantity = isIn ? qty : (0 - qty),
   //            UserName = worksheet.Cells[row, 7].Text.Trim(),
   //            Ps = worksheet.Cells[row, 8].Text.Trim(),

   //         };
   //         list.Add(tran);
   //      }
   //   }
      
   //   list = list.Where(x => x.Date > new DateTime(2024, 12, 18)).ToList();
   //   await _transactionService.AddRangeAsync(list);
   //   return Ok(list);
   //}
}
