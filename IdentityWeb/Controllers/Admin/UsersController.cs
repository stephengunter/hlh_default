using Microsoft.AspNetCore.Mvc;
using IdentityWeb.Models;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using ApplicationCore.Authorization;
using ApplicationCore.Services.Identity;
using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using Ardalis.Specification;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Infrastructure.Paging;
using ApplicationCore.Web.Requests;

namespace IdentityWeb.Controllers.Admin;

public class UsersController : BaseAdminController
{
   private readonly IUsersService _usersService;
   private readonly IRolesService _rolesService;
   private readonly IProfilesService _profilesService;
   private readonly ILdapService _ldapService;
   private readonly IDepartmentsService _departmentsService;
   private readonly IMapper _mapper;
   private readonly AdminSettings _adminSettings;
   private readonly CompanySettings _companySettings;
   private readonly UserLabels _labels;

   public UsersController(IUsersService usersService, IRolesService rolesService, IProfilesService profilesService, IOptions<LdapSettings> ldapSettings,
      ILdapService ldapService, IDepartmentsService departmentsService, IOptions<AdminSettings> adminSettings, IOptions<CompanySettings> companySettings, IMapper mapper)
   {
      _usersService = usersService;
      _rolesService = rolesService;
      _profilesService = profilesService;
      _departmentsService = departmentsService;
      _mapper = mapper;
      _adminSettings = adminSettings.Value;
      _companySettings = companySettings.Value;

      _ldapService = ldapService;
      _labels = new UserLabels();
   }
   [HttpGet("init")]
   public async Task<ActionResult<UsersAdminModel>> Init()
   {
      bool active = true;
      int department = 0;
      string role = "";
      string keyword = "";
      int page = 1;
      int pageSize = 10;

      var request = new UsersAdminRequest(active, department, role, keyword, page, pageSize);

      var roles = await _rolesService.FetchAllAsync();
      var departments = await _departmentsService.FetchRootsAsync();
      departments = departments.Where(x => x.Active);
      departments = departments.GetOrdered();

      return new UsersAdminModel(request, roles.MapViewModelList(_mapper), departments.MapViewModelList(_mapper));
   }

   [HttpGet]
   public async Task<ActionResult<PagedList<User, UserViewModel>>> Index(bool active, int? department, string? role, string? keyword, int page = 1, int pageSize = 10)
   {
      bool includeRoles = true;
      var roleNames = new List<string>();
      if (!string.IsNullOrEmpty(role)) roleNames = role.Split(',').ToList();

      Department? selectedDepartment = null;
      if (department.HasValue && department.Value > 0)
      {
         selectedDepartment = await _departmentsService.GetByIdAsync(department.Value);
         if (selectedDepartment == null) ModelState.AddModelError("department", $"department not found. id: {department.Value}");
      }
      if (!ModelState.IsValid) return BadRequest(ModelState);

      IEnumerable<User> users;
      if (roleNames.Count == 0)
      {
         users = await _usersService.FetchAllAsync(includeRoles);
      }
      else
      {
         var selectedRoles = new List<Role>();
         foreach (var roleName in roleNames)
         {
            var selectedRole = await _rolesService.FindAsync(roleName);
            if (selectedRole == null) ModelState.AddModelError("role", $"Role '{roleName}' not found.");
            else selectedRoles.Add(selectedRole);
         }
         if (!ModelState.IsValid) return BadRequest(ModelState);

         users = await _usersService.FetchByRolesAsync(selectedRoles, includeRoles);
      }

      if (selectedDepartment != null)
      {
         var profiles = await _profilesService.FetchAsync(selectedDepartment);
         var userIds = profiles.Select(x => x.UserId).ToList();
         if (userIds.HasItems()) users = users.Where(u => userIds.Contains(u.Id));
         else users = new List<User>();
      }

      users = users.Where(u => u.Active == active);

      var keywords = keyword.GetKeywords();
      if (keywords.HasItems()) users = users.FilterByKeyword(keywords);

      var model = new PagedList<User, UserViewModel>(users, page, pageSize);
      foreach (var user in model.List)
      {
         await LoadRolesAsync(user);
      }

      model.SetViewList(model.List.MapViewModelList(_mapper));


      return model;
   }

   [HttpGet("create")]
   public ActionResult<UserCreateForm> Create() => new UserCreateForm();

   [HttpPost]
   public async Task<ActionResult<UserViewModel>> Store([FromBody] UserCreateForm model)
   {
      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var user = new User();
      model.SetValuesTo(user);

      user.SetCreated(User.Id());

      user = await _usersService.CreateAsync(user);

      return Ok(user.MapViewModel(_mapper));
   }

   async Task LoadRolesAsync(User user)
   {
      var roleIds = user.UserRoles!.HasItems() ? user.UserRoles!.Select(x => x.RoleId).ToList() : new List<string>();

      if (roleIds.HasItems()) user.Roles = (await _rolesService.FetchByIdsAsync(roleIds)).ToList();
   }

