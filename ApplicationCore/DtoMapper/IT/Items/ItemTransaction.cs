using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper.IT;

public class ItemTransactionMappingProfile : Profile
{
	public ItemTransactionMappingProfile()
	{
		CreateMap<ItemTransaction, ItemTransactionViewModel>();

		CreateMap<ItemTransactionViewModel, ItemTransaction>();
	}
}

