using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;


namespace NewsApp.AccesoAPI
{
    public class AccesoApiEntidad : Entity<int>
    {

        public TimeSpan TiempoTotal { get; set; }

        public DateTime TiempoInicio { get; set; }

        public DateTime TiempoFin { get; set; }

        public string? ErrorMessage { get; set; }

        public IdentityUser? User { get; set; }

        public Guid UserId { get; set; } // Propiedad de clave externa

    }
}
