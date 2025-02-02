using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Xunit;
using NewsApp.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace NewsApp.Alert
{
    public class AlertManager_Tests : NewsAppDomainTestBase
    {
        private readonly IAlertManager _alertManager;
        private readonly IRepository<AlertEntidad, int> _alertRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDbContextProvider<NewsAppDbContext> _dbContextProvider;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;

        public AlertManager_Tests()
        {
            _alertManager = GetRequiredService<IAlertManager>();
            _alertRepository = GetRequiredService<IRepository<AlertEntidad, int>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<NewsAppDbContext>>();
            _userManager = GetRequiredService<UserManager<Volo.Abp.Identity.IdentityUser>>();
        }

        [Fact]
        public async Task Should_Create_New_Alert()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // Arrange

                var fechaCreacion = DateTime.Now;
                var activa = true;
                var cadenaBusqueda = "test search";
                var user = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");

                // Act
                var alertaCreada = await _alertManager.CreateAsync(
                    fechaCreacion,
                    activa,
                    cadenaBusqueda,
                    user.Id
                );

                // Assert
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var alertaEnDb = await _alertRepository.FindAsync(a => a.Id == alertaCreada.Id);

                alertaEnDb.ShouldNotBeNull();
                alertaEnDb.FechaCreacion.ShouldBe(fechaCreacion);
                alertaEnDb.Activa.ShouldBe(activa);
                alertaEnDb.CadenaBusqueda.ShouldBe(cadenaBusqueda);
                alertaEnDb.UserId.ShouldBe(user.Id);

                await uow.CompleteAsync();
            }
        }
    }
}