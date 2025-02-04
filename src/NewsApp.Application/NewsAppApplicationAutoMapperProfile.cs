using AutoMapper;
using NewsAPI.Models;
using NewsApp.News;
using NewsApp.Themes;
using NewsApp.User;
using NewsApp.Alert;
using Volo.Abp.Identity;
using NewsApp.Notification;
using NewsApp.AccesoAPI;
using NewsApp.AccesoApi;

namespace NewsApp;

public class NewsAppApplicationAutoMapperProfile : Profile
{
    public NewsAppApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<ThemeEntidad, ThemeDto>();
        CreateMap<IdentityUser, UserDto>();
        CreateMap<NewsDto, ArticleDto>().ReverseMap();
        CreateMap<NewsEntidad, NewsDto>().ReverseMap();
        CreateMap<AlertEntidad, AlertDto>().ReverseMap();
        CreateMap<NotificationEntidad, NotificationDto>();
        CreateMap<AccesoApiEntidad,AccesoApiDto>();
    }
}
