using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsApp.News
{
    public interface INewsAppService : IApplicationService
    {
        Task<ICollection<NewsDto>> AsyncSearch(string query, Guid userId);
        Task<ICollection<NewsDto>> Search(string query);
        NewsDto SeleccionarNewsDeBusqueda(ICollection<NewsDto> resultados, string titulo);
    }
}
