using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Identity;
using NewsApp.AccesoApi;
using NewsApp.AccesoAPI;


namespace NewsApp
{
    [Authorize]
    public class MonitoredNewsApiAppService : NewsAppAppService, IMonitoredNewsApiAppService
    {
        private readonly IRepository<AccesoApiEntidad, int> _accesoApiRepository;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;

        public MonitoredNewsApiAppService(
            IRepository<AccesoApiEntidad, int> accesoApiRepository,
            UserManager<Volo.Abp.Identity.IdentityUser> userManager)
        {
            _accesoApiRepository = accesoApiRepository;
            _userManager = userManager;
        }

        public async Task<TimeSpan> GetTiempoPromedioConsultas()
        {
            // Obtener todos los accesos
            var accesos = await _accesoApiRepository.GetListAsync();

            // Si no hay accesos, retornar TimeSpan.Zero
            if (!accesos.Any())
                return TimeSpan.Zero;

            // Calcular el promedio de los tiempos totales
            var ticksPromedio = (long)accesos.Average(x => x.TiempoTotal.Ticks);
            return TimeSpan.FromTicks(ticksPromedio);
        }

        public async Task<double> GetPorcentajeErrores()
        {
            // Obtener todos los accesos
            var accesos = await _accesoApiRepository.GetListAsync();

            // Si no hay accesos, retornar 0
            if (!accesos.Any())
                return 0;

            // Contar accesos con error y calcular porcentaje
            var totalAccesos = accesos.Count;
            var accesosConError = accesos.Count(x => !string.IsNullOrEmpty(x.ErrorMessage));

            return (double)accesosConError / totalAccesos * 100;
        }

        public async Task<List<EstadisticasUsuarioDto>> GetEstadisticasBasicasPorUsuario()
        {
            // Obtener todos los accesos
            var accesos = await _accesoApiRepository.GetListAsync();

            // Agrupar por usuario y calcular estadísticas
            var estadisticas = new List<EstadisticasUsuarioDto>();

            foreach (var grupoUsuario in accesos.GroupBy(x => x.UserId))
            {
                // Obtener información del usuario
                var user = await _userManager.FindByIdAsync(grupoUsuario.Key.ToString());
                if (user == null) continue;

                // Calcular estadísticas 
                var accesosUsuario = grupoUsuario.ToList();
                var tiempoPromedio = TimeSpan.FromTicks(
                    (long)accesosUsuario.Average(x => x.TiempoTotal.Ticks)
                );

                estadisticas.Add(new EstadisticasUsuarioDto
                {
                    UserName = user.UserName,
                    TotalConsultas = accesosUsuario.Count,
                    TiempoPromedioConsulta = tiempoPromedio
                });
            }

            return estadisticas;
        }



        public async Task<List<AccesoConUsuarioDto>> GetAccesosConUsuarioYFechaAsync()
        {
            // Obtener todos los accesos
            var accesos = await _accesoApiRepository.GetListAsync();

            // Crear una lista para almacenar los resultados
            var resultados = new List<AccesoConUsuarioDto>();

            // Iterar sobre los accesos y mapearlos con los usuarios
            foreach (var acceso in accesos)
            {
                var user = await _userManager.FindByIdAsync(acceso.UserId.ToString());
                if (user == null) continue;

                resultados.Add(new AccesoConUsuarioDto
                {
                    UserName = user.UserName,
                    FechaAcceso = acceso.TiempoInicio.Date // Solo la fecha, sin hora
                });
            }

            return resultados;
        }

    }
}


