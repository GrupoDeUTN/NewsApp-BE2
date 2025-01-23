using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsApp.Notification
{
    public interface INotificationAppService : IApplicationService
    {

      
            Task<ICollection<NotificationDto>> GetNotificationsAsync();

            Task<NotificationDto> GetNotificationAsync(int id);

            
        

    }
}
