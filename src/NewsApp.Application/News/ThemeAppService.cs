using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NewsApp.News;
using NewsApp.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace NewsApp.Themes
{
    [Authorize]
    public class ThemeAppService : NewsAppAppService, IThemeAppService
    {
        private readonly IRepository<Theme, int> _repository;
        private readonly IRepository<NewsEntidad, int> _repositoryNews;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly ThemeManager _themeManager;
        private readonly INewsAppService _newsAppService;
        private readonly NewsManager _newsManager;

        public ThemeAppService(IRepository<Theme, int> repository, UserManager<Volo.Abp.Identity.IdentityUser> userManager, ThemeManager themeManager,
            INewsAppService newsAppService, NewsManager newsManager, IRepository<NewsEntidad, int> repositoryNews)
        {
            _repository = repository;
            _userManager = userManager;
            _themeManager = themeManager;
            _newsAppService = newsAppService;
            _newsManager = newsManager;
            _repositoryNews = repositoryNews;   
        }

        public async Task<ICollection<ThemeDto>> GetThemesAsync()
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();

            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var themes = await _repository.GetListAsync(includeDetails: true ) ;
          
            var userThemes = themes.Where(x => x.User == identityUser && x.ThemeId == null ).ToList();

            return ObjectMapper.Map<ICollection<Theme>, ICollection<ThemeDto>>(userThemes);
        }

        public async Task<ThemeDto> GetThemesAsync(int id)

        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            // Obtener el tema
            var queryable = await _repository.WithDetailsAsync(x => x.listNews, x=>x.Themes); // Incluye las noticias del tema
            var query = queryable.Where(x => x.Id == id && x.User == identityUser);
            var theme = await AsyncExecuter.FirstOrDefaultAsync(query);

            return ObjectMapper.Map<Theme, ThemeDto>(theme);

        }

        public async Task<ThemeDto> CreateAsync(CretateThemeDto input)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();

            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var theme = await _themeManager.CreateAsyncOrUpdate(input.Id, input.Name, input.ParentId, identityUser);

            if (input.Id is null)
            {
                theme = await _repository.InsertAsync(theme, autoSave: true);
            }
            else
            {
                await _repository.UpdateAsync(theme, autoSave: true);
            }

            return ObjectMapper.Map<Theme, ThemeDto>(theme);
        }

        



        public async Task DeleteAsync(int id)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var queryable = await _repository.WithDetailsAsync(x => x.Themes, x => x.listNews); // Incluye tanto los temas hijos como las noticias
            var query = queryable.Where(x => x.Id == id && x.User == identityUser);
            var theme = await AsyncExecuter.FirstOrDefaultAsync(query);

            if (theme == null)
            {
                throw new Exception("Tema no encontrado o no pertenece al usuario actual.");
            }

            // Llamada recursiva para eliminar el tema y todo su contenido
            await _themeManager.DeleteThemeRecursively(theme);   ///--------> ACA
        }






        public async Task<NewsDto> AgregarNoticia(int idTema, string busqueda, string titulo)

        {
            //Obtener el tema 
            var userGuid = CurrentUser.Id.GetValueOrDefault();

            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var queryable = await _repository.WithDetailsAsync(x => x.Themes);

            var query = queryable.Where(x => x.Id == idTema && x.User == identityUser);

            var theme = await AsyncExecuter.FirstOrDefaultAsync(query);

            //Obtener noticia
            var resultados = await _newsAppService.Search(busqueda);

            var noticia = _newsAppService.SeleccionarNewsDeBusqueda(resultados, titulo);

            var noticiaEntidad = ObjectMapper.Map<NewsDto, NewsEntidad>(noticia);

            //Agregar noticia al tema

            noticiaEntidad.ThemeId = idTema;

            var IdNoticia = await _newsManager.CreateAsyncNews(noticiaEntidad);

            await _repository.UpdateAsync(theme, autoSave: true);

            noticia.Id = IdNoticia;

            return noticia;

        }



        public async Task DeleteNewFromTheme(int idNew, int idTheme)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            // Obtener el tema
            var queryable = await _repository.WithDetailsAsync(x => x.listNews); // Incluye las noticias del tema
            var query = queryable.Where(x => x.Id == idTheme && x.User == identityUser);
            var theme = await AsyncExecuter.FirstOrDefaultAsync(query);

            if (theme == null)
            {
                throw new Exception("Tema no encontrado o no pertenece al usuario actual.");
            }

            // Buscar la noticia dentro del tema
            var newsToDelete = theme.listNews.FirstOrDefault(news => news.Id == idNew);

            if (newsToDelete == null)
            {
                throw new Exception("Noticia no encontrada en el tema especificado.");
            }

            // Eliminar la noticia del tema
            theme.listNews.Remove(newsToDelete);

            // Guardar cambios en el repositorio
            await _repository.UpdateAsync(theme, autoSave: true);


        }



    }
}
