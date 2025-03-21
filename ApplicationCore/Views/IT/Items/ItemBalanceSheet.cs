using ApplicationCore.Models.IT;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Views.IT;

public class ItemBalanceSheetViewModel : EntityBaseView
{
   public int ReportId { get; set; }
   public DateTime Date { get; set; }
   public ItemViewModel? Item { get; set; }
   public int ItemId { get; set; }

   public int LastStock { get; set; }
   public int InQty { get; set; }
   public int OutQty { get; set; }
   public int QuantityChanged { get; set; }
   public int Stock { get; set; }

   public string? Ps { get; set; }

}
