﻿using Ardalis.Specification;
using ApplicationCore.Models.Identity;

namespace ApplicationCore.Specifications.Identity;
public class ProfilesSpecification : Specification<Profiles>
{
   public ProfilesSpecification(User user)
   {
      Query.Where(p => p.UserId == user.Id);
   }
   public ProfilesSpecification(Department department)
   {
      Query.Where(p => p.DepartmentId == department.Id);
   }
}

