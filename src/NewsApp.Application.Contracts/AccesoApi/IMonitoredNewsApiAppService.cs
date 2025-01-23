using System;
using System.Threading.Tasks;
using NewsApp.AccesoApi;
using System.Collections.Generic;

namespace NewsApp
{
    public interface IMonitoredNewsApiAppService
    {
        // Obtiene el tiempo promedio general de todas las consultas
        Task<TimeSpan> GetTiempoPromedioConsultas();

        // Obtiene el porcentaje de consultas con error
        Task<double> GetPorcentajeErrores();

        // Obtiene estadísticas básicas por usuario (total consultas y tiempo promedio)
        Task<List<EstadisticasUsuarioDto>> GetEstadisticasBasicasPorUsuario();
    }


}

