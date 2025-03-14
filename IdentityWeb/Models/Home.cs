using ApplicationCore.Views.Identity;

namespace IdentityWeb.Models;

public class HomeModel
{
   public HomeModel(ICollection<AppViewModel> apps)
   {
      Apps = apps;
   }
   
   public ICollection<AppViewModel> Apps { get; set; }
}