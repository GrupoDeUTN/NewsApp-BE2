using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using NewsApp.Themes;
using NewsApp.News;

namespace NewsApp.Themes
{
    public interface IThemeAppService : IApplicationService
    {
        Task<ICollection<ThemeDto>> GetThemesAsync();

        Task<ThemeDto> GetThemesAsync(int id);

        Task<ThemeDto> CreateAsync(CretateThemeDto input);

        Task DeleteAsync(int id);

        Task<NewsDto> AgregarNoticia(int idTema, string busqueda, string titulo);

        Task DeleteNewFromTheme(int idNew, int idTheme);








    }
}


