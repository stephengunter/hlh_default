using ApplicationCore.Consts;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.Identity;
using ApplicationCore.Views.IT;
using Azure.Core;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ITApi.Models;

public class DeviceLabels
{
   public string Title => "名稱";
   public string Category => "設備分類";
   public string No => "代碼";
   public string PropNum => "財產編號";
   public string Room => "存置地點";
   public string Ps => "備註";
}
public class DevicesAdminModel
{
   public DevicesAdminModel(DevicesFetchRequest request, CategoryViewModel rootCategory)
   {
      Request = request;
      RootCategory = rootCategory;
   }
   public DevicesFetchRequest Request { get; set; }
   public CategoryViewModel RootCategory { get; set; }
   public ICollection<CategoryViewModel> Categories { get; set; }
   public DeviceLabels Labels => new DeviceLabels();
   //public DeviceTransactionLabels TransactionLabels => new DeviceTransactionLabels();
}
public class DevicesIndexModel
{
   public DevicesIndexModel(ICollection<DeviceViewModel> items)
   {
      Devices = items;
   }
   
   public ICollection<DeviceViewModel> Devices { get; set; }
}
public class DevicesFetchRequest
{
   public DevicesFetchRequest(bool fired, int? category)
   {
      Fired = fired;
      Category = category;
   }
   public bool Fired { get; set; }
   public int? Category { get; set; }
}
public abstract class BaseDeviceForm
{
   public string Name { get; set; } = String.Empty;
   public string Code { get; set; } = String.Empty;
   public string? Ps { get; set; }
   public bool Active { get; set; }
}
public class DeviceAddForm : BaseDeviceForm
{

}
public class DeviceEditForm : BaseDeviceForm
{
   public bool CanRemove { get; set; }
}