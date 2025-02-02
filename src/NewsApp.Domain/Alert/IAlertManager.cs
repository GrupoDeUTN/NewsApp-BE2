using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace NewsApp.Alert
{
    public interface IAlertManager : IDomainService
    {
        Task<AlertEntidad> CreateAsync(DateTime fechaCreacion, bool activa, string cadenaBusqueda, Guid userId);
    }
}