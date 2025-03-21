using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.IT;

[Table("IT.ItemTransactions")]
public class ItemTransaction : EntityBase, IBaseRecord, IRemovable
{
   public DateTime Date { get; set; }

   public int ItemId { get; set; }
   [Required]
   public virtual Item? Item { get; set; }

   public int Quantity { get; set; }

   public int? DepartmentId { get; set; }

   public string? UserId { get; set; }
   public string? UserName { get; set; }

   public string? Ps { get; set; }
   public bool Removed { get; set; }
   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
}
