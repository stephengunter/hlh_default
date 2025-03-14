using ApplicationCore.Views.Identity;
using ApplicationCore.Models.Identity;
using AutoMapper;
using Infrastructure.Views;

namespace ApplicationCore.Helpers.Identity;

public static class AppsHelpers
{
   public static AppViewModel MapViewModel(this App entity, IMapper mapper)
      => mapper.Map<AppViewModel>(entity);

   public static AppViewModel MapViewModel(this App entity, string src, IMapper mapper)
   { 
      var model = mapper.Map<AppViewModel>(entity);
      string url = entity.Url;
      if (!url.EndsWith("/")) url += "/";
      model.Url = $"{url}login?source={src}";
      return model;
   }

   public static List<AppViewModel> MapViewModelList(this IEnumerable<App> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static List<AppViewModel> MapViewModelList(this IEnumerable<App> entities, string src, IMapper mapper)
      => entities.Select(item => MapViewModel(item, src, mapper)).ToList();

   public static BaseOption<int> MapOption(this App entity)
      => new BaseOption<int>(entity.Id, entity.Name);

   public static List<BaseOption<int>> MapOptionList(this IEnumerable<App> entities)
      => entities.Select(item => MapOption(item)).ToList();
}