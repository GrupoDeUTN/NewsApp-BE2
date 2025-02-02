using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsApp.Notification
{
    public interface INotificationAppService : IApplicationService
    {
        Task DeleteAsync(int id);

        Task<ICollection<NotificationDto>> GetNotificationsAsync();
        Task<ICollection<NotificationDto>> GetNotificationsByAlertIdAsync(int alertId);

        //Task<NotificationDto> GetNotificationAsync(int id);




    }
}
