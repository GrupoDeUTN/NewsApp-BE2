using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsApp.News
{
    public class NewsAppService : NewsAppAppService, INewsAppService
    {
        private readonly INewsService _newsService;

        public NewsAppService(INewsService newsService)
        {
            _newsService = newsService;
        }
        public async Task<ICollection<NewsDto>> Search(string query)
        {
            //TODO: falta registrar los tiempos de acceso de la API
            var news = await _newsService.GetNewsAsync(query);

            return ObjectMapper.Map<ICollection<ArticleDto>, ICollection<NewsDto>>(news);            
        }

        public NewsDto SeleccionarNewsDeBusqueda(ICollection<NewsDto> resultados, string author)
        {
            var newBusqueda = resultados.FirstOrDefault(x => x.Author == author);
            return newBusqueda;
        }

        //public async Task<NewsDto> AgregarNoticia2(int idTema, string busqueda, string autor)
        //{
        //    var resultados = await Search(busqueda);
        //    var noticia = SeleccionarNewsDeBusqueda(resultados, autor);
        //    var noticiaEntidad = ObjectMapper.Map<NewsDto, NewsEntidad>(noticia);
        //    noticiaEntidad.ThemeId = idTema;

        //   await _newsManager.CreateAsyncNews(noticiaEntidad);

        //    return noticia;

        //}

    }
}
