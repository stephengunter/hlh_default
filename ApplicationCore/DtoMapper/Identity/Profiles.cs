using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using AutoMapper;

namespace ApplicationCore.DtoMapper.Identity;

public class ProfilesMappingProfile : Profile
{
	public ProfilesMappingProfile()
	{
		CreateMap<Profiles, ProfilesViewModel>();

		CreateMap<ProfilesViewModel, Profiles>();
	}
}

