using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.IT;

[Table("IT.Items")]
public class Item : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string Name { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Code { get; set; } = String.Empty;
   public int Price { get; set; }
   public int Stock { get; set; }
   public int SaveStock { get; set; }
   public string? Supplier { get; set; }
   public string Unit { get; set; } = String.Empty;
   public string? Ps { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

}
