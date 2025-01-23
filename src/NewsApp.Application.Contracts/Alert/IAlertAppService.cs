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
            Task<ICollection<AlertDto>> GetAlertsAsync();

            Task<AlertDto> GetAlertAsync(int id);


            Task<AlertDto> CreateAsync(CreateAlertDto input);

            Task DeleteAsync(int id);


        }


    
}
