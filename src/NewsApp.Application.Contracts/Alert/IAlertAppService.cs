using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace NewsApp.Alert
{
    

        public interface IAlertAppService : IApplicationService
        {

            Task<AlertDto> CreateAsync(CreateAlertDto input);

            Task<ICollection<AlertDto>> GetAlertsAsync();

            Task<AlertDto> GetAlertAsync(int id);
                    
            Task DeleteAsync(int id);
            
            Task<ICollection<AlertDto>> GetAlertsActivasAsync();
               
            Task<Guid> GetUserIdByAlertIdAsync(int alertId);

            Task ProcessNewsAlertsAsync(ICollection<AlertDto> alertas);
        }


    
}
