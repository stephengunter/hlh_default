using ApplicationCore.Helpers.IT;
using ApplicationCore.Migrations;
using ApplicationCore.Models.IT;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;
using ApplicationCore.Consts.IT;

namespace ApplicationCore.Views.IT;

public class PropertyViewModel : EntityBaseView, IBaseRecordView
{
   public bool IsItDevice { get; set; }
   public int? CategoryId { get; set; }
   public PropertyType PropertyType { get; set; }
   public string PropertyTypeText => PropertyType == PropertyType.Property ? PropertyTypeTitles.Property : PropertyTypeTitles.Item;
   public string LocationName { get; set; } = String.Empty;
   public string CategoryName { get; set; } = String.Empty;
   public string DeviceCode { get; set; } = String.Empty;
   public string BrandName { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Type { get; set; } = String.Empty;
   public string Number { get; set; } = String.Empty;
   public string NumberText => Number.ToPropertyNumberText();
   public string NumberStickText => Number.ToPropertyNumberStickText();
   
   public string? UserName { get; set; }
   public string? UserId { get; set; }
   public int? LocationId { get; set; }
   public int? DeviceId { get; set; }
   public DateTime? BuyDate { get; set; }
   public DateTime? GetDate { get; set; }
   public DateTime? DownDate { get; set; }
   public int MinYears { get; set; }

   public bool IsDown => DownDate.HasValue;
   public bool Deprecated { get; set; }
   public string TitleNameText
   {
      get
      {
         var items = new List<string>();
         if (!string.IsNullOrEmpty(Title)) items.Add(Title);
         if (!string.IsNullOrEmpty(Name)) items.Add(Name);

         return items.JoinToString(" / ");
      }
   }
   public string? Ps { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }

   public DateTime CreatedAt { get; set; }
   public string CreatedBy { get; set; } = String.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateTimeString();
   public string LastUpdatedText => LastUpdated.ToDateTimeString();
   public string GetDateText => GetDate.ToDateString();
   public string DownDateText => DownDate.ToDateString();
   public string BuyDateText => BuyDate.ToDateString();

}

public class PropertyLabels
{
   public string PropertyType => "帳別";
   public string Title => "名稱";
   public string Name => "別名";
   public string Brand => "廠牌";
   public string Type => "型號";
   public string Category => "分類";
   public string Number => "財產編號";
   public string Location => "存置地點";
   public string UserName => "保管人";
   public string DownDate => "下架日期";
   public string GetDate => "取得日期";
   public string BuyDate => "購置日期";
   public string ItDevice => "資訊設備";
   public string DeviceCode => "設備編號";
   public string Ps => "備註";
}
public class PropertiesGroupView
{
   public int? CategoryId { get; set; }
   public string CategoryName { get; set; } = string.Empty;
   public int Count { get; set; }
}

public class SourcePropertyModel
{
   public string UId { get; set; } = Guid.NewGuid().ToString();
   public PropertyType PropertyType { get; set; }
   public string LocationCode { get; set; } = String.Empty;
   public string LocationName { get; set; } = String.Empty;
   public string CategoryName { get; set; } = String.Empty;
   public string BrandName { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Type { get; set; } = String.Empty;
   public string Number { get; set; } = String.Empty;
   public string? UserCode { get; set; }
   public string? UserName { get; set; }
   public DateTime? BuyDate { get; set; }
   public DateTime? GetDate { get; set; }
   public DateTime? DownDate { get; set; }
   public int MinYears { get; set; }
   public bool Deprecated { get; set; }
   public bool Removed { get; set; }

   public string NumberText => Number.ToPropertyNumberText();
   public string NumberStickText => Number.ToPropertyNumberStickText();
   public string PropertyTypeText => PropertyType == PropertyType.Property ? PropertyTypeTitles.Property : PropertyTypeTitles.Item;
   public string GetDateText => GetDate.ToDateString();
   public string DownDateText => DownDate.ToDateString();
   public string BuyDateText => BuyDate.ToDateString();
   public string TitleNameText
   {
      get
      {
         var items = new List<string>();
         if (!string.IsNullOrEmpty(Title)) items.Add(Title);
         if (!string.IsNullOrEmpty(Name)) items.Add(Name);

         return items.JoinToString(" / ");
      }
   }
}
