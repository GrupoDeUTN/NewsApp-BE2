using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsApp.AccesoApi
{
    // DTO auxiliar para las estadísticas por usuario
    public class EstadisticasUsuarioDto : EntityDto<int>
    {
        public string UserName { get; set; }
        public int TotalConsultas { get; set; }
        public TimeSpan TiempoPromedioConsulta { get; set; }
    }
}
