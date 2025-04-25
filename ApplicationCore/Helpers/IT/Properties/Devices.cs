using Infrastructure.Helpers;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Views;
using Microsoft.Data.SqlClient;
using ApplicationCore.DataAccess;
using ApplicationCore.Models.Identity;
using ApplicationCore.Migrations;
using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace ApplicationCore.Helpers.IT;

public static class DeviceHelpers
{
   public static DeviceViewModel MapViewModel(this Device entity, IMapper mapper)
   {
      var model = mapper.Map<DeviceViewModel>(entity);

      return model;
   }
   public static List<DeviceViewModel> MapViewModelList(this IEnumerable<Device> entitie, IMapper mapper)
      => entitie.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Device, DeviceViewModel> GetPagedList(this IEnumerable<Device> entitie, IMapper mapper, int page, int pageSize, bool allItems = false)
   {
      var pageList = new PagedList<Device, DeviceViewModel>(entitie, page, pageSize, allItems);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Device MapEntity(this DeviceViewModel model, IMapper mapper, string currentUserId, Device? entity = null)
   {
      if (entity == null) entity = mapper.Map<DeviceViewModel, Device>(model);
      else entity = mapper.Map<DeviceViewModel, Device>(model, entity);

      return entity;
   }
   public static List<DevicesGroupView> GetPropertiesGroupViews(this IEnumerable<Device> devices)
   {
      var groupedCounts = devices.GroupBy(p => p.CategoryId)
                         .Select(g => new
                         {
                            CategoryId = g.Key,
                            Count = g.Count()
                         })
                        .ToList();
      var list = groupedCounts.Select(x => new DevicesGroupView
      {
         CategoryId = x.CategoryId.HasValue ? x.CategoryId.Value : null,
         Count = x.Count
      }).ToList();
      return list;
   }

   public static BaseOption<int> ToOption(this Device entity)
      => new BaseOption<int>(entity.Id, entity.Title!);

   public static IEnumerable<Device> GetOrdered(this IEnumerable<Device> entitie)
     => entitie.OrderBy(item => item.Id);
}

public class DeviceExcelHelpers
{
   public static List<Device> GetDeviceListFromFile(MemoryStream stream)
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

