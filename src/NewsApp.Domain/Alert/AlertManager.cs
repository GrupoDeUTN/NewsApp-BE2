using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace NewsApp.Alert
{
    public class AlertManager: DomainService
    {
        private readonly IRepository<AlertEntidad, int> _repository;
        public AlertManager(IRepository<AlertEntidad, int> repository)
        {
            _repository = repository;
        }


        public async Task<int> CreateAsyncAlert(AlertEntidad alertEntidad)
        {
            // Inserta la entidad y guarda cambios en la base de datos
            await _repository.InsertAsync(alertEntidad, autoSave: true); // Asegúrate de que autoSave esté en true

            // Verifica que el ID fue generado correctamente
            if (alertEntidad.Id == 0)
            {
                throw new Exception("Error al guardar la alerta, el ID generado es 0.");
            }

            // Retorna el ID de la entidad que fue generada automáticamente
            return alertEntidad.Id;
        }

    }

}

