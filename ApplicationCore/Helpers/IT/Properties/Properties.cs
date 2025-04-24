using Infrastructure.Helpers;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using ApplicationCore.Consts.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Views;
using OfficeOpenXml;
using ApplicationCore.Views;
using OfficeOpenXml.Style;
using System.Collections.Generic;

namespace ApplicationCore.Helpers.IT;
public static class PropertyHelpers
{
   public static PropertyViewModel MapViewModel(this Property entity, IMapper mapper)
   {
      var model = mapper.Map<PropertyViewModel>(entity);

      return model;
   }
   public static List<PropertyViewModel> MapViewModelList(this IEnumerable<Property> entitie, IMapper mapper)
      => entitie.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Property, PropertyViewModel> GetPagedList(this IEnumerable<Property> entitie, IMapper mapper, int page = 1, int pageSize = 10)
   {
      var pageList = new PagedList<Property, PropertyViewModel>(entitie, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }
   public static PagedList<SourcePropertyModel> GetPagedList(this IEnumerable<SourcePropertyModel> entitie, int page = 1, int pageSize = 10)
   {
      var pageList = new PagedList<SourcePropertyModel>(entitie, page, pageSize);
      return pageList;
   }

   public static Property MapEntity(this PropertyViewModel model, IMapper mapper, string currentUserId, Property? entity = null)
   {
      if (entity == null) entity = mapper.Map<PropertyViewModel, Property>(model);
      else entity = mapper.Map<PropertyViewModel, Property>(model, entity);

      return entity;
   }

   public static string ToPropNumber(this string num)
      => num.Replace("-", "");
   public static BaseOption<int> ToOption(this Property entity)
      => new BaseOption<int>(entity.Id, entity.Name);

   public static IEnumerable<Property> GetOrdered(this IEnumerable<Property> entitie)
     => entitie.OrderBy(item => item.CategoryId).OrderBy(item => item.LocationId).OrderBy(item => item.Id);

   public static List<PropertiesGroupView> GetPropertiesGroupViews(this IEnumerable<Property> properties)
   {
      var groupedCounts = properties.GroupBy(p => p.CategoryId)
                         .Select(g => new
                         {
                            CategoryId = g.Key,
                            Count = g.Count()
                         })
                        .ToList();
      var list = groupedCounts.Select(x => new PropertiesGroupView
      {
         CategoryId = x.CategoryId.HasValue ? x.CategoryId.Value : null,
         Count = x.Count
      }).ToList();
      return list;
   }
   public static string ToPropertyNumberText(this string? number)
   {
      if (string.IsNullOrEmpty(number)) return "";
      if (number.Length == 18)
      {
         return $"{number[0]}-{number.Substring(1, 2)}-{number.Substring(3, 2)}-{number.Substring(5, 2)}-{number.Substring(7, 4)}-{number.Substring(11, 7)}";
      }
      return "";
   }
   public static string ToPropertyNumberStickText(this string? number)
   {
      if (string.IsNullOrEmpty(number)) return "";
      if (number.Length == 18)
      {
         return $"{number.Substring(0, 7)}-{number.Substring(7, 4)}-{number.Substring(11, 7)}";
      }
      return number;
   }
}

public class PropertyExcelHelpers
{
   public static MemoryStream CreateReportExcel(string title, List<PropertyViewModel> views, List<PropertiesGroupView> groupViews)
   {
      var reportColumns = GetReportColumns();
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var package = new ExcelPackage())
      {
         var worksheet = package.Workbook.Worksheets.Add("Sheet1");
         int cols = reportColumns.Count;
         int rowIndex = 1;
         worksheet.Cells[rowIndex, 1].Value = title;
         worksheet.Cells[rowIndex, 1, rowIndex, cols].Merge = true;
         worksheet.Cells[rowIndex, 1].Style.Font.Size = 14;
         worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
         rowIndex += 1;

         int height = 15;
         for (int i = 0; i < cols; i++)
         {
            worksheet.Column(i + 1).Width = reportColumns[i].Width;
            worksheet.Cells[rowIndex, i + 1].Value = reportColumns[i].Title;
            worksheet.Row(rowIndex).Height = height;
            worksheet.Cells[rowIndex, 1, rowIndex, cols].Style.Font.Size = 10;
            worksheet.Cells[rowIndex, 1, rowIndex, cols].Style.Font.Bold = true;
         }
         rowIndex += 1;
         foreach (var view in views)
         {
            worksheet.Cells[rowIndex, 1].Value = view.PropertyTypeText;
            worksheet.Cells[rowIndex, 2].Value = view.CategoryName;
            worksheet.Cells[rowIndex, 3].Value = view.TitleNameText;
            worksheet.Cells[rowIndex, 4].Value = view.NumberStickText;
            worksheet.Cells[rowIndex, 5].Value = view.BrandName;
            worksheet.Cells[rowIndex, 6].Value = view.Type;
            worksheet.Cells[rowIndex, 7].Value = view.UserName;
            worksheet.Cells[rowIndex, 8].Value = view.LocationName;
            worksheet.Cells[rowIndex, 9].Value = view.DeviceCode;
            worksheet.Cells[rowIndex, 10].Value = view.Ps;
            worksheet.Row(rowIndex).Height = height;
            worksheet.Row(rowIndex).Style.Font.Size = 10;
            rowIndex += 1;
         }

         rowIndex += 1;
         var batches = groupViews.GetInBatches(4);
         foreach (var groupViewsBatch in batches)
         {
            string text = "";
            foreach (var groupView in groupViewsBatch)
            {
               text += $"   {groupView.CategoryName} : {groupView.Count}   ";
            }
            worksheet.Cells[rowIndex, 1].Value = text;
            worksheet.Cells[rowIndex, 1, rowIndex, cols].Merge = true;
            worksheet.Cells[rowIndex, 1].Style.Font.Size = 14;
            worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
            rowIndex += 1;
         }

         using (var range = worksheet.Cells[worksheet.Dimension.Address])
         {
            // Set borders
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Set alignment (center text)
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
         }

         // Step 3: Save the Excel package to a memory stream
         var stream = new MemoryStream();
         package.SaveAs(stream);
         stream.Position = 0; 
         return stream;
      }
   }
   static List<ReportColumn> GetReportColumns()
   {
      var labels = new PropertyLabels();
      int unit = 10;
      var list = new List<ReportColumn>();
      list.Add(new ReportColumn(nameof(labels.PropertyType), labels.PropertyType, 8));
      list.Add(new ReportColumn(nameof(labels.Category), labels.Category, 15));
      list.Add(new ReportColumn(nameof(labels.Title), labels.Title, 25));
      list.Add(new ReportColumn(nameof(labels.Number), labels.Number, 20));
      list.Add(new ReportColumn(nameof(labels.Brand), labels.Brand, 10));
      list.Add(new ReportColumn(nameof(labels.Type), labels.Type, 20));
      list.Add(new ReportColumn(nameof(labels.UserName), labels.UserName, 8));
      list.Add(new ReportColumn(nameof(labels.Location), labels.Location, 12));
      list.Add(new ReportColumn(nameof(labels.DeviceCode), labels.DeviceCode, 8));
      list.Add(new ReportColumn(nameof(labels.Ps), labels.Ps, 8));
      return list;
   }
   public static List<SourcePropertyModel> GetPropertyListFromFile(MemoryStream stream, PropertyType proptype)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var package = new ExcelPackage(stream))
      {
         var worksheet = package.Workbook.Worksheets[0];

         if (worksheet == null) throw new InvalidOperationException("No worksheet found in the Excel file.");
         if (worksheet.Dimension == null) throw new InvalidOperationException("The worksheet is empty.");
         int rowCount = worksheet.Dimension.Rows; // Total rows
         int colCount = worksheet.Dimension.Columns; // Total columns
         var columnMapping = GetColumnMapping(worksheet);

         if (columnMapping.ContainsKey("資訊設備種類")) return GetPropertyListFromITSheet(worksheet, columnMapping);
         return GetPropertyListFromSheet(worksheet, proptype, columnMapping);
      }
   }
   static List<SourcePropertyModel> GetPropertyListFromSheet(ExcelWorksheet worksheet, PropertyType proptype, Dictionary<string, int> columnMapping)
   {
      var list = new List<SourcePropertyModel>();
      int numCol = columnMapping["財產編號"];
      int categoryNameCol = columnMapping["財產名稱"];
      int nameCol = columnMapping["財產別名"];
      int minyearCol = columnMapping["最低使用年限"];
      int buyDateCol = columnMapping["購置日期"];
      int getDateCol = columnMapping["取得日期"];
      int typeCol = columnMapping["型式"];
      int brandCol = columnMapping["廠牌"];
      int usernameCol = columnMapping["保管人(名稱)"];

      int locationCodeCol = columnMapping["存置地點(代碼)"];
      int locationNameCol = columnMapping["存置地點(名稱)"];
      int deprecatedCol = columnMapping["是否減損"];

      int rowCount = worksheet.Dimension.Rows; // Total rows
      for (int row = 2; row <= rowCount; row++)
      {
         string num = worksheet.Cells[row, numCol].Text.Trim(); //3101001-0007-0000001
         string categoryName = worksheet.Cells[row, categoryNameCol].Text.Trim(); //不斷電裝置
         string name = worksheet.Cells[row, nameCol].Text.Trim(); //財產別名
         string minyear = worksheet.Cells[row, minyearCol].Text.Trim(); //最低使用年限
         string buyDate = worksheet.Cells[row, buyDateCol].Text.Trim(); //購置日期
         string getDate = worksheet.Cells[row, getDateCol].Text.Trim(); //取得日期
         string type = worksheet.Cells[row, typeCol].Text.Trim(); //型式
         string brand = worksheet.Cells[row, brandCol].Text.Trim(); //廠牌
                                                                    //string userCode = worksheet.Cells[row, userCodeCol].Text.Trim(); //保管人(代碼)
         string username = worksheet.Cells[row, usernameCol].Text.Trim(); //保管人(名稱)
         string locationCode = worksheet.Cells[row, locationCodeCol].Text.Trim(); //存置地點(代碼)
         string locationName = worksheet.Cells[row, locationNameCol].Text.Trim(); //存置地點(名稱)

         string deprecated = worksheet.Cells[row, deprecatedCol].Text.Trim(); //是否減損

         var item = new SourcePropertyModel()
         {
            PropertyType = proptype,
            Number = num.ToPropNumber(),
            CategoryName = categoryName,
            Name = name,
            MinYears = minyear.Replace("年", "").Trim().ToInt(),
            BuyDate = buyDate.Replace("/", "").Trim().ToInt().RocToDatetime(),
            GetDate = buyDate.Replace("/", "").Trim().ToInt().RocToDatetime(),
            Type = type,
            BrandName = brand,
            UserName = username.Split('（')[0],
            LocationCode = locationCode,
            LocationName = locationName,

            Deprecated = deprecated == "是"
         };
         list.Add(item);
      }
      return list;
   }
   static Dictionary<string, int> GetColumnMapping(ExcelWorksheet worksheet)
   {
      int rowCount = worksheet.Dimension.Rows; // Total rows
      int colCount = worksheet.Dimension.Columns; // Total columns
      var columnMapping = new Dictionary<string, int>();
      for (int col = 1; col <= colCount; col++)
      {
         string header = worksheet.Cells[1, col].Text.Trim();
         if (!string.IsNullOrEmpty(header) && !columnMapping.ContainsKey(header))
         {
            columnMapping[header] = col; // Store column index by name
         }
      }
      return columnMapping;
   }
   static List<SourcePropertyModel> GetPropertyListFromITSheet(ExcelWorksheet worksheet, Dictionary<string, int> columnMapping)
   {
      var list = new List<SourcePropertyModel>();

      int proptypeCol = columnMapping["資訊設備種類"];
      int numCol = columnMapping["財產編號"];
      int categoryNameCol = columnMapping["財產名稱"];
      int titleCol = columnMapping["設備名稱"];
      int nameCol = columnMapping["財產別名"];
      int minyearCol = columnMapping["最低使用年限"];
      int buyDateCol = columnMapping["購置日期"];
      int getDateCol = columnMapping["取得日期"];
      int typeCol = columnMapping["型式"];
      int brandCol = columnMapping["廠牌"];

      //int userCodeCol = columnMapping["保管人(代碼)"];
      int usernameCol = columnMapping["保管人(名稱)"];

      int locationCodeCol = columnMapping["存置地點(代碼)"];
      int locationNameCol = columnMapping["存置地點(名稱)"];

      int downdateCol = columnMapping["已下架日期"];
      int deprecatedCol = columnMapping["是否減損"];

      int rowCount = worksheet.Dimension.Rows; // Total rows
      for (int row = 2; row <= rowCount; row++)
      {
         string proptype = worksheet.Cells[row, proptypeCol].Text.Trim(); //物品
         string num = worksheet.Cells[row, numCol].Text.Trim(); //3101001-0007-0000001
         string categoryName = worksheet.Cells[row, categoryNameCol].Text.Trim(); //不斷電裝置
         string title = worksheet.Cells[row, titleCol].Text.Trim(); //設備名稱
         string name = worksheet.Cells[row, nameCol].Text.Trim(); //財產別名
         string minyear = worksheet.Cells[row, minyearCol].Text.Trim(); //最低使用年限
         string buyDate = worksheet.Cells[row, buyDateCol].Text.Trim(); //購置日期
         string getDate = worksheet.Cells[row, getDateCol].Text.Trim(); //取得日期
         string type = worksheet.Cells[row, typeCol].Text.Trim(); //型式
         string brand = worksheet.Cells[row, brandCol].Text.Trim(); //廠牌
                                                                    //string userCode = worksheet.Cells[row, userCodeCol].Text.Trim(); //保管人(代碼)
         string username = worksheet.Cells[row, usernameCol].Text.Trim(); //保管人(名稱)
         string locationCode = worksheet.Cells[row, locationCodeCol].Text.Trim(); //存置地點(代碼)
         string locationName = worksheet.Cells[row, locationNameCol].Text.Trim(); //存置地點(名稱)
         string downdate = worksheet.Cells[row, downdateCol].Text.Trim(); //已下架日期
         string deprecated = worksheet.Cells[row, deprecatedCol].Text.Trim(); //是否減損

         var item = new SourcePropertyModel()
         {
            PropertyType = proptype == "物品" ? PropertyType.Item : PropertyType.Property,
            Number = num.ToPropNumber(),
            CategoryName = categoryName,
            Title = title,
            Name = name,
            MinYears = minyear.Replace("年", "").Trim().ToInt(),
            BuyDate = buyDate.Replace("/", "").Trim().ToInt().RocToDatetime(),
            GetDate = buyDate.Replace("/", "").Trim().ToInt().RocToDatetime(),
            Type = type,
            BrandName = brand,
            //UserCode = userCode,
            UserName = username.Split('（')[0],
            LocationCode = locationCode,
            LocationName = locationName,
            DownDate = downdate.Replace("/", "").Trim().ToInt().RocToDatetime(),
            Deprecated = deprecated == "是"
         };
         list.Add(item);
      }
      return list;
   }
   public static List<SourcePropertyModel> GetPropertyListFromITFile(MemoryStream stream)
   {
      //資訊設備輸出Excel
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      var list = new List<SourcePropertyModel>();
      using (var package = new ExcelPackage(stream))
      {
         var worksheet = package.Workbook.Worksheets[0];
         if (worksheet == null) throw new InvalidOperationException("No worksheet found in the Excel file.");
         if (worksheet.Dimension == null) throw new InvalidOperationException("The worksheet is empty.");
         int rowCount = worksheet.Dimension.Rows; // Total rows
         int colCount = worksheet.Dimension.Columns; // Total columns
         var columnMapping = GetColumnMapping(worksheet); // new Dictionary<string, int>();
         //for (int col = 1; col <= colCount; col++)
         //{
         //   string header = worksheet.Cells[1, col].Text.Trim();
         //   if (!string.IsNullOrEmpty(header) && !columnMapping.ContainsKey(header))
         //   {
         //      columnMapping[header] = col; // Store column index by name
         //   }
         //}
         int proptypeCol = columnMapping["資訊設備種類"];
         int numCol = columnMapping["財產編號"];
         int categoryNameCol = columnMapping["財產名稱"];
         int titleCol = columnMapping["設備名稱"];
         int nameCol = columnMapping["財產別名"];
         int minyearCol = columnMapping["最低使用年限"];
         int buyDateCol = columnMapping["購置日期"];
         int getDateCol = columnMapping["取得日期"];
         int typeCol = columnMapping["型式"];
         int brandCol = columnMapping["廠牌"];

         //int userCodeCol = columnMapping["保管人(代碼)"];
         int usernameCol = columnMapping["保管人(名稱)"];

         int locationCodeCol = columnMapping["存置地點(代碼)"];
         int locationNameCol = columnMapping["存置地點(名稱)"];

         int downdateCol = columnMapping["已下架日期"];
         int deprecatedCol = columnMapping["是否減損"];

         for (int row = 2; row <= rowCount; row++)
         {
            string proptype = worksheet.Cells[row, proptypeCol].Text.Trim(); //物品
            string num = worksheet.Cells[row, numCol].Text.Trim(); //3101001-0007-0000001
            string categoryName = worksheet.Cells[row, categoryNameCol].Text.Trim(); //不斷電裝置
            string title = worksheet.Cells[row, titleCol].Text.Trim(); //設備名稱
            string name = worksheet.Cells[row, nameCol].Text.Trim(); //財產別名
            string minyear = worksheet.Cells[row, minyearCol].Text.Trim(); //最低使用年限
            string buyDate = worksheet.Cells[row, buyDateCol].Text.Trim(); //購置日期
            string getDate = worksheet.Cells[row, getDateCol].Text.Trim(); //取得日期
            string type = worksheet.Cells[row, typeCol].Text.Trim(); //型式
            string brand = worksheet.Cells[row, brandCol].Text.Trim(); //廠牌
            //string userCode = worksheet.Cells[row, userCodeCol].Text.Trim(); //保管人(代碼)
            string username = worksheet.Cells[row, usernameCol].Text.Trim(); //保管人(名稱)
            string locationCode = worksheet.Cells[row, locationCodeCol].Text.Trim(); //存置地點(代碼)
            string locationName = worksheet.Cells[row, locationNameCol].Text.Trim(); //存置地點(名稱)
            string downdate = worksheet.Cells[row, downdateCol].Text.Trim(); //已下架日期
            string deprecated = worksheet.Cells[row, deprecatedCol].Text.Trim(); //是否減損

            var item = new SourcePropertyModel()
            {
               PropertyType = proptype == "物品" ? PropertyType.Item : PropertyType.Property,
               Number = num.ToPropNumber(),
               CategoryName = categoryName,
               Title = title,
               Name = name,
               MinYears = minyear.Replace("年", "").Trim().ToInt(),
               BuyDate = buyDate.Replace("/", "").Trim().ToInt().RocToDatetime(),
               GetDate = buyDate.Replace("/", "").Trim().ToInt().RocToDatetime(),
               Type = type,
               BrandName = brand,
               //UserCode = userCode,
               UserName = username.Split('（')[0],
               LocationCode = locationCode,
               LocationName = locationName,
               DownDate = downdate.Replace("/", "").Trim().ToInt().RocToDatetime(),
               Deprecated = deprecated == "是"
            };
            list.Add(item);
         }
      }
      return list;
   }
}
