﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace NewsApp.Themes
{
    public interface IThemeManager : IDomainService
    {
  
        Task<ThemeEntidad> CreateAsyncOrUpdate(int? id, string name, int? parentId, Volo.Abp.Identity.IdentityUser identityUser);

        //Task AddNewAsync(int idNoticia, Theme theme);

        Task DeleteThemeRecursively(ThemeEntidad theme);

        Task DeleteNewsFromTheme(ThemeEntidad theme);
    }
}
