using Infrastructure.Helpers;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using ApplicationCore.Models.Identity;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Views;
using OfficeOpenXml;
using ApplicationCore.Migrations;
using Infrastructure.Interfaces;
using ApplicationCore.DataAccess;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

   public static PagedList<Property, PropertyViewModel> GetPagedList(this IEnumerable<Property> entitie, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Property, PropertyViewModel>(entitie, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

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
     => entitie.OrderBy(item => item.Id);
}

public class PropertyExcelHelpers
{
   public static List<SourcePropertyModel> GetPropertyListFromFile(string filePath)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      var list = new List<SourcePropertyModel>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[0];
         if (worksheet == null) throw new InvalidOperationException("No worksheet found in the Excel file.");
         if (worksheet.Dimension == null) throw new InvalidOperationException("The worksheet is empty.");
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
