using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper.IT;

public class PropertyMappingProfile : Profile
{
	public PropertyMappingProfile()
	{
		CreateMap<Property, PropertyViewModel>();

		CreateMap<PropertyViewModel, Property>();
	}
}

