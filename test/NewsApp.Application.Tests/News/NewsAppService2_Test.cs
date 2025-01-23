using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NewsApp.News
{
    public class NewsAppService_Test2 : NewsAppApplicationTestBase
    {
        private readonly INewsAppService _newsAppService;

        public NewsAppService_Test2()
        {
            _newsAppService = GetRequiredService<INewsAppService>();
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


    }
}
