using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NewsApp.News;
using NewsApp.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace NewsApp.Alert
{
    [Authorize]
    public class AlertAppService : NewsAppAppService, IAlertAppService
    {
        private readonly IRepository<AlertEntidad, int> _repository;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly IAlertManager _alertManager;
        private readonly INewsAppService _newsService;
        private readonly IRepository<NotificationEntidad, int> _notificationRepository;
        private readonly INotificationAppService _notificationAppService;

        public AlertAppService(
            IRepository<AlertEntidad, int> repository,
            UserManager<Volo.Abp.Identity.IdentityUser> userManager,
             IAlertManager alertManager,
             INewsAppService newsService,
             IRepository<NotificationEntidad, int> notificationRepository,
             INotificationAppService notificationAppService)
        {
            _repository = repository;
            _userManager = userManager;
            _alertManager = alertManager;
            _newsService = newsService;
            _notificationRepository = notificationRepository;
            _notificationAppService = notificationAppService;
        }



        public async Task<AlertDto> CreateAsync(CreateAlertDto input)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            // var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var createdAlert = await _alertManager.CreateAsync(
                input.FechaCreacion,
                input.Activa,
                input.CadenaBusqueda,
                userGuid
            );

            return ObjectMapper.Map<AlertEntidad, AlertDto>(createdAlert);
        }



        // Obtener todas las alertas asociadas al usuario actual
        public async Task<ICollection<AlertDto>> GetAlertsAsync()
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            // var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var alerts = await _repository.GetListAsync(alert => alert.UserId == userGuid);

            return ObjectMapper.Map<ICollection<AlertEntidad>, ICollection<AlertDto>>(alerts);
        }

        // Obtener una alerta específica asociada al usuario actual
        public async Task<AlertDto> GetAlertAsync(int id)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            //var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var alert = await _repository.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userGuid);

            if (alert == null)
            {
                throw new Exception("Alerta no encontrada o no pertenece al usuario actual.");
            }

            return ObjectMapper.Map<AlertEntidad, AlertDto>(alert);
        }



        // Eliminar una alerta
        public async Task DeleteAsync(int id)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            //var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var alert = await _repository.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userGuid);

            if (alert == null)
            {
                throw new Exception("Alerta no encontrada o no pertenece al usuario actual.");
            }

            await _repository.DeleteAsync(alert, autoSave: true);
        }


        [AllowAnonymous] // Permite acceso sin autenticación
        public async Task<ICollection<AlertDto>> GetAlertsActivasAsync()
        {
            var alerts = await _repository.GetListAsync(a => a.Activa == true);
            return ObjectMapper.Map<ICollection<AlertEntidad>, ICollection<AlertDto>>(alerts);
        }

        [AllowAnonymous] // Permite acceso sin autenticación
        public async Task<Guid> GetUserIdByAlertIdAsync(int alertId)
        {
            // Obtener la alerta por su ID
            var alert = await _repository.FirstOrDefaultAsync(a => a.Id == alertId);

            if (alert == null)
            {
                throw new Exception("Alerta no encontrada.");
            }

            // Devolver directamente el UserId de la alerta
            return alert.UserId;
        }


        [AllowAnonymous] // Solo este método es público y no requiere autenticación
        public async Task ProcessNewsAlertsAsync(ICollection<AlertDto> alertas)
        {
            foreach (var alerta in alertas)
            {
                var userId = await GetUserIdByAlertIdAsync(alerta.Id);
                var ultimaNotificacion = await ObtenerUltimaNotificacionAsync(alerta.Id);
                var noticias = await _newsService.AsyncSearch(alerta.CadenaBusqueda, userId);
                var noticiasNuevas = FiltrarNoticiasNuevas(alerta, ultimaNotificacion, noticias);

                if (noticiasNuevas.Any())
                {

                    await _notificationAppService.CrearNotificacionAsync(alerta.Id, noticiasNuevas);
                }
            }
        }




        private async Task<NotificationEntidad> ObtenerUltimaNotificacionAsync(int alertId)
        {
            var notificaciones = await _notificationRepository.GetListAsync(n => n.AlertId == alertId);
            return notificaciones.OrderByDescending(n => n.FechaEnvio).FirstOrDefault();
        }


        private ICollection<NewsDto> FiltrarNoticiasNuevas(AlertDto alerta, NotificationEntidad ultimaNotificacion, ICollection<NewsDto> noticias)
        {
            if (ultimaNotificacion == null)
            {
                return noticias.Where(n => n.PublishedAt > alerta.FechaCreacion).ToList();
            }
            else
            {
                return noticias.Where(n => n.PublishedAt > ultimaNotificacion.FechaEnvio).ToList();
            }
        }


    }
}
