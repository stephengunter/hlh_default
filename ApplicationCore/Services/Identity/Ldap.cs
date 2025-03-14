using ApplicationCore.Views.Identity.AD;
using ApplicationCore.Settings;
using Novell.Directory.Ldap;
using Microsoft.Extensions.Options;

namespace ApplicationCore.Services.Identity;

public interface ILdapService
{
   IEnumerable<AdUser> FetchAll();
   AdUser? CheckAuth(string username, string password);
}

public class LdapService : ILdapService
{
   private readonly LdapSettings _settings;

   private string ATTR_CN = "cn";
   private string ATTR_DISPLAY_NAME = "displayName";
   private string ATTR_DEPARTMENT = "department";
   private string ATTR_TITLE = "title";
   public LdapService(IOptions<LdapSettings> settings)
   {
      _settings = settings.Value;
   }

   string AdminLdapRdn => $"{_settings.AdminUser}@{_settings.Domain}";
   List<string> AttrKeys => new List<string> { ATTR_CN, ATTR_DISPLAY_NAME, ATTR_DEPARTMENT, ATTR_TITLE };

   public AdUser? CheckAuth(string username, string password)
   {
      try
      {
         using (var ldapConnection = new LdapConnection())
         {
            string ldapRdn = $"{username}@{_settings.Domain}";
            ldapConnection.Connect(_settings.Server, _settings.Port);
            ldapConnection.Bind(ldapRdn, password);

            string searchFilter = $"(sAMAccountName={username})";
            var searchResults = ldapConnection.Search(
                _settings.BaseDn,
                LdapConnection.ScopeSub,
                searchFilter,
                AttrKeys.ToArray(),
                false
            );

            while (searchResults.HasMore())
            {
               var entry = searchResults.Next();
               return ResolveAdUser(entry);
            }
            return null;
         }
      }
      catch (LdapException ex)
      {
         Console.WriteLine($"LDAP query failed: {ex.Message}");
         return null;
      }
   }

   AdUser ResolveAdUser(LdapEntry entry)
   {
      var attributes = entry.GetAttributeSet();
      
      string userName = GetAttributeValue(attributes, ATTR_CN);
      string name = GetAttributeValue(attributes, ATTR_DISPLAY_NAME);
      string title = GetAttributeValue(attributes, ATTR_TITLE);
      string dpt = GetAttributeValue(attributes, ATTR_DEPARTMENT);

      return new AdUser
      {
         Username = userName,
         Name = name,
         Title = title,
         Department = dpt
      };
   }
   public IEnumerable<AdUser> FetchAll()
   {
      var users = new List<AdUser>();
      using (var ldapConnection = new LdapConnection())
      {
         ldapConnection.Connect(_settings.Server, _settings.Port);
         ldapConnection.Bind(AdminLdapRdn, _settings.AdminPassword);
         
         string searchFilter = "(&(objectClass=user))";
         var searchResults = ldapConnection.Search(
             _settings.BaseDn, 
             LdapConnection.ScopeSub,
             searchFilter,
             null,
             false
         );

         while (searchResults.HasMore())
         {
            var entry = searchResults.Next();
            users.Add(ResolveAdUser(entry));
         }
      }
      return users;
   }

   private string GetAttributeValue(LdapAttributeSet attributes, string attributeName)
   {
      if (attributes.Keys.Contains(attributeName))
      {
         var attribute = attributes.GetAttribute(attributeName);
         if (attribute != null) return attribute.StringValue;
      }
      
      return "";
   }
}
