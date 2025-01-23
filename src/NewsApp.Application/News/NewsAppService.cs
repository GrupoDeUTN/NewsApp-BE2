using NewsApp.AccesoAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace NewsApp.News
{
    public class NewsAppService : NewsAppAppService, INewsAppService
    {
        private readonly INewsApiService _newsService;
        private readonly IAccesoApiLogger _accesoApiLogger;
        private readonly ICurrentUser _currentUser;

        public NewsAppService(
            INewsApiService newsService,
            IAccesoApiLogger accesoApiLogger,
            ICurrentUser currentUser)
        {
            _newsService = newsService;
            _accesoApiLogger = accesoApiLogger;
            _currentUser = currentUser;
        }

        public async Task<ICollection<NewsDto>> Search(string query)
        {
            var inicio = DateTime.Now;
            var userId = _currentUser.Id.GetValueOrDefault();

            try
            {
                var news = await _newsService.GetNewsAsync(query);
                var newsDto = ObjectMapper.Map<ICollection<ArticleDto>, ICollection<NewsDto>>(news);

                await _accesoApiLogger.LogAccessAsync(userId, inicio, DateTime.Now);
                return newsDto;
            }
            catch (Exception ex)
            {
                await _accesoApiLogger.LogAccessAsync(userId, inicio, DateTime.Now, ex.Message);
                throw;
            }
        }

        public NewsDto SeleccionarNewsDeBusqueda(ICollection<NewsDto> resultados, string titulo)
        {
            var noticia = resultados.FirstOrDefault(x => x.Title == titulo);
            if (noticia == null)
            {
                throw new KeyNotFoundException($"No se encontró una noticia con el título: {titulo}");
            }

            return noticia;
        }
    }
}
