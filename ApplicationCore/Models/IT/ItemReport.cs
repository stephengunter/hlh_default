using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.IT;

[Table("IT.ItemReports")]
public class ItemReport : EntityBase, IBaseRecord
{
   public int Year { get; set; }
   public int Month { get; set; }
   public string? Date { get; set; }
   public string? Ps { get; set; }
   
   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public virtual ICollection<ItemBalanceSheet> ItemBalanceSheets { get; set; } = new List<ItemBalanceSheet>();
}