         return GetDeviceListFromSheet(worksheet, columnMapping);
      }
   }

   static List<Device> GetDeviceListFromSheet(ExcelWorksheet worksheet, Dictionary<string, int> columnMapping)
   {
      var list = new List<Device>();

      int no1Col = columnMapping["no1"];
      int scriptCol = columnMapping["script"];
      int de_noCol = columnMapping["de_no"];
      int de_kindCol = columnMapping["de_kind"];
      int de_hlhnoCol = columnMapping["de_hlhno"];
      int roomCol = columnMapping["room"];
      int usernameCol = columnMapping["username"];
      int de_name1Col = columnMapping["de_name1"];
      int de_name2Col = columnMapping["de_name2"];
      int aliasCol = columnMapping["alias"];
      int space_aCol = columnMapping["space_a"];
      int stateCol = columnMapping["state"];
      int money_kindCol = columnMapping["money_kind"];
      int fac_aCol = columnMapping["fac_a"];
      int memo_aCol = columnMapping["memo_a"];
      int ip_addressCol = columnMapping["ip_address"];
      int work_aCol = columnMapping["work_a"];
      int work_mCol = columnMapping["work_m"];
      int date_sCol = columnMapping["date_s"];
      int firedCol = columnMapping["fired"];
      int date_oCol = columnMapping["date_o"];

      int rowCount = worksheet.Dimension.Rows; // Total rows
      for (int row = 2; row <= rowCount; row++)
      {
         string no1 = worksheet.Cells[row, no1Col].Text.Trim();
         string script = worksheet.Cells[row, scriptCol].Text.Trim();
         string de_no = worksheet.Cells[row, de_noCol].Text.Trim();
         string de_kind = worksheet.Cells[row, de_kindCol].Text.Trim();
         string de_hlhno = worksheet.Cells[row, de_hlhnoCol].Text.Trim();
         string room = worksheet.Cells[row, roomCol].Text.Trim();
         string username = worksheet.Cells[row, usernameCol].Text.Trim();
         string de_name1 = worksheet.Cells[row, de_name1Col].Text.Trim();
         string de_name2 = worksheet.Cells[row, de_name2Col].Text.Trim();                                                                  
         string alias = worksheet.Cells[row, aliasCol].Text.Trim();
         string space_a = worksheet.Cells[row, space_aCol].Text.Trim();
         string state = worksheet.Cells[row, stateCol].Text.Trim();
         string money_kind = worksheet.Cells[row, money_kindCol].Text.Trim();
         string fac_a = worksheet.Cells[row, fac_aCol].Text.Trim();
         string memo_a = worksheet.Cells[row, memo_aCol].Text.Trim();
         string ip_address = worksheet.Cells[row, ip_addressCol].Text.Trim();
         string work_a = worksheet.Cells[row, work_aCol].Text.Trim();
         string work_m = worksheet.Cells[row, work_mCol].Text.Trim();
         string date_s = worksheet.Cells[row, date_sCol].Text.Trim();
         string fired = worksheet.Cells[row, firedCol].Text.Trim();
         string date_o = worksheet.Cells[row, date_oCol].Text.Trim();

         var device = new Device()
         {
            OldId = Convert.ToInt32(no1),
            Title = script,
            No = de_no,
            Kind = de_kind,
            PropNum = de_hlhno,
            Room = room,
            UserName = username,
            BrandName = de_name1,
            Type = de_name2,
            Alias = alias,
            Spec = space_a,
            StatusText = state,
            Money = money_kind,
            SupplierName = fac_a,
            Ip = ip_address,
            Trans = memo_a,
            Work_A = work_a,
            Work_M = work_m,
            GetDate = date_s.ToDatetimeOrNull(),
            OutDate = date_o.ToDatetimeOrNull(),
            Fired = fired.ToInt() < 0
         };
         list.Add(device);
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
}
public class DeviceOldDbHelpers
{
   public static void UpdatePropNumber(DefaultContext context)
   {
      var props = context.Properties.ToList();
      var devices = context.Devices.ToList();
      devices = devices.Where(x => x.NoProperty).ToList();
      foreach (var device in devices)
      {
         if (string.IsNullOrEmpty(device.PropNum)) continue;
         var prop = props.FirstOrDefault(x => x.Number == device.PropNum);
         if (prop == null) continue;
         device.PropertyId = prop.Id;
      }
      context.SaveChanges();
   }
   public static List<Device> GetListFromOldDb(string connectionString)
   {
      var list = new List<Device>();
      using (var connection = new SqlConnection(connectionString))
      {
         string query = "SELECT * FROM [hlh_device]";
         connection.Open();

         using (var command = new SqlCommand(query, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read()) // 逐行讀取結果
               {
                  var device = new Device()
                  {
                     OldId = Convert.ToInt32(reader["no1"]),
                     Title = reader["script"] == DBNull.Value ? null : reader["script"].ToString(),
                     No = reader["de_no"] == DBNull.Value ? "" : reader["de_no"].ToString(),
                     Kind = reader["de_kind"] == DBNull.Value ? null : reader["de_kind"].ToString(),
                     PropNum = reader["de_hlhno"] == DBNull.Value ? null : reader["de_hlhno"].ToString(),
                     Room = reader["room"] == DBNull.Value ? null : reader["room"].ToString(),
                     UserName = reader["username"] == DBNull.Value ? null : reader["username"].ToString(),
                     BrandName = reader["de_name1"] == DBNull.Value ? null : reader["de_name1"].ToString(),
                     Type = reader["de_name2"] == DBNull.Value ? null : reader["de_name2"].ToString(),
                     Alias = reader["alias"] == DBNull.Value ? null : reader["alias"].ToString(),
                     Spec = reader["space_a"] == DBNull.Value ? null : reader["space_a"].ToString(),
                     StatusText = reader["state"] == DBNull.Value ? null : reader["state"].ToString(),
                     Money = reader["money_kind"] == DBNull.Value ? null : reader["money_kind"].ToString(),
                     SupplierName = reader["fac_a"] == DBNull.Value ? null : reader["fac_a"].ToString(),
                     Ip = reader["ip_address"] == DBNull.Value ? null : reader["ip_address"].ToString(),
                     Trans = reader["memo_a"] == DBNull.Value ? null : reader["memo_a"].ToString(),
                     Work_A = reader["work_a"] == DBNull.Value ? null : reader["work_a"].ToString(),
                     Work_M = reader["work_m"] == DBNull.Value ? null : reader["work_m"].ToString(),
                     GetDate = reader["date_s"].ToNullableDateTime(),
                     OutDate = reader["date_o"].ToNullableDateTime(),
                     Fired = Convert.ToBoolean(reader["fired"]),
                  };
                  device.PropNum = string.IsNullOrEmpty(device.PropNum) ? "" : device.PropNum.Replace("-", "").Trim();
                  if (device.PropNum.Length == 21) device.PropNum = device.PropNum.Substring(3);

                  list.Add(device);

               }
            }
         }
      }
      return list;
   }

   public static List<Device> RefreshList(List<Device> sourceList, List<Property> properties, List<Location> locations, List<Category> categories, List<Profiles> profiles)
   { 
      foreach (var source in sourceList)
      {
         //PropNum
         source.PropNum = string.IsNullOrEmpty(source.PropNum) ? "" : source.PropNum.Replace("-", "").Trim();
         if (source.PropNum.Length == 21) source.PropNum = source.PropNum.Substring(3);

         if (!string.IsNullOrEmpty(source.PropNum))
         {
            var property = properties.FirstOrDefault(x => x.Number == source.PropNum);
            if (property != null) source.PropertyId = property.Id;
         }

         //category
         string key = string.IsNullOrEmpty(source.No) ? "" : Regex.Match(source.No.Trim(), @"^[A-Za-z]+").Value;
         if (!string.IsNullOrEmpty(key))
         {
            var category = categories.FirstOrDefault(x => x.Key == key);
            if (category != null) source.CategoryId = category.Id;
         }


         //location
         if (!string.IsNullOrEmpty(source.Room))
         {
            var location = locations.FirstOrDefault(x => x.Title == source.Room.Trim());
            if (location != null) source.LocationId = location.Id;
         }
         //user
         if (!string.IsNullOrEmpty(source.UserName))
         {
            var profile = profiles.FirstOrDefault(x => x.Name == source.UserName.Trim());
            if (profile != null) source.UserId = profile.UserId;
         }
      }
      return sourceList;
   }
}
