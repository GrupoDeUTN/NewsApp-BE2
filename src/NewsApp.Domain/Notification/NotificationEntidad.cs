using NewsApp.Alert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace NewsApp.Notification
{
    public class NotificationEntidad : Entity<int> 
    {
        public DateTime? FechaEnvio { get; set; }

        public bool Leida { get; set; }

        public string? CadenaBusqueda { get; set; }

        public int CantidadNoticiasNuevas { get; set; }

        // Clave externa y navegación inversa a AlertEntidad
        public int AlertId { get; set; } // Propiedad de clave externa
        public AlertEntidad? Alert { get; set; } // Navegación inversa
    }
}


