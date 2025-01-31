using System.Collections.Generic;
using Shouldly;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
//using NewsApp.Monitoreo;
using NSubstitute;
using Volo.Abp.Domain.Repositories;
using NewsApp.AccesoAPI;
using Autofac.Core;
using System;
using Microsoft.AspNetCore.Identity;
namespace NewsApp.Monitored
{
    public class MonitoredNewsApiAppService_Tests : NewsAppApplicationTestBase
    {
        //private readonly IRepository<AccesoApiEntidad, int> _accesoApiRepository;
        private readonly IMonitoredNewsApiAppService _service;
        public MonitoredNewsApiAppService_Tests()
        {
            _service = GetRequiredService<IMonitoredNewsApiAppService>();
        }




        [Fact]
        public async Task Should_Calculate_Percentage_Of_Errors_Correctly()
        {
            // Arrange
            var porcentajeErroresCorrecto = 50;
            // Act
            var porcentajeErrores = await _service.GetPorcentajeErrores();
            // Assert
            porcentajeErrores.ShouldBe(porcentajeErroresCorrecto);
        }
        [Fact]
        public async Task Should_Calculate_Average_Response_Time_Correctly()
        {
            // Arrange
            var tiempoPromedioCorrecto = TimeSpan.FromMinutes(4);
            // Act
            var tiempoPromedio = await _service.GetTiempoPromedioConsultas();
            //Assert
            tiempoPromedio.ShouldBe(tiempoPromedioCorrecto);
        }
        [Fact]
        public async Task Should_Get_Statistics_By_User()
        {
            // Arrange
            var usernameCorrecto = "admin";
            var totalConsultasCorrecto = 4;
            var tiempoPromedioConsultasCorrecto = TimeSpan.FromMinutes(4);
            // Act
            var estadisticas = await _service.GetEstadisticasBasicasPorUsuario();
            // Assert
            estadisticas.ShouldNotBeEmpty();
            estadisticas[0].UserName.ShouldBe(usernameCorrecto);
            estadisticas[0].TotalConsultas.ShouldBe(totalConsultasCorrecto);
            estadisticas[0].TiempoPromedioConsulta.ShouldBe(tiempoPromedioConsultasCorrecto);
        }
        [Fact]
        public async Task Should_Get_Accesses_With_User_And_Date()
        {
            // Arrange
            var usernameCorrecto = "admin";
            var fechaCorrecta = new DateTime(2025, 1, 27);
            // Act
            var accesos = await _service.GetAccesosConUsuarioYFechaAsync();
            // Assert
            accesos.ShouldNotBeEmpty();
            accesos[0].UserName.ShouldBe(usernameCorrecto);
            accesos[0].FechaAcceso.ShouldBe(fechaCorrecta);
        }
    }
}