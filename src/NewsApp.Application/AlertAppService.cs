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

        public AlertAppService(
            IRepository<AlertEntidad, int> repository,
            UserManager<Volo.Abp.Identity.IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
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

        // Crear una nueva alerta
        public async Task<AlertDto> CreateAsync(AlertDto input)
        {
            var userGuid = CurrentUser.Id.GetValueOrDefault();
            var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());

            // Truncar la fecha para conservar solo día, mes y año
            var fechaCreacion = DateTime.UtcNow.Date;

            // Verificar si el ID ya existe en la base de datos
            var existingAlert = await _repository.FirstOrDefaultAsync(x => x.Id == input.Id);
            if (existingAlert != null)
            {
                throw new Exception($"Una alerta con el ID {input.Id} ya existe.");
            }

            var alert = new AlertEntidad
            {
                FechaCreacion = fechaCreacion,
                Activa = input.Activa,
                CadenaBusqueda = input.CadenaBusqueda,
                UserId = identityUser.Id
            };

            var createdAlert = await _repository.InsertAsync(alert, autoSave: true);

            return ObjectMapper.Map<AlertEntidad, AlertDto>(createdAlert);
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
