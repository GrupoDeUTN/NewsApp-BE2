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

namespace NewsApp;

public class NewsAppTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Theme, int> _themeRepository;
    private readonly IRepository<NewsEntidad, int> _newRepository;
    private readonly IRepository<AlertEntidad, int> _alertRepository;
    private readonly IRepository<NotificationEntidad, int> _notificationRepository;

    private readonly IdentityUserManager _identityUserManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public NewsAppTestDataSeedContributor(IRepository<Theme, int> themeRepository, IdentityUserManager identityUserManager,
        IRepository<NewsEntidad, int> newRepository, IRepository<AlertEntidad, int> alertRepository,
        IRepository<NotificationEntidad, int> notificationRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _themeRepository = themeRepository;
        _identityUserManager = identityUserManager;
        _newRepository = newRepository;
        _alertRepository = alertRepository;
        _notificationRepository = notificationRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // USUARIO       
        IdentityUser identityUser1 = new IdentityUser(Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d"), "admin", "admin@abp.io");
        await _identityUserManager.CreateAsync(identityUser1, "1q2w3E*");

        // TEMAS: Guardamos los temas y forzamos el guardado en la BD antes de insertar noticias
        var theme1 = await _themeRepository.InsertAsync(new Theme { Name = "Primer tema", User = identityUser1, UserId = identityUser1.Id });
        var theme2 = await _themeRepository.InsertAsync(new Theme { Name = "Segundo tema", User = identityUser1, UserId = identityUser1.Id });
        var theme3 = await _themeRepository.InsertAsync(new Theme { Name = "Tercer tema", User = identityUser1, UserId = identityUser1.Id });

        // NOTICIAS: Ahora que los temas existen en la BD, podemos insertar noticias con ThemeId válido
        var new1 = await _newRepository.InsertAsync(new NewsEntidad { Title = "Primer noticia", ThemeId = 2 });



        // Añadir la noticia al tema
        theme2.listNews.Add(new1);
    }

}
