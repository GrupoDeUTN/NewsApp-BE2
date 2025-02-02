using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Xunit;
using NewsApp.EntityFrameworkCore;
using NewsApp.AccesoAPI;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace NewsApp.AccesoApi
{
    public class AccesoApiLogger_Tests : NewsAppDomainTestBase
    {
        private readonly IAccesoApiLogger _accesoApiLogger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDbContextProvider<NewsAppDbContext> _dbContextProvider;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;

        public AccesoApiLogger_Tests()
        {
            _accesoApiLogger = GetRequiredService<IAccesoApiLogger>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<NewsAppDbContext>>();
            _userManager = GetRequiredService<UserManager<Volo.Abp.Identity.IdentityUser>>();
        }

        [Fact]
        public async Task Should_Log_Access_Successfully()
        {



            // Arrange
            var user = await _userManager.FindByIdAsync("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");
            var inicio = DateTime.Now;
            var fin = inicio.AddMinutes(1);
            var errorMessage = "Test error message";

            // Act
            var accesoCreado = await _accesoApiLogger.LogAccessAsync(user.Id, inicio, fin, errorMessage);


            // Assert
            using (var uow = _unitOfWorkManager.Begin())
            {

               
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var accesoLog = dbContext.AccesoApiEntidad.FirstOrDefault(x => x.Id == accesoCreado.Id);




                accesoLog.ShouldNotBeNull();
                accesoLog.UserId.ShouldBe(user.Id);
                accesoLog.TiempoInicio.ShouldBe(inicio);
                accesoLog.TiempoFin.ShouldBe(fin);
                accesoLog.TiempoTotal.ShouldBe(fin - inicio);
                accesoLog.ErrorMessage.ShouldBe(errorMessage);
            }
        }
    }
}