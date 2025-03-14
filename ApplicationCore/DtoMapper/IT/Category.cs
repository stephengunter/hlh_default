using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper.IT;

public class CategoryMappingProfile : Profile
{
	public CategoryMappingProfile()
	{
		CreateMap<Category, CategoryViewModel>();

		CreateMap<CategoryViewModel, Category>();
	}
}

