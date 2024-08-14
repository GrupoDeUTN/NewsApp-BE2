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
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly ThemeManager _themeManager;
        private readonly INewsAppService _newsAppService;
        private readonly NewsManager _newsManager;

        public ThemeAppService(IRepository<Theme, int> repository, UserManager<Volo.Abp.Identity.IdentityUser> userManager, ThemeManager themeManager,
            INewsAppService newsAppService, NewsManager newsManager)
        {
            _repository = repository;
            _userManager = userManager;
            _themeManager = themeManager;
            _newsAppService = newsAppService;
            _newsManager = newsManager;
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
            await DeleteThemeRecursively(theme);
        }




        private async Task DeleteThemeRecursively(Theme theme)
        {
            // Primero, eliminar todas las noticias asociadas al tema actual
            if (theme.listNews != null && theme.listNews.Any())
            {
                await DeleteNewsFromTheme(theme);
            }

            // Luego, proceder con la eliminación recursiva de los temas hijos
            if (theme.Themes != null && theme.Themes.Any())
            {
                foreach (var childTheme in theme.Themes.ToList())
                {
                    await DeleteThemeRecursively(childTheme);
                }
            }

            // Finalmente, eliminar el tema
            await _repository.DeleteAsync(theme, autoSave: true);
        }

        private async Task DeleteNewsFromTheme(Theme theme)
        {

            if (theme.listNews != null && theme.listNews.Any())
            {
                foreach (var news in theme.listNews.ToList())
                {
                    // Aquí podrías manejar la eliminación de la noticia según sea necesario
                    // por ejemplo, eliminar de un repositorio de noticias si existe o simplemente eliminarla de la lista.
                    theme.listNews.Remove(news);

                }
            }

            // Actualizar el tema en el repositorio para reflejar los cambios
            await _repository.UpdateAsync(theme, autoSave: true);
        }

        //probando con autor ---> 3010, Argentina, Steven Levy
        public async Task<NewsDto> AgregarNoticia(int idTema, string busqueda, string autor)

        {
            //Obtener el tema 
            var userGuid = CurrentUser.Id.GetValueOrDefault();

            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var queryable = await _repository.WithDetailsAsync(x => x.Themes);

            var query = queryable.Where(x => x.Id == idTema && x.User == identityUser);

            var theme = await AsyncExecuter.FirstOrDefaultAsync(query);

            //Obtener noticia
            var resultados = await _newsAppService.Search(busqueda);

            var noticia = _newsAppService.SeleccionarNewsDeBusqueda(resultados, autor);

            var noticiaEntidad = ObjectMapper.Map<NewsDto, NewsEntidad>(noticia);

            //Agregar noticia al tema

            noticiaEntidad.ThemeId = idTema;

           var IdNoticia = await _newsManager.CreateAsyncNews(noticiaEntidad);

          // await  _themeManager.AddNewAsync(IdNoticia, theme);

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


        /* Ola mi loko, lo arreglaste capo, aca van algunos detalles craizis para tener en cuenta
            1_ El id que llega de la api casi siempre es 0 el mapeo de todas formas le asigna un id diferente 
            2_ El dto que estoy devolviendo es antes del mapeo, osea va a devolver un id 0, ¿Deberìa devolver la noticia post-mapeo?
            3_ Esto de agregar noticias deberìa ir en este servicio? o tendrìa que ir con los servicios de NewsAppService 
        
        
        Me quede en el metodo 5. "Crear una alerta de nuevas noticias a partir de un texto de búsqueda"
          Creo que esto sale facil, cuento la catidad de resultados que arroja una busqueda, si al realizar una busqueda es mayor a la ultima busqueda realizada hay nuevas noticias
          .... de todas formas esto me pide CREAR LA ALERTA, no la logica de cuando pasa esto. Osea :
                  existe el metodo "ExisteNuevaNoticia" que es un booleano no me importa como esta definida esta logica
            Lo que yo tengo que pensar es como se va a crear la alerta
                Voy a necesitar una entidad que guarde las alertas, creo, 
                Alerta 
                       fecha de creaciòn:
                       texto de busqueda:
                       isActive:
                       ultimaFechaDeBusqueda:
                       cantidadDeNuevasNoticias
                   
                       intervalo de repeticion: (me avisa cada cuanto tiene que aprecer esta alerta) 


        Los siguientes metodos que se relacionan con este punto son:

            6. Obtener la informacion de notificaciones de las alertas del usuario para el área de
               notificación. ( Este método no realiza la busqueda en la API, solamente deberá
               devolver la información persistida)

            7. Ejecucion asincrónica que busque los textos de las alertas en la API y persista la
            información de las notificaciones. Opcional envío de mail.


        POR ULTIMO vieja, no arreglaste la parte de los privilegios del usuario protegidos esta andando porque lo borraste, tenes que resolvver eso, y ademas relacionar 
        los usuarios con las carpetas. O ver que verga hizo el profe para que ande [RESULESTO PONELE] 
                       */



    }
}
