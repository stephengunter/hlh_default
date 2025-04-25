using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.IT;

[Table("IT.Devices")]
public class Device : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public int? OldId { get; set; } //no1
   public string? Title { get; set; } //script
   public string No { get; set; }  = string.Empty; //de_no
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
   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);


   public bool NoCategory => CategoryId is null ? true : CategoryId.Value == 0;
   public bool NoProperty => PropertyId is null ? true : PropertyId.Value == 0;
}
