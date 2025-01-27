using NewsApp.EntityFrameworkCore;
using NewsApp.News;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Xunit;


namespace NewsApp.News
{
    public class NewsManager_Test : NewsAppDomainTestBase
    {
        private readonly NewsManager _newsManager;
        private readonly IDbContextProvider<NewsAppDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NewsManager_Test()
        {
            _newsManager = GetRequiredService<NewsManager>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<NewsAppDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Should_Create_News()
        {
            //Arrange
            var input = new NewsEntidad
            {
                Author = "Autor Test",
                Title = "Título de prueba",
                Description = "Descripción de prueba",
                Url = "https://test.com",
                UrlToImage = "https://test.com/image.jpg",
                PublishedAt = DateTime.Now,
                Content = "Contenido de prueba",
                ThemeId = 1
            };

            //Act
            var newNewsId = await _newsManager.CreateAsyncNews(input);

            //Assert
            // Verificamos que el ID generado sea válido
            newNewsId.ShouldBePositive();
            newNewsId.ShouldBeGreaterThan(0);


            // Verificamos los datos persistidos en la base de datos
            using (var uow = _unitOfWorkManager.Begin())
            {
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var savedNews = await dbContext.Set<NewsEntidad>().FindAsync(newNewsId);

                savedNews.ShouldNotBeNull();
                savedNews.Author.ShouldBe(input.Author);
                savedNews.Title.ShouldBe(input.Title);
                savedNews.Description.ShouldBe(input.Description);
                savedNews.Url.ShouldBe(input.Url);
                savedNews.UrlToImage.ShouldBe(input.UrlToImage);
                savedNews.Content.ShouldBe(input.Content);
                savedNews.ThemeId.ShouldBe(input.ThemeId);
            }
        }
    }
}