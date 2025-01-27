using NewsApp.EntityFrameworkCore;
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

namespace NewsApp.Theme
{
    public class ThemeAppService_Test : NewsAppApplicationTestBase
    {
        private readonly IThemeAppService _themeAppService;
        private readonly IDbContextProvider<NewsAppDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly INewsAppService _newsAppService;

        public ThemeAppService_Test()
        {
            _themeAppService = GetRequiredService<IThemeAppService>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<NewsAppDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            _newsAppService = GetRequiredService<INewsAppService>();
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
            //Arrange
            var theme = await _themeAppService.CreateAsync(
                new CretateThemeDto { Name = "tema con noticias" }
            );
            var searchQuery = "test news";
            var newsTitle = "Noticia de prueba";

            //Act
            var addedNews = await _themeAppService.AgregarNoticia(theme.Id, searchQuery, newsTitle);

            //Assert
            addedNews.ShouldNotBeNull();
            addedNews.Id.ShouldBePositive();
            addedNews.Title.ShouldBe(newsTitle);

            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var newsInDb = dbContext.Set<NewsEntidad>().FirstOrDefault(n => n.Id == addedNews.Id);
                newsInDb.ShouldNotBeNull();
                newsInDb.ThemeId.ShouldBe(theme.Id);
                newsInDb.Title.ShouldBe(newsTitle);
            }
        }

        [Fact]
        public async Task Should_Delete_News_From_Theme()
        {
            //Arrange
            var theme = await _themeAppService.CreateAsync(
                new CretateThemeDto { Name = "tema para eliminar noticia" }
            );
            var news = await _themeAppService.AgregarNoticia(theme.Id, "test", "Noticia para eliminar");

            //Act
            await _themeAppService.DeleteNewFromTheme(news.Id, theme.Id);

            //Assert
            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var newsInDb = dbContext.Set<NewsEntidad>()
                    .FirstOrDefault(n => n.Id == news.Id && n.ThemeId == theme.Id);
                newsInDb.ShouldBeNull();
            }
        }
    }
}