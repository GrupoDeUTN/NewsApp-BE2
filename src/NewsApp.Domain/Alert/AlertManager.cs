using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace NewsApp.Alert
{
    public class AlertManager : DomainService, IAlertManager
    {
        private readonly IRepository<AlertEntidad, int> _repository;

        public AlertManager(IRepository<AlertEntidad, int> repository)
        {
            _repository = repository;
        }

        public async Task<AlertEntidad> CreateAsync(DateTime fechaCreacion, bool activa, string cadenaBusqueda, Guid userId)
        {
            var alert = new AlertEntidad
            {
                FechaCreacion = fechaCreacion,
                Activa = activa,
                CadenaBusqueda = cadenaBusqueda,
                UserId = userId
            };

            await _repository.InsertAsync(alert, autoSave: true);

            return alert;
        }
    }
}