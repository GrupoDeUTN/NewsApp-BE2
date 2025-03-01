﻿    using NewsApp.EntityFrameworkCore;
    using NewsApp.Themes;
    using NewsApp.News;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Volo.Abp.EntityFrameworkCore;
    using Volo.Abp.Uow;
    using Xunit;
    using NSubstitute;
    using Microsoft.Extensions.DependencyInjection;
    using Volo.Abp.Testing;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Domain.Repositories;



namespace NewsApp.Theme
    {
    public class ThemeAppService_Test : NewsAppApplicationTestBase
    {
        private readonly IThemeAppService _themeAppService;
        private readonly IDbContextProvider<NewsAppDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Themes.ThemeEntidad, int> _themeRepository;
        private readonly IRepository<NewsEntidad, int> _newsRepository;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;

        public ThemeAppService_Test()
        {
            _themeAppService = GetRequiredService<IThemeAppService>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<NewsAppDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            _themeRepository = GetRequiredService<IRepository<Themes.ThemeEntidad, int>>();
            _newsRepository = GetRequiredService<IRepository<NewsEntidad, int>>();
            _userManager = GetRequiredService<UserManager<Volo.Abp.Identity.IdentityUser>>();
        }



        [Fact]
        public async Task Should_Get_All_Themes()
        {
            //Act
            var themes = await _themeAppService.GetThemesAsync();
            //Assert
            themes.ShouldNotBeNull();
            themes.Count.ShouldBeGreaterThan(1);
        }


        [Fact]
        public async Task Should_Create_Theme()
        {
            //Arrange            
            var input = new CretateThemeDto { Name = "nuevo tema" };
            //Act
            var newTheme = await _themeAppService.CreateAsync(input);
            //Assert
            // Se verifican los datos devueltos por el servicio
            newTheme.ShouldNotBeNull();
            newTheme.Id.ShouldBePositive();
            // se verifican los datos persistidos por el servicio
            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                dbContext.Themes.FirstOrDefault(t => t.Id == newTheme.Id).ShouldNotBeNull();
                dbContext.Themes.FirstOrDefault(t => t.Id == newTheme.Id).Name.ShouldBe(input.Name);
            }
        }

        [Fact]
        public async Task Should_Update_Theme()
        {
            //Arrange            
            var input = new CretateThemeDto { Name = "nuevo tema", Id = 1 };
            //Act
            var newTheme = await _themeAppService.CreateAsync(input);
            //Assert
            // Se verifican los datos devueltos por el servicio
            newTheme.ShouldNotBeNull();
            newTheme.Id.ShouldBePositive();
            // se verifican los datos persistidos por el servicio
            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                dbContext.Themes.FirstOrDefault(t => t.Id == newTheme.Id).ShouldNotBeNull();
                dbContext.Themes.FirstOrDefault(t => t.Id == newTheme.Id).Name.ShouldBe(input.Name);
            }
        }

        [Fact]
        public async Task Should_Create_Child_Theme()
        {
            //Arrange            
            var input = new CretateThemeDto { Name = "nuevo tema hijo", ParentId = 1 };
            //Act
            var newTheme = await _themeAppService.CreateAsync(input);
            //Assert
            // Se verifican los datos devueltos por el servicio
            newTheme.ShouldNotBeNull();
            newTheme.Id.ShouldBePositive();
            // se verifican los datos persistidos por el servicio
            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                dbContext.Themes.FirstOrDefault(t => t.Id == newTheme.Id).ShouldNotBeNull();
                dbContext.Themes.FirstOrDefault(t => t.Id == newTheme.Id).Name.ShouldBe(input.Name);
            }
        }

        [Fact]
        public async Task Should_Get_Theme_By_Id()
        {
            //Arrange
            var themeId = 1;

            //Act
            var theme = await _themeAppService.GetThemesAsync(themeId);

            //Assert
            theme.ShouldNotBeNull();
            theme.Id.ShouldBe(themeId);

            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var themeInDb = dbContext.Themes.FirstOrDefault(t => t.Id == themeId);
                themeInDb.ShouldNotBeNull();
                themeInDb.Id.ShouldBe(theme.Id);
                themeInDb.Name.ShouldBe(theme.Name);
            }
        }

        [Fact]
        public async Task Should_Delete_Theme()
        {
            //Arrange
            var themeToDelete = await _themeAppService.CreateAsync(
                new CretateThemeDto { Name = "tema para eliminar" }
            );

            //Act
            await _themeAppService.DeleteAsync(themeToDelete.Id);

            //Assert
            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var deletedTheme = dbContext.Themes.FirstOrDefault(t => t.Id == themeToDelete.Id);
                deletedTheme.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_Add_News_To_Theme()
        {
            //Arrage
            var themeId = 1;
            var busqueda = "Milei";
            var tituloEsperado = "Argentina's Milei welcomes Venezuelan opposition leader Gonzalez";


            // Act
            var noticia = await _themeAppService.AgregarNoticia(themeId, busqueda, tituloEsperado);

            // Assert
            noticia.ShouldNotBeNull();
            noticia.Id.ShouldBePositive();
            noticia.Title.ShouldBe(tituloEsperado);

            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var newInDb = dbContext.NewsEntidad.FirstOrDefault(n => n.Id == noticia.Id);
                newInDb.ShouldNotBeNull();
                newInDb.Id.ShouldBe(noticia.Id);
                newInDb.Title.ShouldBe(tituloEsperado);
                newInDb.ThemeId.ShouldBe(themeId);

            }

        }


        [Fact]
        public async Task Should_Delete_New_From_Theme()
        {

            
            using (var uow = _unitOfWorkManager.Begin())
            {


                // Arrange
                //Datos persistidos en los datos de prueba
                var idNoticia = 1;
                var idTema = 2;
                var user = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
                var theme = await _themeRepository.GetAsync(idTema, includeDetails: true);
                var noticia = await _newsRepository.GetAsync(idNoticia, includeDetails: true);


                // Act
                await _themeAppService.DeleteNewFromTheme(idNoticia, idTema);


                // Assert
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var newInDb = dbContext.NewsEntidad.FirstOrDefault(n => n.Id == noticia.Id);
                newInDb.ShouldBeNull();
            }

        }

 





    }

}


