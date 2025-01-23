using System;
using System.Threading.Tasks;
using NewsApp.AccesoApi;
using System.Collections.Generic;
using Volo.Abp.Application.Services;

namespace NewsApp
{
    public interface IMonitoredNewsApiAppService : IApplicationService
    {
        // Obtiene el tiempo promedio general de todas las consultas
        Task<TimeSpan> GetTiempoPromedioConsultas();

        // Obtiene el porcentaje de consultas con error
        Task<double> GetPorcentajeErrores();

        // Obtiene estadísticas básicas por usuario (total consultas y tiempo promedio)
        Task<List<EstadisticasUsuarioDto>> GetEstadisticasBasicasPorUsuario();


        //Obtener todos los accesos de api dando el usuario y la fecha 
        Task<List<AccesoConUsuarioDto>> GetAccesosConUsuarioYFechaAsync();
    }


}

