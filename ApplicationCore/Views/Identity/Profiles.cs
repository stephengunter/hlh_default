using Infrastructure.Helpers;

namespace ApplicationCore.Views.Identity;

public class ProfilesViewModel
{
	public string UserId { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;

   public int? DepartmentId { get; set; }
   public DepartmentViewModel? Department { get; set; }

   public string? Ps { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }


   public string CreatedAtText => CreatedAt.ToDateTimeString();
   public string LastUpdatedText => LastUpdated.ToDateTimeString();

}
