using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsApp.Alert
{
    public class CreateAlertDto : EntityDto
    {
        public bool Activa { get; set; }
        public string? CadenaBusqueda { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}