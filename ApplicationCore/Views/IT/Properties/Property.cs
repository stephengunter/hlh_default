using ApplicationCore.Migrations;
using ApplicationCore.Models.IT;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views.IT;

public class PropertyViewModel : EntityBaseView, IBaseRecordView
{
   public int? CategoryId { get; set; }
   public PropertyType PropertyType { get; set; }
   public string PropertyTypeText => PropertyType == PropertyType.Property ? "財產" : "物品";
   public string LocationName { get; set; } = String.Empty;
   public string CategoryName { get; set; } = String.Empty;
   public string BrandName { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Type { get; set; } = String.Empty;
   public string Number { get; set; } = String.Empty;
   public string NumberText 
   {  
      get 
      {
         if(string.IsNullOrEmpty(Number)) return "";
         if (Number.Length == 18)
         {
            return $"{Number[0]}-{Number.Substring(1, 2)}-{Number.Substring(3, 2)}-{Number.Substring(5, 2)}-{Number.Substring(7, 4)}-{Number.Substring(11, 7)}";
         }
         return "";
      }
   }
   public string? UserName { get; set; }
   public string? UserId { get; set; }
   public int? LocationId { get; set; }
   public DateTime? BuyDate { get; set; }
   public DateTime? GetDate { get; set; }
   public DateTime? DownDate { get; set; }
   public int MinYears { get; set; }
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

}


public class SourcePropertyModel
{
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
}
