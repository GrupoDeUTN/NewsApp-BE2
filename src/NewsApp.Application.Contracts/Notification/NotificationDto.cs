using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsApp.Notification
{
    public class NotificationDto : EntityDto<int>
    {
        public DateTime? FechaEnvio { get; set; }

        public bool Leida { get; set; }

        public string? CadenaBusqueda { get; set; }

        public int CantidadNoticiasNuevas { get; set; }

        public int AlertId { get; set; }

    }
}
