using NewsApp.Alert;
using NewsApp.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace NewsApp.Notification
{
    public interface INotificationManager : IDomainService
    {

        Task<NotificationEntidad> CrearNotificacion(AlertEntidad alerta, ICollection<NewsDto> noticias);


    }
}