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
            var themes = await _repository.GetListAsync(includeDetails: true);

            return ObjectMapper.Map<ICollection<Theme>, ICollection<ThemeDto>>(themes);
        }

        public async Task<ThemeDto> GetThemesAsync(int id)
        {
            var queryable = await _repository.WithDetailsAsync(x => x.Themes);

            var query = queryable.Where(x => x.Id == id);

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
            // Primero, encuentra la entidad Theme basada en el id 
            var theme = await _repository.FindAsync(id);

            // Verifica si se encontró la entidad antes de intentar eliminarla.
            if (theme != null)
            {
                await _repository.DeleteAsync(theme, autoSave: true);
            }
        }

        public async Task<NewsDto> AgregarNoticia2(int idTema, string busqueda, string autor)
        {
            var resultados = await _newsAppService.Search(busqueda);
            var noticia = _newsAppService.SeleccionarNewsDeBusqueda(resultados, autor);
            var noticiaEntidad = ObjectMapper.Map<NewsDto, NewsEntidad>(noticia);
            noticiaEntidad.ThemeId = idTema;

            await _newsManager.CreateAsyncNews(noticiaEntidad);

            return noticia;

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
