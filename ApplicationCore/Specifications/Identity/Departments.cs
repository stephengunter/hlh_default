using Ardalis.Specification;
using ApplicationCore.Models.Identity;

namespace ApplicationCore.Specifications.Identity;
public class DepartmentSpecification : Specification<Department>
{
	public DepartmentSpecification()
	{
		Query.Where(item => !item.Removed);
	}
   public DepartmentSpecification(string key)
   {
      Query.Where(item => !item.Removed).Where(item => item.Key == key);
   }
   
   public DepartmentSpecification(IEnumerable<int> ids)
   {
      Query.Where(item => !item.Removed).Where(item => ids.Contains(item.Id));
   }
}
public class DepartmentRootSpecification : Specification<Department>
{
   public DepartmentRootSpecification()
   {
      Query.Where(item => !item.Removed).Where(item => item.ParentId == null || item.ParentId == 0);
   }
}
public class DepartmentParentSpecification : Specification<Department>
{
   public DepartmentParentSpecification(Department parent)
   {
      Query.Where(item => !item.Removed).Where(item => item.ParentId == parent.Id);
   }
}
public class DepartmentTitleSpecification : Specification<Department>
{
   public DepartmentTitleSpecification(string title)
   {
      Query.Where(item => !item.Removed).Where(item => item.Title == title);
   }
}

