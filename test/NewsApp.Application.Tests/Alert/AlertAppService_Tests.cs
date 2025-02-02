using System;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using NewsApp.Alert;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace NewsApp.Alert
{
    public class AlertAppService_Tests : NewsAppApplicationTestBase
    {
        private readonly IAlertAppService _alertAppService;
        private readonly IRepository<AlertEntidad, int> _alertRepository;
        private readonly IAlertManager _alertManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AlertAppService_Tests()
        {
            _alertAppService = GetRequiredService<IAlertAppService>();
            _alertRepository = GetRequiredService<IRepository<AlertEntidad, int>>();
            _alertManager = GetRequiredService<IAlertManager>();
            _userManager = GetRequiredService<UserManager<IdentityUser>>();
        }

        [Fact]
        public async Task Should_Create_Alert()
        {
            // Arrange
            var input = new CreateAlertDto
            {
                FechaCreacion = DateTime.Now,
                Activa = true,
                CadenaBusqueda = "test search"
            };

            // Act
            var result = await _alertAppService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.CadenaBusqueda.ShouldBe(input.CadenaBusqueda);
            result.Activa.ShouldBe(input.Activa);
            result.FechaCreacion.ShouldBe(input.FechaCreacion);
        }

        [Fact]
        public async Task Should_Get_All_Alerts_For_Current_User()
        {
            // Arrange
            var currentUser = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");

            // Crear algunas alertas de prueba
            await _alertManager.CreateAsync(DateTime.Now, true, "test1", currentUser.Id);
            await _alertManager.CreateAsync(DateTime.Now, true, "test2", currentUser.Id);

            // Act
            var alerts = await _alertAppService.GetAlertsAsync();

            // Assert
            alerts.ShouldNotBeNull();
            alerts.Count.ShouldBeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task Should_Get_Alert_By_Id()
        {
            // Arrange
            var currentUser = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var alert = await _alertManager.CreateAsync(DateTime.Now, true, "test", currentUser.Id);

            // Act
            var result = await _alertAppService.GetAlertAsync(alert.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(alert.Id);
            result.CadenaBusqueda.ShouldBe(alert.CadenaBusqueda);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Alert_Not_Found()
        {
            // Arrange
            var nonExistentId = 99999;

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () =>
                await _alertAppService.GetAlertAsync(nonExistentId)
            );
        }

        [Fact]
        public async Task Should_Delete_Alert()
        {
            // Arrange
            var currentUser = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var alert = await _alertManager.CreateAsync(DateTime.Now, true, "test", currentUser.Id);

            // Act
            await _alertAppService.DeleteAsync(alert.Id);

            // Assert
            var deletedAlert = await _alertRepository.FindAsync(alert.Id);
            deletedAlert.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Deleting_Nonexistent_Alert()
        {
            // Arrange
            var nonExistentId = 99999;

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () =>
                await _alertAppService.DeleteAsync(nonExistentId)
            );
        }
    }
}