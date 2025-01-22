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


namespace NewsApp
{
    public class NewsBackgroundAppService : BackgroundService
    {
        private readonly INewsService _newsService;
        private readonly IRepository<AlertEntidad, int> _alertRepository;
        private readonly IRepository<NotificationEntidad, int> _notificationRepository;
        private readonly ILogger<NewsBackgroundAppService> _logger;

        public NewsBackgroundAppService(
            INewsService newsService,
            IRepository<AlertEntidad, int> alertRepository,
            IRepository<NotificationEntidad, int> notificationRepository,
            ILogger<NewsBackgroundAppService> logger)
        {
            _newsService = newsService;
            _alertRepository = alertRepository;
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Iniciando búsqueda de noticias para alertas...");

                    var alertas = await _alertRepository.GetListAsync(x => x.Activa); //Obtengo todas alertas activa

                    foreach (var alerta in alertas)
                    {
                        // Obtener la última notificación para esta alerta
                        var ultimaNotificacion = await _notificationRepository
                            .GetListAsync(n => n.AlertId == alerta.Id);
                        var ultimaNotif = ultimaNotificacion
                            .OrderByDescending(n => n.FechaEnvio)
                            .FirstOrDefault();                                        //Obtengo las notificaciones y la ordeno de forma descendente, y obtengo la primera q es la más reciente

                        var noticias = await _newsService.GetNewsAsync(alerta.CadenaBusqueda); // Busca todas las noticias con el texto de busqueda de la alerta 

                        if (ultimaNotif == null)
                        {
                            var nuevasNoticias = noticias
                                .Where(n => n.PublishedAt > alerta.FechaCreacion)
                                .ToList();

                            if (nuevasNoticias.Any())
                            {
                                await CrearNotificacion(alerta, nuevasNoticias);
                            }
                        }
                        else
                        {
                            // Filtrar solo noticias más recientes que la última notificación
                            var noticiasNuevas = noticias
                                .Where(n => n.PublishedAt > ultimaNotif.FechaEnvio)
                                .ToList();

                            if (noticiasNuevas.Any())
                            {
                                await CrearNotificacion(alerta, noticiasNuevas);
                            }
                        }
                    }

                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante la ejecución del servicio de búsqueda de noticias");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task CrearNotificacion(AlertEntidad alerta, ICollection<ArticleDto> noticias)
        {
            var notificacion = new NotificationEntidad
            {
                FechaEnvio = DateTime.UtcNow,
                Leida = false,
                CadenaBusqueda = alerta.CadenaBusqueda,
                CantidadNoticiasNuevas = noticias.Count,
                AlertId = alerta.Id,
                
            };

            await _notificationRepository.InsertAsync(notificacion);
        }


    }
}

