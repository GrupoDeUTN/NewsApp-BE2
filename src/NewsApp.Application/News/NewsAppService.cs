using Microsoft.AspNetCore.Identity;
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
        private readonly INewsService _newsService;
        private readonly IRepository<AccesoApiEntidad, int> _accesoApiRepository;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly ICurrentUser _currentUser;

        public NewsAppService(
            INewsService newsService,
            IRepository<AccesoApiEntidad, int> accesoApiRepository,
            UserManager<Volo.Abp.Identity.IdentityUser> userManager,
            ICurrentUser currentUser)
        {
            _newsService = newsService;
            _accesoApiRepository = accesoApiRepository;
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task<ICollection<NewsDto>> Search(string query)
        {
            // Iniciar registro de acceso
            var accesoApi = new AccesoApiEntidad
            {
                TiempoInicio = DateTime.Now,
                UserId = _currentUser.Id.GetValueOrDefault()
            };

            try
            {
                // Ejecutar la búsqueda
                var news = await _newsService.GetNewsAsync(query);
                var newsDto = ObjectMapper.Map<ICollection<ArticleDto>, ICollection<NewsDto>>(news);

                // Completar registro exitoso
                accesoApi.TiempoFin = DateTime.Now;
                accesoApi.TiempoTotal = accesoApi.TiempoFin - accesoApi.TiempoInicio;
                await _accesoApiRepository.InsertAsync(accesoApi);

                return newsDto;
            }
            catch (Exception ex)
            {
                // Registrar acceso con error
                accesoApi.TiempoFin = DateTime.Now;
                accesoApi.TiempoTotal = accesoApi.TiempoFin - accesoApi.TiempoInicio;
                accesoApi.ErrorMessage = ex.Message;
                await _accesoApiRepository.InsertAsync(accesoApi);

                throw; // Re-lanzar la excepción para mantener el flujo de error
            }
        }
   

    public NewsDto SeleccionarNewsDeBusqueda(ICollection<NewsDto> resultados, string titulo)
        {
            var newBusqueda = resultados.FirstOrDefault(x => x.Title == titulo);
            return newBusqueda;
        }



    }
}
