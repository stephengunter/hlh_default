using ApplicationCore.Consts;
using ApplicationCore.Views.Identity;
using Infrastructure.Helpers;
using Infrastructure.Views;
using System.Linq;

namespace ITApi.Models;

public class ItemBalanceSheetLabels
{
   public string Item => "品名";
   public string LastStock => "前期庫存";
   public string QuantityChanged => "本期增減";
   public string Stock => "期末庫存";
   public string Ps => "備註";
}

