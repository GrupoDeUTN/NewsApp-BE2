using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace NewsApp.AccesoAPI
{
    public interface IAccesoApiLogger : IDomainService
    {
        Task<AccesoApiEntidad> LogAccessAsync(Guid userId, DateTime inicio, DateTime fin, string? errorMessage = null);
    }
}
