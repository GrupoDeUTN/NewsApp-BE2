using System;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using NewsApp.Notification;
using NewsApp.Alert;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using NewsApp.News;

namespace NewsApp.Notification
{
    public class NotificationAppService_Tests : NewsAppApplicationTestBase
    {
        private readonly INotificationAppService _notificationAppService;
        private readonly IRepository<NotificationEntidad, int> _notificationRepository;
        private readonly IRepository<AlertEntidad, int> _alertRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationAppService_Tests()
        {
            _notificationAppService = GetRequiredService<INotificationAppService>();
            _notificationRepository = GetRequiredService<IRepository<NotificationEntidad, int>>();
            _alertRepository = GetRequiredService<IRepository<AlertEntidad, int>>();
            _userManager = GetRequiredService<UserManager<IdentityUser>>();
        }

        [Fact]
        public async Task Should_Get_Notifications_For_Current_User()
        {
            // Arrange
            var currentUser = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var alert = await _alertRepository.InsertAsync(new AlertEntidad
            {
                FechaCreacion = DateTime.Now,
                Activa = true,
                CadenaBusqueda = "news test",
                UserId = currentUser.Id
            }, autoSave: true);

            await _notificationRepository.InsertAsync(new NotificationEntidad
            {
                FechaEnvio = DateTime.Now,
                Leida = false,
                CadenaBusqueda = "news test",
                CantidadNoticiasNuevas = 5,
                AlertId = alert.Id
            }, autoSave: true);

            // Act
            var notifications = await _notificationAppService.GetNotificationsAsync();

            // Assert
            notifications.ShouldNotBeNull();
            notifications.Count.ShouldBeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public async Task Should_Delete_Notification()
        {
            // Arrange
            var currentUser = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var alert = await _alertRepository.InsertAsync(new AlertEntidad
            {
                FechaCreacion = DateTime.Now,
                Activa = true,
                CadenaBusqueda = "news test",
                UserId = currentUser.Id
            }, autoSave: true);

            var notification = await _notificationRepository.InsertAsync(new NotificationEntidad
            {
                FechaEnvio = DateTime.Now,
                Leida = false,
                CadenaBusqueda = "news test",
                CantidadNoticiasNuevas = 5,
                AlertId = alert.Id
            }, autoSave: true);

            // Act
            await _notificationAppService.DeleteAsync(notification.Id);

            // Assert
            var deletedNotification = await _notificationRepository.FindAsync(notification.Id);
            deletedNotification.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Deleting_Nonexistent_Notification()
        {
            // Arrange
            var nonExistentId = 99999;

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () =>
                await _notificationAppService.DeleteAsync(nonExistentId)
            );
        }


        [Fact]
        public async Task Should_Get_Notifications_By_AlertId()
        {
            // Arrange
            var currentUser = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var alert = new AlertEntidad
            {
                FechaCreacion = DateTime.Now,
                Activa = true,
                CadenaBusqueda = "test alert",
                UserId = currentUser.Id
            };
            await _alertRepository.InsertAsync(alert, autoSave: true);

            var notification1 = new NotificationEntidad
            {
                FechaEnvio = DateTime.Now,
                Leida = false,
                CadenaBusqueda = "test notification 1",
                CantidadNoticiasNuevas = 5,
                AlertId = alert.Id
            };
            var notification2 = new NotificationEntidad
            {
                FechaEnvio = DateTime.Now,
                Leida = false,
                CadenaBusqueda = "test notification 2",
                CantidadNoticiasNuevas = 3,
                AlertId = alert.Id
            };

            await _notificationRepository.InsertAsync(notification1, autoSave: true);
            await _notificationRepository.InsertAsync(notification2, autoSave: true);

            // Act
            var notifications = await _notificationAppService.GetNotificationsByAlertIdAsync(alert.Id);

            // Assert
            notifications.ShouldNotBeNull();
            notifications.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_Create_Notification()
        {
            // Arrange
            var currentUser = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var alert = await _alertRepository.InsertAsync(new AlertEntidad
            {
                FechaCreacion = DateTime.Now,
                Activa = true,
                CadenaBusqueda = "news test",
                UserId = currentUser.Id
            }, autoSave: true);

            var noticias = new List<NewsDto>
            {
                new NewsDto { Title = "Noticia 1", Content = "Contenido de prueba 1" },
                new NewsDto { Title = "Noticia 2", Content = "Contenido de prueba 2" }
            };

            // Act
            var notification = await _notificationAppService.CrearNotificacionAsync(alert.Id, noticias);

            // Assert
            notification.ShouldNotBeNull();
            notification.AlertId.ShouldBe(alert.Id);
            notification.CantidadNoticiasNuevas.ShouldBe(noticias.Count);
        }
    }
}
