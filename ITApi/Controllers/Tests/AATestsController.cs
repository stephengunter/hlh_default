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
using ApplicationCore.Models.Identity;
using ApplicationCore.Migrations;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.Helpers.IT;
using Polly;
using ApplicationCore.Specifications.IT;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Ardalis.Specification;

namespace ITApi.Controllers.Tests;

public class AATestsController : BaseTestController
{
   
   private readonly DefaultContext _context;
   private readonly IPropertyService _propertyService;
   private readonly IMapper _mapper;
   public AATestsController(DefaultContext context, IPropertyService propertyService, IMapper mapper)
   {
      _context = context;
      _propertyService = propertyService;
      _mapper = mapper;
   }
   [HttpGet("{input}")]
   public async Task<ActionResult> Index(string input)
   {
      return Ok(input.ToPropNumber());
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      
      await _propertyService.RefreshAsync();
      //string filePath = @"C:\temp\20250321\資訊財產物品帳.xlsx";
      //var src = PropertyExcelHelpers.GetPropertyListFromFile(filePath);
      //await _propertyService.SyncAsync(src);
      //var categories = _context.Categories.Where(x => x.EntityType == nameof(Device)).ToList();
      //foreach (var prop in prop_list)
      //{
      //   var category = categories.FirstOrDefault(x => x.Title == prop.CategoryName);
      //   if (category is null) continue;

      //   prop.CategoryId = category.Id;
      //}

      //_context.SaveChanges();
      //  bool hasDuplicates = _context.Properties.GroupBy(p => p.Number).Any(g => g.Count() > 1);
      //  var duplicateNumbers = _context.Properties
      //.GroupBy(p => p.Number)
      //.Where(g => g.Count() > 1)
      //.Select(g => g.Key)
      //.ToList();



      return Ok();
   }

   void test()
   {
      //var devices = _context.Devices.ToList();
      //var categories = _context.Categories.ToList();
      //foreach (var device in devices)
      //{
      //   string? num = device.No;
      //   if (string.IsNullOrEmpty(num)) continue;
      //   if (num.Length != 5) continue;
      //   string code = num.Substring(0, 2);
      //   if (code != "NB") continue;
      //   var category = categories.FirstOrDefault(x => x.Key == code);
      //   if (category is null) continue;
      //   device.CategoryId = category.Id;
      //}
      //_context.SaveChanges();
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

   //         bool isIn = worksheet.Cells[row, 4].Text.Trim() == "入庫";
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
