using ApplicationCore.Consts;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.Identity;
using ApplicationCore.Views.IT;
using Azure.Core;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ITApi.Models;

public class PropertyLabels
{
   public string PropertyType => "帳別";
   public string Title => "名稱";
   public string Name => "別名";
   public string Brand => "廠牌";
   public string Type => "型號";
   public string Category => "財產分類";
   public string Number => "財產編號";
   public string Location => "存置地點";
   public string UserName => "保管人";
   public string DownDate => "下架日期";
   public string Ps => "備註";
}
public class PropertiesAdminModel
{
   public PropertiesAdminModel(PropertiesFetchRequest request, ICollection<CategoryViewModel> categories,
      ICollection<LocationViewModel> locations)
   {
      Request = request;
      Categories = categories;
      Locations = locations;
   }
   public PropertiesFetchRequest Request { get; set; }
   public ICollection<CategoryViewModel> Categories { get; set; }
   public ICollection<LocationViewModel> Locations { get; set; }
   public PropertyLabels Labels => new PropertyLabels();
}
public class PropertiesFetchRequest
{
   public PropertiesFetchRequest(bool deprecated, int? category)
   {
      Deprecated = deprecated;
      Category = category;
   }
   public bool Deprecated { get; set; }
   public int? Category { get; set; }
}