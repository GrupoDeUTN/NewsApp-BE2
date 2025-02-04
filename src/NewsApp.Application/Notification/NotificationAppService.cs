using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NewsApp.Alert;
using NewsApp.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace NewsApp.Notification
{
    [Authorize]
    public class NotificationAppService : NewsAppAppService, INotificationAppService
    {
        private readonly IRepository<NotificationEntidad, int> _repository;
        private readonly IRepository<AlertEntidad, int> _alertRepository;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly INotificationManager _notificationManager;


        public NotificationAppService(
            IRepository<NotificationEntidad, int> repository,
            IRepository<AlertEntidad, int> alertRepository,
            UserManager<Volo.Abp.Identity.IdentityUser> userManager,
            INotificationManager notificationManager)
        {
            _repository = repository;
            _alertRepository = alertRepository;
            _userManager = userManager;
            _notificationManager = notificationManager;
        }

        // Obtener todas las notificaciones asociadas al usuario actual
        public async Task<ICollection<NotificationDto>> GetNotificationsAsync()
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var notifications = await _repository.GetListAsync(
                notification => notification.Alert != null && notification.Alert.UserId == identityUser.Id);

            return ObjectMapper.Map<ICollection<NotificationEntidad>, ICollection<NotificationDto>>(notifications);
        }





        // Eliminar una notificación
        public async Task DeleteAsync(int id)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var notification = await _repository.FirstOrDefaultAsync(
                n => n.Id == id && n.Alert != null && n.Alert.UserId == identityUser.Id);

            if (notification == null)
            {
                throw new Exception("Notificación no encontrada o no pertenece al usuario actual.");
            }

            await _repository.DeleteAsync(notification, autoSave: true);
        }

        public async Task<ICollection<NotificationDto>> GetNotificationsByAlertIdAsync(int alertId)
        {
            // Verificar si la alerta existe y pertenece al usuario actual
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var alert = await _alertRepository.FirstOrDefaultAsync(
                a => a.Id == alertId && a.UserId == identityUser.Id);

            if (alert == null)
            {
                throw new Exception("La alerta no existe o no pertenece al usuario actual.");
            }

            // Obtener todas las notificaciones asociadas a la alerta
            var notifications = await _repository.GetListAsync(n => n.AlertId == alertId);

            // Mapear entidades a DTOs
            return ObjectMapper.Map<ICollection<NotificationEntidad>, ICollection<NotificationDto>>(notifications);
        }



        [AllowAnonymous] // Permite acceso sin autenticación
        public async Task<NotificationDto> CrearNotificacionAsync(int idAlerta, ICollection<NewsDto> noticias)
        {
            // Obtener la alerta asociada al DTO
            var alerta = await _alertRepository.FirstOrDefaultAsync(a => a.Id == idAlerta);
            if (alerta == null)
            {
                throw new Exception("La alerta no existe.");
            }

            // Crear la notificación utilizando el NotificationManager

            var notificacion = await _notificationManager.CrearNotificacion(alerta, noticias);

            // Mapear la entidad a DTO y retornar
            return ObjectMapper.Map<NotificationEntidad, NotificationDto>(notificacion);
        }





    }
}
