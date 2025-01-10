using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsApp.Notification
{
    public class NotificationEntidad
    {
        public DateTime? FechaEnvio { get; set; }

        public bool Leida { get; set; }

        public string? CadenaBusqueda { get; set; }

        public int CantidadNoticiasNuevas {  get; set; }

    }
}
