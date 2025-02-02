using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace NewsApp.AccesoAPI
{
    public class AccesoApiLogger : IAccesoApiLogger
    {
        private readonly IRepository<AccesoApiEntidad, int> _repository;

        public AccesoApiLogger(IRepository<AccesoApiEntidad, int> repository)
        {
            _repository = repository;
        }

        public async Task<AccesoApiEntidad> LogAccessAsync(Guid userId, DateTime inicio, DateTime fin, string? errorMessage = null)
        {
            var accesoApi = new AccesoApiEntidad
            {
                UserId = userId,
                TiempoInicio = inicio,
                TiempoFin = fin,
                TiempoTotal = fin - inicio,
                ErrorMessage = errorMessage
            };

            await _repository.InsertAsync(accesoApi);

            return accesoApi;
        }
    }


}
