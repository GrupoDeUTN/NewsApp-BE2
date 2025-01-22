using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsApp.AccesoApi
{
    public class AccesoApiDto : EntityDto<int>
    {
        public TimeSpan TiempoTotal { get; set; }

        public DateTime TiempoInicio { get; set; }

        public DateTime TiempoFin { get; set; }

        public string? ErrorMessage { get; set; }

    }
}
