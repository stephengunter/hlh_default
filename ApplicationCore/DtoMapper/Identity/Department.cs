using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using AutoMapper;

namespace ApplicationCore.DtoMapper.Identity;

public class DepartmentMappingProfile : Profile
{
	public DepartmentMappingProfile()
	{
		CreateMap<Department, DepartmentViewModel>();

		CreateMap<DepartmentViewModel, Department>()
         .ForMember(x => x.Parent, opt => opt.Ignore());
   }
}
