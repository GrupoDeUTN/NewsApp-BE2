using NewsApp.Notification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Volo.Abp.Application.Dtos;

namespace NewsApp.Alert
{
    public class AlertDto : EntityDto<int>
    {
        public DateTime? FechaCreacion { get; set; }

        public bool Activa { get; set; }

        public string? CadenaBusqueda { get; set; }

        // Excluir listNotifications de la serialización JSON
        [JsonIgnore]
        public ICollection<NotificationDto>? listNotifications { get; set; }
    }
}
