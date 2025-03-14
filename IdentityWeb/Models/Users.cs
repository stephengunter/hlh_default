using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace IdentityWeb.Models;

public class UserLabels
{
   public string UserName { get; } = "UserName";
   public string Email { get; } = "Email";
   public string Name { get; } = "�W��";
   public string PhoneNumber { get; } = "������X";
   public string Roles { get; } = "����";
   public string Active { get; } = "���A";
   public string CreatedAt { get; } = BaseLabels.CreatedAt;
   public string LastUpdated { get; } = BaseLabels.LastUpdated;

   public string Department { get; } = "����";

   public ProfilesLabels Profiles { get; } = new ProfilesLabels();
}
public class ProfilesLabels
{
   public string Name { get; } = "�m�W";
   public string Ps { get; } = "�Ƶ�";
   public string CreatedAt { get; } = BaseLabels.CreatedAt;
   public string LastUpdated { get; } = BaseLabels.LastUpdated;
}
public class UsersAdminRequest
{
   public UsersAdminRequest(bool active, int? department, string? role, string? keyword, int page = 1, int pageSize = 10)
   {
      Active = active;
      Department = department;
      Role = role;
      Keyword = keyword;
      Page = page < 1 ? 1 : page;
      PageSize = pageSize;
   }
   
   public bool Active { get; set; }
   public int? Department { get; set; }
   public string? Role { get; set; }
   public string? Keyword { get; set; }
   public int Page { get; set; } 
   public int PageSize { get; set; }
}
public class UsersAdminModel
{
   public UsersAdminModel(UsersAdminRequest request, ICollection<RoleViewModel> roles, ICollection<DepartmentViewModel> departments)
   {
      Request = request; 
      Roles = roles;
      Departments = departments;
   }
   public UserLabels Labels { get; set; } = new UserLabels();
   public UsersAdminRequest Request { get; set; }
   public ICollection<RoleViewModel> Roles{ get; set; } = new List<RoleViewModel>();
   public ICollection<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();

}

public abstract class BaseUserForm
{
   public string? UserName { get; set; }
   public string? Name { get; set; }

   public string? Email { get; set; }

   public string? PhoneNumber { get; set; }

   public bool Active { get; set; }
   public ICollection<string> Roles { get; set; } = new List<string>();

   public void SetRoles(ICollection<Role> roles)
   {
      if (roles.HasItems()) Roles = roles!.Select(x => x.Name!).ToList();
      else Roles = new List<string>();
   }

}
public class UserCreateForm : BaseUserForm
{
   
}

public class UserEditForm : BaseUserForm
{
   
}

public class UsersImportRequest
{
   public List<IFormFile> Files { get; set; } = new List<IFormFile>();
}

public class UsersUpDownRequest
{
   public bool Up { get; set; }
   public List<string> Ids { get; set; } = new List<string>();
}