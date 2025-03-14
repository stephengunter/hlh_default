using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper.IT;

public class ItemMappingProfile : Profile
{
	public ItemMappingProfile()
	{
		CreateMap<Item, ItemViewModel>();

		CreateMap<ItemViewModel, Item>();
	}
}

