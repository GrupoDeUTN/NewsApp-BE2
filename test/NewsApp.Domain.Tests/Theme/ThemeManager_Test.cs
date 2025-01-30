using NewsApp.EntityFrameworkCore;
using NewsApp.Themes;
using NewsApp.News;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;
using Volo.Abp.Identity;
using NSubstitute;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Users;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace NewsApp.Theme
{
    public class ThemeManager_Test : NewsAppDomainTestBase
    {
        private readonly IThemeManager _themeManager;
        private readonly IRepository<Themes.Theme, int> _themeRepository;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly IRepository<NewsEntidad, int> _newsRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDbContextProvider<NewsAppDbContext> _dbContextProvider;
        
        public ThemeManager_Test()
        {
            _themeRepository = GetRequiredService<IRepository<Themes.Theme, int>>();
            _newsRepository = GetRequiredService<IRepository<NewsEntidad, int>>();
            _themeManager = GetRequiredService<IThemeManager>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<NewsAppDbContext>>();
            _userManager = GetRequiredService<UserManager<Volo.Abp.Identity.IdentityUser>>();
        }

        [Fact]
        public async Task Should_Create_New_Theme()
        {
            // Arrange
            var name = "Nuevo Tema";
            var user = new IdentityUser(Guid.NewGuid(), "testuser", "test@test.com");

            // Act
            var theme = await _themeManager.CreateAsyncOrUpdate(null, name, null, user);

            // Assert
            theme.ShouldNotBeNull();
            theme.Name.ShouldBe(name);
            theme.User.ShouldBe(user);
        }

        [Fact]
        //Parece que la lógica de esta función no se encarga, ni de proporcionarle un Id al hijo, ni de asociarlo con el padre, eso lo hace la función en la capa servicios
        public async Task Should_Create_Child_Theme()
        {
            // Arrange
            var parentThemeId = 1;
            var childName = "Tema Hijo";
            var user = new IdentityUser(Guid.NewGuid(), "testuser", "test@test.com");

            // Act
            var childTheme = await _themeManager.CreateAsyncOrUpdate(null, childName, parentThemeId, user);

            // Assert
            childTheme.ShouldNotBeNull();
            childTheme.Name.ShouldBe(childName);
            childTheme.User.ShouldBe(user);

        }

        [Fact]
        public async Task Should_Update_Existing_Theme()
        {
            // Arrange
            var identityUser =  await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var newName = "Nuevo Tema";
            var themeId = 1;

            // Act
            var updatedTheme = await _themeManager.CreateAsyncOrUpdate(themeId, newName, null, identityUser);

            // Assert
            updatedTheme.ShouldNotBeNull();
            updatedTheme.Name.ShouldBe(newName);

        }




        [Fact]
        public async Task Should_Delete_News_From_Theme()
        {

            // Arrange
            var idTheme = 2;
            var idNoticia = 1;

            var theme = await _themeRepository.GetAsync(idTheme, includeDetails: true);
            var noticia = await _newsRepository.GetAsync(idNoticia, includeDetails: true);

            theme.listNews.Add(noticia);

            // Act
            await _themeManager.DeleteNewsFromTheme(theme);

            // Assert
            theme.listNews.Count.ShouldBe(0);
        }


        [Fact]
        public async Task Should_Delete_Theme_Recursivelye()
        {

            // Arrange

            //Datos comunes entre carpetas
            var user = new IdentityUser(Guid.NewGuid(), "testuser", "test@test.com");
            var idNoticia = 1;

            //Crear carpeta padre y añadir noticia
            var createTheme = await _themeManager.CreateAsyncOrUpdate(null, "TEMA PADRE", null, user);
            var theme = await _themeRepository.InsertAsync(createTheme, autoSave: true);
            var noticia = await _newsRepository.GetAsync(idNoticia, includeDetails: true);
            theme.listNews.Add(noticia);


            //Esta parte añade un tema hijo y le añade una notica    
            var themeId = theme.Id;
            var childTheme = await _themeManager.CreateAsyncOrUpdate(null, "TEMA HIJO", themeId, user);
            childTheme.ThemeId = themeId;   
            var childTheme_update = await _themeRepository.InsertAsync(childTheme, autoSave: true);
            childTheme_update.listNews.Add(noticia);
            theme.Themes.Add(childTheme_update);

            await _themeManager.DeleteThemeRecursively(theme);

            // Assert
            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var deletedTheme = dbContext.Themes.FirstOrDefault(t => t.Id == theme.Id);
                deletedTheme.ShouldBeNull();
            }
            
        }




  









    }
}