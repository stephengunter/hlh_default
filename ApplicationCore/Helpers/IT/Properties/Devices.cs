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

   public static PagedList<Device, DeviceViewModel> GetPagedList(this IEnumerable<Device> entitie, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Device, DeviceViewModel>(entitie, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Device MapEntity(this DeviceViewModel model, IMapper mapper, string currentUserId, Device? entity = null)
   {
      if (entity == null) entity = mapper.Map<DeviceViewModel, Device>(model);
      else entity = mapper.Map<DeviceViewModel, Device>(model, entity);

      return entity;
   }

   public static BaseOption<int> ToOption(this Device entity)
      => new BaseOption<int>(entity.Id, entity.Title!);

   public static IEnumerable<Device> GetOrdered(this IEnumerable<Device> entitie)
     => entitie.OrderBy(item => item.Id);
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
                     No = reader["de_no"] == DBNull.Value ? null : reader["de_no"].ToString(),
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
