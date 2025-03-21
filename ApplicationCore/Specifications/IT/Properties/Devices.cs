using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;
using System.Linq;

namespace ApplicationCore.Specifications.IT;

public abstract class BaseDeviceSpecification : Specification<Device>
{
   public BaseDeviceSpecification(ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!)
         {
            Query.Include(item);
         }
      }
      Query.Where(item => !item.Removed);
   }
}
public class DeviceSpecification : BaseDeviceSpecification
{
   public DeviceSpecification(bool fired, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(x => x.Fired == fired);
   }
}
public class DeviceCategorySpecification : BaseDeviceSpecification
{
   public DeviceCategorySpecification(bool fired, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(x => x.Fired == fired && !x.CategoryId.HasValue);
   }
   public DeviceCategorySpecification(bool fired, ICollection<int> categoryIds, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(x => x.Fired == fired && x.CategoryId.HasValue && categoryIds.Contains(x.CategoryId.Value!));
   }
}
public class DevicebyCodeSpecification : Specification<Device>
{
   public DevicebyCodeSpecification(string code)
   {
      Query.Where(x => x.No == code);
   }
}
