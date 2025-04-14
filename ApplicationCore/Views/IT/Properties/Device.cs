using ApplicationCore.Helpers.IT;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views.IT;

public class DeviceViewModel : EntityBaseView, IBaseRecordView
{
   public int? OldId { get; set; }
   public string? Title { get; set; } //script
   public string? No { get; set; }  //de_no
   public string? Kind { get; set; } //de_kind
   public string? PropNum { get; set; } //de_hlhno
   public string? Room { get; set; } //room
   public string? UserName { get; set; } //username
   public string? BrandName { get; set; } //de_name1
   public string? Type { get; set; } //de_name2
   public string? Alias { get; set; } //alias
   public string? Spec { get; set; } //space_a
   public string? StatusText { get; set; } //state
   public string? Money { get; set; }  //money_kind
   public string? SupplierName { get; set; } //fac_a

   public string? Trans { get; set; } // memo_a
   public string? Ip { get; set; } // ip_address

   public string? Work_A { get; set; } //work_a
   public string? Work_M { get; set; } //work_m

   public int? PropertyId { get; set; }
   public int? CategoryId { get; set; }
   public string? UserId { get; set; }

   public int? LocationId { get; set; }
   public DateTime? GetDate { get; set; } //date_s

   public bool Fired { get; set; } //fired
   public DateTime? OutDate { get; set; } //date_o
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

   public string OutDateText => OutDate.ToDateTimeString();
   public string GetDateText => GetDate.ToDateTimeString();

   public string PropNumText => PropNum.ToPropertyNumberText();
   public string PropNumStickText => PropNum.ToPropertyNumberStickText();

}

public class DeviceLabels
{
   public string Title => "名稱";
   public string Name => "別名";
   public string Category => "設備分類";
   public string No => "設備編號"; 
   public string Brand => "廠牌";
   public string Type => "型號";
   public string PropNum => "財產編號";
   public string Location => "存置地點";
   public string Room => "存置地點";
   public string UserName => "保管人";
   public string DownDate => "下架日期";
   public string GetDate => "取得日期";
   public string Ps => "備註";
}