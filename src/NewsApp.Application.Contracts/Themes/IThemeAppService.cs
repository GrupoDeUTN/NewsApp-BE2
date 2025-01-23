using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using NewsApp.Themes;

namespace NewsApp.Themes
{
    public interface IThemeAppService : IApplicationService
    {
        Task<ICollection<ThemeDto>> GetThemesAsync();

        Task<ThemeDto> GetThemesAsync(int id);

        Task<ThemeDto> CreateAsync(CretateThemeDto input);

        Task DeleteAsync(int id);







    }
}


