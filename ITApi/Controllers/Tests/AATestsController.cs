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
   private readonly IDeviceService _deviceService;
   private readonly IMapper _mapper;
   public AATestsController(DefaultContext context, IDeviceService deviceService, IMapper mapper)
   {
      _context = context;
      _deviceService = deviceService;
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
      string test = "110314010100070000070";
      if (test.Length == 21) test = test.Substring(3);
      return Ok(test);
   }
   //int AddTitle(ExcelWorksheet worksheet, int rowIndex, string title)
   //{
   //   worksheet.Cells[rowIndex, 1].Value = title;
   //   worksheet.Cells[rowIndex, 1, rowIndex, Keys.Count].Merge = true;
   //   worksheet.Cells[rowIndex, 1].Style.Font.Size = 18;
   //   worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
   //   worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
   //   worksheet.Cells[rowIndex, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

   //   //Set row bgcolor
   //   //codes goes here
   //   // Set row background color
   //   worksheet.Cells[rowIndex, 1, rowIndex, Keys.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
   //   worksheet.Cells[rowIndex, 1, rowIndex, Keys.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Use your desired color


   //   worksheet.Row(rowIndex).Height = 30;

   //   rowIndex += 1;
   //   return rowIndex;
   //}

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
