using NewsApp.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace NewsApp.News
{
    public class NewsManager: DomainService
    {
        private readonly IRepository<NewsEntidad, int> _repository;
        public NewsManager(IRepository<NewsEntidad, int> repository)
        {
            _repository = repository;
        }


        public async Task<int> CreateAsyncNews(NewsEntidad newsEntidad)
        {
            // Inserta la entidad y guarda cambios en la base de datos
            await _repository.InsertAsync(newsEntidad, autoSave: true); 



            // Retorna el ID de la entidad que fue generada automáticamente
            return newsEntidad.Id;
        }

    }
}
