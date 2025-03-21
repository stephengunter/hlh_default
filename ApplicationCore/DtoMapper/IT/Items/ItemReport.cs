using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper.IT;

public class ItemReportMappingProfile : Profile
{
	public ItemReportMappingProfile()
	{
		CreateMap<ItemReport, ItemReportViewModel>();

		CreateMap<ItemReportViewModel, ItemReport>();
	}
}

