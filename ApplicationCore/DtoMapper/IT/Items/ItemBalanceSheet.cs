using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper.IT;

public class ItemBalanceSheetMappingProfile : Profile
{
	public ItemBalanceSheetMappingProfile()
	{
		CreateMap<ItemBalanceSheet, ItemBalanceSheetViewModel>();

		CreateMap<ItemBalanceSheetViewModel, ItemBalanceSheet>();
	}
}

