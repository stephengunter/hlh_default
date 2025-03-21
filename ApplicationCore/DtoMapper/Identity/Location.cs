using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using AutoMapper;

namespace ApplicationCore.DtoMapper.Identity;

public class LocationMappingProfile : Profile
{
	public LocationMappingProfile()
	{
		CreateMap<Location, LocationViewModel>();

		CreateMap<LocationViewModel, Location>();
   }
}
