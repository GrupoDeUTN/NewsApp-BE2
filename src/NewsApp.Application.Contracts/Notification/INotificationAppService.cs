using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsApp.Notification
{
    public interface INotificationAppService
    {

      
            Task<ICollection<NotificationDto>> GetNotificationsAsync();

            Task<NotificationDto> GetNotificationAsync(int id);

            Task<NotificationDto> CreateAsync(NotificationDto input);
        

    }
}