   [HttpGet("{id}")]
   public async Task<ActionResult<UserViewModel>> Details(string id)
   {
      bool includeRoles = true;
      var user = await _usersService.GetByIdAsync(id, includeRoles);
      if (user == null) return NotFound();

      await LoadRolesAsync(user);
      return user.MapViewModel(_mapper);
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult<UserEditForm>> Edit(string id)
   {
      bool includeRoles = true;
      var user = await _usersService.GetByIdAsync(id, includeRoles);
      if (user == null) return NotFound();

      await LoadRolesAsync(user);

      var model = new UserEditForm();
      var excepts = new List<string> { "Roles" };
      user.SetValuesTo(model, excepts);


      model.SetRoles(user!.Roles);
      return model;
   }
   [HttpPut("{id}")]
   public async Task<ActionResult> Update(string id, [FromBody] UserEditForm model)
   {
      var user = await _usersService.FindByIdAsync(id);
      if (user == null) return NotFound();

      await ValidateRequestAsync(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);


      var excepts = new List<string>() { "Roles" };
      model.SetValuesTo(user, excepts);
      user.SetUpdated(User.Id());

      await _usersService.UpdateAsync(user);

      var currentRoles = await _usersService.GetRolesAsync(user);
      if (!currentRoles.AllTheSame(model.Roles))
      {
         await _usersService.SyncRolesAsync(user, model.Roles);
      }

      return NoContent();
   }
   [HttpPost("sync")]
   public async Task<IActionResult> Sync([FromBody] AdminRequest request)
   {
      ValidateRequest(request, _adminSettings);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var adUsers = _ldapService.FetchAll();
      adUsers = adUsers.Where(x => !String.IsNullOrEmpty(x.Department));
      foreach (var aduser in adUsers)
      {
         var department = await _departmentsService.FindByTitleAsync(aduser.Department);
         if (department == null) continue;

         var user = await _usersService.FindByUsernameAsync(aduser.Username);
         if (user == null)
         {
            user = await _usersService.CreateAsync(new User
            {
               UserName = aduser.Username,
               Name = aduser.Username,
               SecurityStamp = Guid.NewGuid().ToString(),
               Active = true,
               CreatedAt = DateTime.Now,
               CreatedBy = User.Id()
            });
         }

         var profiles = await _profilesService.FindAsync(user);
         if (profiles == null)
         {
            profiles = await _profilesService.CreateAsync(new Profiles
            {
               UserId = user.Id,
               Name = aduser.Name,
               DepartmentId = department.Id,
               CreatedAt = DateTime.Now,
               CreatedBy = User.Id()
            });
         }
         else
         {
            profiles.Name = aduser.Name;
            profiles.DepartmentId = department.Id;
            profiles.LastUpdated = DateTime.Now;
            profiles.UpdatedBy = User.Id();

            await _profilesService.UpdateAsync(profiles);
         }

      }

      return Ok();
   }

   [HttpPost("updown")]
   public async Task<IActionResult> UpDown([FromBody] UsersUpDownRequest request)
   {
      if (request.Ids.IsNullOrEmpty()) ModelState.AddModelError("ids", "���~��ids");
      if (!ModelState.IsValid) return BadRequest(ModelState);
      var users = await _usersService.FetchByIdsAsync(request.Ids);
      foreach (var user in users)
      {
         user.Active = request.Up;
         user.LastUpdated = DateTime.Now;
         user.UpdatedBy = User.Id();
      }
      await _usersService.UpdateRangeAsync(users.ToList());
      return Ok();
   }

   async Task ValidateRequestAsync(BaseUserForm model, string id = "")
   {
      await CheckUserNameAsync(model, id);
      await CheckEmailAsync(model, id);
      await CheckPhoneNumberAsync(model, id);
   }

   async Task CheckUserNameAsync(BaseUserForm model, string id)
   {
      string key = "UserName";
      if (String.IsNullOrEmpty(model.UserName))
      {
         if (model is UserCreateForm) ModelState.AddModelError(key, $"������g{_labels.UserName}");
      }
      else
      {
         if (model.UserName.IsValidUserName())
         {
            var existingUser = await _usersService.FindByUsernameAsync(model.UserName);
            if (existingUser != null && existingUser.Id != id) ModelState.AddModelError(key, $"{_labels.UserName}���ƤF");
         }
         else
         {
            ModelState.AddModelError(key, $"{_labels.UserName}���榡�����T");
         }
      }
   }
   async Task CheckEmailAsync(BaseUserForm model, string id)
   {
      string key = "Email";
      if (String.IsNullOrEmpty(model.Email))
      {

      }
      else
      {
         if (model.Email.IsValidEmail())
         {
            var existingUser = await _usersService.FindByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != id) ModelState.AddModelError(key, $"{_labels.Email}���ƤF");
         }
         else
         {
            ModelState.AddModelError(key, $"{_labels.Email}���榡�����T");
         }
      }

   }
   async Task CheckPhoneNumberAsync(BaseUserForm model, string id)
   {
      string key = "PhoneNumber";
      if (String.IsNullOrEmpty(model.PhoneNumber))
      {

      }
      else
      {
         if (model.PhoneNumber.IsValidPhoneNumber())
         {
            var existingUser = await _usersService.FindByPhoneAsync(model.PhoneNumber);
            if (existingUser != null && existingUser.Id != id) ModelState.AddModelError(key, $"{_labels.PhoneNumber}���ƤF");
         }
         else
         {
            ModelState.AddModelError(key, $"{_labels.PhoneNumber}���榡�����T");
         }
      }

   }
}