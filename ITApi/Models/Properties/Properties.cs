using ApplicationCore.Consts;
using ApplicationCore.Consts.IT;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.Identity;
using ApplicationCore.Views.IT;
using Azure.Core;
using Infrastructure.Helpers;
using Infrastructure.Paging;
using Infrastructure.Views;

namespace ITApi.Models;

public class PropertiesAdminModel
{
   public PropertiesAdminModel(PropertiesFetchRequest request, ICollection<CategoryViewModel> categories,
      ICollection<LocationViewModel> locations)
   {
      Request = request;
      Categories = categories;
      Locations = locations;
      var options = new List<BaseOption<int>>()
      {
         new BaseOption<int>(-1, "全部"),
         new BaseOption<int>((int)PropertyType.Item, PropertyTypeTitles.Item),
         new BaseOption<int>((int)PropertyType.Property, PropertyTypeTitles.Property)
      };
      TypeOptions = options;
   }
   public PropertiesFetchRequest Request { get; set; }
   public ICollection<CategoryViewModel> Categories { get; set; }
   public ICollection<LocationViewModel> Locations { get; set; }
   public List<BaseOption<int>> TypeOptions { get; set; }
   public PropertyLabels Labels => new PropertyLabels();
}
public class PropertiesIndexModel
{
   public PropertiesIndexModel(PagedList<Property, PropertyViewModel> pagedList, ICollection<PropertiesGroupView> groupViews)
   {
      PagedList = pagedList;
      GroupViews = groupViews;
   }
   public PagedList<Property, PropertyViewModel> PagedList { get; set; }
   public ICollection<PropertiesGroupView> GroupViews { get; set; }
}
public class PropertiesFetchRequest
{
   public PropertiesFetchRequest()
   { 
   }
   public PropertiesFetchRequest(bool deprecated, int down, int type, int category, int? location, int page = 1, int pageSize = 10)
   {
      Deprecated = deprecated;
      Down = down;
      Type = type;
      Category = category;
      Location = location;
      Page = page;
      PageSize = pageSize;
   }
   public bool Deprecated { get; set; }
   public int Down { get; set; }
   public int Type { get; set; }
   public int Category { get; set; }
   public int? Location { get; set; }
   public int? Page { get; set; }
   public int? PageSize { get; set; }
}
public class PropertiesUploadRequest
{
   public int PropertyType { get; set; }
   public IFormFile? File { get; set; }
}
public class PropertiesImportRequest
{
   public List<SourcePropertyModel> List { get; set; } = new List<SourcePropertyModel>();
}