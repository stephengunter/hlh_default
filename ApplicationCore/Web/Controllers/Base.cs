using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Settings;
using Microsoft.AspNetCore.Cors;
using ApplicationCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Authorization;
using ApplicationCore.Exceptions.Identity;
using Infrastructure.Helpers;
using OpenIddict.Validation.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using ApplicationCore.Web.Requests;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ApplicationCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{
   protected string RemoteIpAddress => HttpContext.Connection.RemoteIpAddress is null ? "" : HttpContext.Connection.RemoteIpAddress.ToString();
   
   protected void CheckCurrentUser(User user)
   {
      string id = User.Id();
      if(String.IsNullOrEmpty(id)) throw new CurrentUserIdNotFoundException();
      if(id != user.Id) throw new CurrentUserIdNotEqualToRequestUserIdException();
   }

   protected string TempPath(IWebHostEnvironment environment)
     => Path.Combine(environment.WebRootPath, "temp");

   protected string GetTempPath(IWebHostEnvironment environment, string folder)
     => Path.Combine(TempPath(environment), folder);

   protected string TemplatePath(IWebHostEnvironment environment, AppSettings appSettings)
     => Path.Combine(environment.WebRootPath, appSettings.TemplatePath.HasValue() ? appSettings.TemplatePath : "templates");

   protected string GetMailTemplate(IWebHostEnvironment environment, AppSettings appSettings, string name = "default")
   {
      var pathToFile = Path.Combine(TemplatePath(environment, appSettings), $"{name}.html");
      if (!System.IO.File.Exists(pathToFile)) throw new Exception("email template file not found: " + pathToFile);

      string body = "";
      using (StreamReader reader = System.IO.File.OpenText(pathToFile))
      {
         body = reader.ReadToEnd();
      }

      return body.Replace("APPNAME", appSettings.Title).Replace("APPURL", appSettings.ClientUrl);
   }
   protected  Dictionary<string, string> ValidateExcelFile(IFormFile file)
   {
      var errors = new Dictionary<string, string>();
      if (file == null)
      {
         errors.Add("file", "必須上傳檔案");
         return errors;
      }
      else
      {
         // Check file extension (for Excel files)
         var allowedExtensions = new[] { ".xlsx", ".xls" };
         var extension = Path.GetExtension(file.FileName).ToLower();

         if (!allowedExtensions.Contains(extension))
         {
            errors.Add("file", "只接受 Excel 檔案 (.xlsx, .xls)");
            return errors;
         }

         // Check MIME type for Excel
         if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
             file.ContentType != "application/vnd.ms-excel")
         {
            errors.Add("file", "檔案類型必須是 Excel (.xlsx, .xls)");
            return errors;
         }

         return errors;
      }
   }
   protected void AddErrors(Dictionary<string, string> errors)
   {
      if (errors.Count > 0)
      {
         foreach (var kvp in errors)
         {
            ModelState.AddModelError(kvp.Key, kvp.Value);
         }
      }
   }
}


[EnableCors("Api")]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public abstract class BaseApiController : BaseController
{
   
}

[EnableCors("Admin")]
[Route("admin/[controller]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Policy = "Admin")]
public class BaseAdminController : BaseController
{
   protected void ValidateRequest(AdminRequest model, AdminSettings adminSettings)
   {
      if (string.IsNullOrEmpty(model.Key)) ModelState.AddModelError("key", "認證錯誤");
      else
      {
         if (model.Key != adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");
      }
   }
}



[EnableCors("Global")]
[Route("tests/[controller]")]
public abstract class BaseTestController : BaseController
{

}

[EnableCors("Open")]
[Route("open/[controller]")]
public abstract class BaseOpenController : BaseController
{

}





