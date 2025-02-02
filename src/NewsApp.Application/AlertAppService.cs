using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NewsApp.Alert;
using NewsApp.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace NewsApp
{
    [Authorize]
    public class AlertAppService : NewsAppAppService, IAlertAppService
    {
        private readonly IRepository<AlertEntidad, int> _repository;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly IAlertManager _alertManager;


        public AlertAppService(
            IRepository<AlertEntidad, int> repository,
            UserManager<Volo.Abp.Identity.IdentityUser> userManager,
             IAlertManager alertManager)
        {
            _repository = repository;
            _userManager = userManager;
            _alertManager = alertManager;
        }



        public async Task<AlertDto> CreateAsync(CreateAlertDto input)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var createdAlert = await _alertManager.CreateAsync(
                input.FechaCreacion,
                input.Activa,
                input.CadenaBusqueda,
                identityUser.Id
            );

            return ObjectMapper.Map<AlertEntidad, AlertDto>(createdAlert);
        }
    


    // Obtener todas las alertas asociadas al usuario actual
    public async Task<ICollection<AlertDto>> GetAlertsAsync()
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var alerts = await _repository.GetListAsync(alert => alert.UserId == identityUser.Id);

            return ObjectMapper.Map<ICollection<AlertEntidad>, ICollection<AlertDto>>(alerts);
        }

        // Obtener una alerta específica asociada al usuario actual
        public async Task<AlertDto> GetAlertAsync(int id)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var alert = await _repository.FirstOrDefaultAsync(a => a.Id == id && a.UserId == identityUser.Id);

            if (alert == null)
            {
                throw new Exception("Alerta no encontrada o no pertenece al usuario actual.");
            }

            return ObjectMapper.Map<AlertEntidad, AlertDto>(alert);
        }



        // Eliminar una alerta
        public async Task DeleteAsync(int id)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            var alert = await _repository.FirstOrDefaultAsync(a => a.Id == id && a.UserId == identityUser.Id);

            if (alert == null)
            {
                throw new Exception("Alerta no encontrada o no pertenece al usuario actual.");
            }

            await _repository.DeleteAsync(alert, autoSave: true);
        }
    }
}
