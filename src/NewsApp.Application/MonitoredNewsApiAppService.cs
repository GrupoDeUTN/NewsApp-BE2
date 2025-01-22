using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NewsApp.News;
using NewsApp.AccesoAPI;

namespace NewsApp
{
    [Authorize]
    public class MonitoredNewsApiAppService : NewsAppAppService
    {

    }

      

}