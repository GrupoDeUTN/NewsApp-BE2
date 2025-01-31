using NewsApp.Alert;
using NewsApp.News;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace NewsApp.Notification
{
    public class NotificationManager : INotificationManager
    {
        private readonly IRepository<NotificationEntidad, int> _notificationRepository;

        public NotificationManager(IRepository<NotificationEntidad, int> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task CrearNotificacion(AlertEntidad alerta, ICollection<ArticleDto> noticias)
        {
            var notificacion = new NotificationEntidad
            {
                FechaEnvio = DateTime.UtcNow,
                Leida = false,
                CadenaBusqueda = alerta.CadenaBusqueda,
                CantidadNoticiasNuevas = noticias.Count,
                AlertId = alerta.Id,
            };

            await _notificationRepository.InsertAsync(notificacion);
        }
    }

    // ALERTA { NOTIF1, NOTIF2} ... .
   
}  