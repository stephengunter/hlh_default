using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using AutoMapper;

namespace ApplicationCore.DtoMapper.Identity;

public class UserMappingProfile : Profile
{
	public UserMappingProfile()
	{
		CreateMap<User, UserViewModel>();

		CreateMap<UserViewModel, User>();
	}
}

