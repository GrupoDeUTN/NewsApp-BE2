using System;
using NewsApp.Themes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Entities;


namespace NewsApp.Alert
{
    public class AlertEntidad : Entity<int>
    {
        public DateTime? FechaCreacion { get; set; }

        public bool Activa { get; set; }

        public string? CadenaBusqueda { get; set; }

        public IdentityUser? User { get; set; }

        public Guid UserId { get; set; } // Propiedad de clave externa



    }
}


