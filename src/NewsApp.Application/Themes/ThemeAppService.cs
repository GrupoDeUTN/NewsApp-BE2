﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace NewsApp.Themes
{
    public class ThemeAppService : NewsAppAppService, IThemeAppService 
    {
        private readonly IRepository<Theme, int> _repository;

        public ThemeAppService(IRepository<Theme, int> repository)            
        {
            _repository = repository;
        }

        public async Task<ICollection<ThemeDto>> GetThemesAsync()
        {
            var themes = await _repository.GetListAsync();

            return ObjectMapper.Map<ICollection<Theme>, ICollection<ThemeDto>>(themes);
        }

        public async Task<ThemeDto> GetThemesAsync(int id)
        {
            var theme = await _repository.GetAsync(id);

            return ObjectMapper.Map<Theme, ThemeDto>(theme);
        }
    }
}
