using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper.IT;

public class DeviceMappingProfile : Profile
{
	public DeviceMappingProfile()
	{
		CreateMap<Device, DeviceViewModel>();

		CreateMap<DeviceViewModel, Device>();
	}
}

