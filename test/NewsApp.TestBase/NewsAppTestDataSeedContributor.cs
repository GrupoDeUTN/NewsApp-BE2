using NewsApp.Alert;
using NewsApp.News;
using NewsApp.Notification;
using NewsApp.Themes;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using NewsApp.AccesoAPI;
using System.Collections.Generic;

namespace NewsApp;

public class NewsAppTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Theme, int> _themeRepository;
    private readonly IRepository<NewsEntidad, int> _newRepository;
    private readonly IRepository<AlertEntidad, int> _alertRepository;
    private readonly IRepository<NotificationEntidad, int> _notificationRepository;

    private readonly IdentityUserManager _identityUserManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRepository<AccesoApiEntidad, int> _accesoApiEntidadRepository;
    public NewsAppTestDataSeedContributor(IRepository<Theme, int> themeRepository, IdentityUserManager identityUserManager,
        IRepository<NewsEntidad, int> newRepository, IRepository<AlertEntidad, int> alertRepository,
        IRepository<NotificationEntidad, int> notificationRepository, IUnitOfWorkManager unitOfWorkManager, IRepository<AccesoApiEntidad, int> accesoApiEntidadRepository)
    {
        _themeRepository = themeRepository;
        _identityUserManager = identityUserManager;
        _newRepository = newRepository;
        _alertRepository = alertRepository;
        _notificationRepository = notificationRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _accesoApiEntidadRepository = accesoApiEntidadRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // USUARIO       
        IdentityUser identityUser1 = new IdentityUser(Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d"), "admin", "admin@abp.io");
        await _identityUserManager.CreateAsync(identityUser1, "1q2w3E*");


        // ALERTAS 

        await _alertRepository.InsertAsync(new AlertEntidad()
        {
            Activa = true,
            CadenaBusqueda = "deportes",
            UserId = identityUser1.Id,
            Notificaciones = new List<NotificationEntidad>(),
            FechaCreacion = DateTime.Now.AddDays(-10)

        });





        // TEMAS: Guardamos los temas y forzamos el guardado en la BD antes de insertar noticias
        var theme1 = await _themeRepository.InsertAsync(new Theme { Name = "Primer tema", User = identityUser1, UserId = identityUser1.Id });
        var theme2 = await _themeRepository.InsertAsync(new Theme { Name = "Segundo tema", User = identityUser1, UserId = identityUser1.Id });
        var theme3 = await _themeRepository.InsertAsync(new Theme { Name = "Tercer tema", User = identityUser1, UserId = identityUser1.Id });

        // NOTICIAS: Ahora que los temas existen en la BD, podemos insertar noticias con ThemeId válido
        var new1 = await _newRepository.InsertAsync(new NewsEntidad { Title = "Primer noticia", ThemeId = 2 });



        // Añadir la noticia al tema
        theme2.listNews.Add(new1);









        // Accesos a la API

        await _accesoApiEntidadRepository.InsertAsync(new AccesoApiEntidad()
        {
            UserId = identityUser1.Id,
            TiempoInicio = new DateTime(2025, 1, 27, 10, 30, 0),
            TiempoFin = new DateTime(2025, 1, 27, 10, 33, 0),
            TiempoTotal = TimeSpan.FromMinutes(3),
            ErrorMessage = "This shouldn't happen, and if it does then it's our fault, not yours. Try the request again shortly."
        });

        await _accesoApiEntidadRepository.InsertAsync(new AccesoApiEntidad()
        {
            UserId = identityUser1.Id,
            TiempoInicio = new DateTime(2025, 1, 29, 10, 30, 0),
            TiempoFin = new DateTime(2025, 1, 29, 10, 33, 0),
            TiempoTotal = TimeSpan.FromMinutes(3),
            ErrorMessage = "You have requested a source which does not exist."
        });

        await _accesoApiEntidadRepository.InsertAsync(new AccesoApiEntidad()
        {
            UserId = identityUser1.Id,
            TiempoInicio = new DateTime(2025, 1, 30, 10, 30, 0),
            TiempoFin = new DateTime(2025, 1, 30, 10, 33, 0),
            TiempoTotal = TimeSpan.FromMinutes(5),
            ErrorMessage = null
        });

        await _accesoApiEntidadRepository.InsertAsync(new AccesoApiEntidad()
        {
            UserId = identityUser1.Id,
            TiempoInicio = new DateTime(2025, 1, 28, 9, 15, 0),
            TiempoFin = new DateTime(2025, 1, 28, 9, 20, 0),
            TiempoTotal = TimeSpan.FromMinutes(5),
            ErrorMessage = null
        });


    }
}


