using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.IT;

[Table("IT.Properties")]
public class Property : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public int? CategoryId { get; set; }
   public PropertyType PropertyType { get; set; }
   public string LocationName { get; set; } = String.Empty;
   public string CategoryName { get; set; } = String.Empty;
   public string BrandName { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Type { get; set; } = String.Empty;
   public string Number { get; set; } = String.Empty;
   public string? UserName { get; set; }
   public string? UserId { get; set; }

   public int? LocationId { get; set; }
   public DateTime? BuyDate { get; set; }
   public DateTime? GetDate { get; set; }
   public DateTime? DownDate { get; set; }

   public int MinYears { get; set; }

   public bool Deprecated { get; set; }
   public string? Ps { get; set; }
   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

}

public enum PropertyType
{ 
   Item = 0,   //物品
   Property = 1, //財產
}
