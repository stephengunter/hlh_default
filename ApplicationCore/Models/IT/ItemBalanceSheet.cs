using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.IT;

[Table("IT.ItemBalanceSheets")]
public class ItemBalanceSheet : EntityBase
{
   public int ReportId { get; set; }
   [Required]
   public virtual ItemReport? Report { get; set; }


   public DateTime Date { get; set; }

   [Required]
   public virtual Item? Item { get; set; }
   public int ItemId { get; set; }
   public int LastStock { get; set; }
   public int InQty { get; set; }
   public int OutQty { get; set; }
   public int QuantityChanged => InQty - OutQty;
   public int Stock => LastStock + QuantityChanged;

   public string? Ps { get; set; }
}
