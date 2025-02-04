using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsApp.Alert;
using NewsApp.News;
using NewsApp.Notification;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using System.Linq;
using System.Collections.Generic;


namespace NewsApp.NewsBackground
{
    public class NewsBackgroundAppService : BackgroundService
    {
        private readonly INewsAppService _newsService;              //CORREGIDO: INewsApiService a INewsAppService
        private readonly IRepository<AlertEntidad, int> _alertRepository;
        private readonly IRepository<NotificationEntidad, int> _notificationRepository;
        private readonly ILogger<NewsBackgroundAppService> _logger;
        private readonly INotificationManager _notificationManager;
        private readonly IAlertAppService _alertService;
        private readonly INotificationAppService _notificationAppService;

        public NewsBackgroundAppService(
            INewsAppService newsService,        //CORREGIDO: INewsApiService a INewsAppService
            INotificationManager notificationManager,
            IRepository<AlertEntidad, int> alertRepository,
            IRepository<NotificationEntidad, int> notificationRepository,
            ILogger<NewsBackgroundAppService> logger,
            IAlertAppService alertService,
            INotificationAppService notificationAppService)

        {
            _newsService = newsService;
            _alertRepository = alertRepository;
            _notificationRepository = notificationRepository;
            _logger = logger;
            _notificationManager = notificationManager;
            _alertService = alertService;
            _notificationAppService = notificationAppService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Iniciando búsqueda de noticias para alertas...");

                    var alertas = await _alertService.GetAlertsActivasAsync();

                    await _alertService.ProcessNewsAlertsAsync(alertas);

                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante la ejecución del servicio de búsqueda de noticias");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }









        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            _logger.LogInformation("Iniciando búsqueda de noticias para alertas...");

        //            var alertas = await _alertRepository.GetListAsync(x => x.Activa); //Obtengo todas alertas activa

        //            foreach (var alerta in alertas)
        //            {
        //                // Obtener la última notificación para esta alerta
        //                var ultimaNotificacion = await _notificationRepository
        //                    .GetListAsync(n => n.AlertId == alerta.Id);
        //                var ultimaNotif = ultimaNotificacion
        //                    .OrderByDescending(n => n.FechaEnvio)
        //                    .FirstOrDefault();                                        //Obtengo las notificaciones y la ordeno de forma descendente, y obtengo la primera q es la más reciente

        //                var noticias = await _newsService.Search(alerta.CadenaBusqueda); // Busca todas las noticias con el texto de busqueda de la alerta 

        //                if (ultimaNotif == null)
        //                {
        //                    var nuevasNoticias = noticias
        //                        .Where(n => n.PublishedAt > alerta.FechaCreacion)
        //                        .ToList();

        //                    if (nuevasNoticias.Any())
        //                    {
        //                        await _notificationManager.CrearNotificacion(alerta, nuevasNoticias);
        //                    }
        //                }
        //                else
        //                {
        //                    // Filtrar solo noticias más recientes que la última notificación
        //                    var noticiasNuevas = noticias
        //                        .Where(n => n.PublishedAt > ultimaNotif.FechaEnvio)
        //                        .ToList();

        //                    if (noticiasNuevas.Any())
        //                    {
        //                        await _notificationManager.CrearNotificacion(alerta, noticiasNuevas);
        //                    }
        //                }
        //            }

        //            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error durante la ejecución del servicio de búsqueda de noticias");
        //            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        //        }
        //    }
        //}




    }
}

