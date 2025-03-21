using Ardalis.Specification;
using ApplicationCore.Models.Identity;

namespace ApplicationCore.Specifications.Identity;

public abstract class BaseDepartmentSpecification : Specification<Department>
{
   public BaseDepartmentSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}
public class DepartmentSpecification : BaseDepartmentSpecification
{
	public DepartmentSpecification()
	{
		
	}
   public DepartmentSpecification(string key)
   {
      Query.Where(item => item.Key == key);
   }
   
   public DepartmentSpecification(IEnumerable<int> ids)
   {
      Query.Where(item => ids.Contains(item.Id));
   }
}
public class DepartmentRootSpecification : BaseDepartmentSpecification
{
   public DepartmentRootSpecification()
   {
      Query.Where(item => item.ParentId == null || item.ParentId == 0);
   }
}
public class DepartmentParentSpecification : BaseDepartmentSpecification
{
   public DepartmentParentSpecification(Department parent)
   {
      Query.Where(item => item.ParentId == parent.Id);
   }
}
public class DepartmentTitleSpecification : BaseDepartmentSpecification
{
   public DepartmentTitleSpecification(string title)
   {
      Query.Where(item => item.Title == title);
   }
}

