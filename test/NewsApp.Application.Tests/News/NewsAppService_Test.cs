using Microsoft.AspNetCore.Identity;
using Moq;
using NewsApp.AccesoAPI;
using NewsApp.Notification;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace NewsApp.News
{
    public class NewsAppService_Test2 : NewsAppApplicationTestBase
    {
        private readonly INewsAppService _newsAppService;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly IRepository<AccesoApiEntidad, int> _accesoApiLogger;


        public NewsAppService_Test2()
        {
            _newsAppService = GetRequiredService<INewsAppService>();
            _userManager = GetRequiredService<UserManager<Volo.Abp.Identity.IdentityUser>>();
            _accesoApiLogger = GetRequiredService<IRepository<AccesoApiEntidad, int>>();
        }

        [Fact]
        public async Task Should_Search_News()
        {
            // Arrange
            var query = "Apple";

            // Act
            var news = await _newsAppService.Search(query);

            // Assert
            news.ShouldNotBeNull();
            news.Count.ShouldBeGreaterThan(1);
        }


        [Fact]
        public void Should_Select_News_From_Search_Results()
        {
            // Arrange
            var resultados = new List<NewsDto>
            {
                new NewsDto { Title = "Apple releases new iPhone", Description = "New iPhone is out!" },
                new NewsDto { Title = "Apple stock rises", Description = "Apple stock hits all-time high." }
            };
            var titulo = "Apple releases new iPhone";

            // Act
            var result = _newsAppService.SeleccionarNewsDeBusqueda(resultados, titulo);

            // Assert
            result.ShouldNotBeNull();
            result.Title.ShouldBe(titulo);
        }

        [Fact]
        public void Should_Throw_Exception_When_Selecting_Non_Existent_News()
        {
            // Arrange
            var resultados = new List<NewsDto>
            {
                new NewsDto { Title = "Apple releases new iPhone", Description = "New iPhone is out!" },
                new NewsDto { Title = "Apple stock rises", Description = "Apple stock hits all-time high." }
            };

            var titulo = "Non-existent title";

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                _newsAppService.SeleccionarNewsDeBusqueda(resultados, titulo));

            exception.Message.ShouldBe($"No se encontró una noticia con el título: {titulo}");
        }



        [Fact]
        public async Task Should_AsyncSearch_News()
        {
            // Arrange
            var query = "Apple";
            var user = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var accesosIniciales = _accesoApiLogger.GetCountAsync().Result;

            // Act
            var news = await _newsAppService.AsyncSearch(query, user.Id);

            // Assert
            news.ShouldNotBeNull();
            news.Count.ShouldBeGreaterThan(0);
            _accesoApiLogger.GetCountAsync().Result.ShouldBe(accesosIniciales + 1);
        }


    }
}
