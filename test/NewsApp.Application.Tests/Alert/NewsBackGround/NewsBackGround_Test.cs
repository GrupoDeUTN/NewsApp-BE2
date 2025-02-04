using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NewsApp.EntityFrameworkCore;
using NewsApp.News;
using NewsApp.Notification;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Xunit;

namespace NewsApp.Alert.NewsBackGround
{
    public class NewsBackGround_Test : NewsAppApplicationTestBase
    {
        private readonly IRepository<AlertEntidad, int> _repository;
        private readonly IAlertManager _alertManager;
        private readonly IRepository<NotificationEntidad, int> _notificationRepository;
        private readonly INotificationAppService _notificationAppService;
        private readonly AlertAppService _alertAppService;
        private readonly IDbContextProvider<NewsAppDbContext> _dbContextProvider;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        // 🔹 Mock solo para AsyncSearch
        private readonly Mock<INewsAppService> _newsServiceMock;

        public NewsBackGround_Test()
        {
            // 🔹 Obtener servicios reales de la aplicación
            _repository = GetRequiredService<IRepository<AlertEntidad, int>>();
            _alertManager = GetRequiredService<IAlertManager>();
            _notificationRepository = GetRequiredService<IRepository<NotificationEntidad, int>>();
            _notificationAppService = GetRequiredService<INotificationAppService>();
            _userManager = GetRequiredService<UserManager<Volo.Abp.Identity.IdentityUser>>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<NewsAppDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();


            // 🔹 Crear el mock solo para `INewsAppService`
            _newsServiceMock = new Mock<INewsAppService>();

            // 🔹 Inyectar dependencias (reales + mock)
            _alertAppService = new AlertAppService(
                _repository,
                _userManager,
                _alertManager,
                _newsServiceMock.Object, // Aquí usamos el mock
                _notificationRepository,
                _notificationAppService
            );
        }

        [Fact]
        public async Task ProcessNewsAlertsAsync_ShouldPersistNotificationForMatchingAlerts()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // 🔹 Obtener alertas desde el repositorio real
                var user = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
                var alertasEntidad = await _repository.GetListAsync();
                var alertasDto = alertasEntidad.Select(a => new AlertDto { Id = a.Id, CadenaBusqueda = a.CadenaBusqueda, FechaCreacion = a.FechaCreacion }).ToList();

                // 🔹 Noticias simuladas para cada alerta
                var noticiasDeportes = new List<NewsDto>
                {
                    new NewsDto { Title = "Fútbol hoy", Description = "Resumen de la liga", Url = "http://sports.com/1", PublishedAt = DateTime.Now.AddDays(-1) }
                };


                // 🔹 Configurar el mock de `AsyncSearch`
                _newsServiceMock
                    .Setup(service => service.AsyncSearch(It.Is<string>(q => q.Contains("deportes")), It.IsAny<Guid>()))
                    .ReturnsAsync(noticiasDeportes);



                // 🔹 Ejecutar el método a probar
                await _alertAppService.ProcessNewsAlertsAsync(alertasDto);
                await uow.SaveChangesAsync();

                // 🔹 Verificar que se creó la notificación con las noticias adecuadas

                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var alertsInDb = await _repository.GetListAsync();

                var alertaDeportes = alertsInDb.Where(alert => alert.CadenaBusqueda == "deportes").FirstOrDefault();
                alertaDeportes.Notificaciones.ShouldNotBeNull();
                alertaDeportes.Notificaciones.Count.ShouldBe(1);

                var notific = alertaDeportes.Notificaciones.FirstOrDefault();
                notific.CantidadNoticiasNuevas.ShouldBe(1);
                notific.Leida.ShouldBe(false);



            }
        }
    }







}