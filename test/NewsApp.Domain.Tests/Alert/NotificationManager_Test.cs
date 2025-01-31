using Microsoft.AspNetCore.Identity;
using NewsApp;
using NewsApp.Alert;
using NewsApp.EntityFrameworkCore;
using NewsApp.News;
using NewsApp.Notification;
using NewsApp.Themes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Xunit;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
public class NotificationManager_Test : NewsAppDomainTestBase
{

    private readonly IRepository<AlertEntidad, int> _alertRepository;
    private readonly IRepository<NotificationEntidad, int> _notificationRepository;
    private readonly INotificationManager _notificationManager;

    private readonly IdentityUserManager _identityUserManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IDbContextProvider<NewsAppDbContext> _dbContextProvider;

    public NotificationManager_Test()
    {
        _notificationRepository = GetRequiredService<IRepository<NotificationEntidad, int>>();
        _notificationManager = GetRequiredService<INotificationManager>();
        _alertRepository = GetRequiredService<IRepository<AlertEntidad, int>>();
        _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        _dbContextProvider = GetRequiredService<IDbContextProvider<NewsAppDbContext>>();
        _identityUserManager = GetRequiredService<IdentityUserManager>();

    }


    [Fact]
    public async Task Should_Create_New_Notification()
    {
        using (var uow = _unitOfWorkManager.Begin())
        {
        // Arrange
        var articleCollection = new List<ArticleDto>();


        var article1 = new ArticleDto
        {
            
            Author = "John Doe",
            Title = "C# en 2025",
            Description = "Las novedades de C# en el futuro.",
            Url = "https://example.com/csharp",
            UrlToImage = "https://example.com/csharp.jpg",
            PublishedAt = DateTime.Now,
            Content = "Contenido del artículo..."
        };

        var article2 = new ArticleDto
        {
            
            Author = "Jane Smith",
            Title = "IA en el desarrollo",
            Description = "Cómo la IA está cambiando el desarrollo de software.",
            Url = "https://example.com/ia",
            UrlToImage = "https://example.com/ia.jpg",
            PublishedAt = DateTime.Now,
            Content = "Más contenido..."
        };

        articleCollection.Add(article1);
        articleCollection.Add(article2);



        var alert = await _alertRepository.GetAsync(1);
        alert.Notificaciones = new List<NotificationEntidad>();

        var cantidad = alert.Notificaciones.Count;

        // Act
         await _notificationManager.CrearNotificacion(alert, articleCollection);
            
        // Assert



            // Assert: comprueba que el tema no existe
            var dbContext = await _dbContextProvider.GetDbContextAsync();
            var notif = dbContext.NotificationEntidad.LastOrDefault(t => t.AlertId == alert.Id);

            alert.Notificaciones.Count.ShouldBe(cantidad+1);
            notif.ShouldNotBeNull();
            notif.CantidadNoticiasNuevas.ShouldBe(2);
            notif.CadenaBusqueda.ShouldBe(alert.CadenaBusqueda);

            var themeInDb = dbContext.Themes.GroupBy(t => t.Id).LastOrDefault();
        }
    }

    






}

