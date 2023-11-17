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


        public async Task CreateAsyncNews(NewsEntidad newsEntidad)
        {
           await _repository.InsertAsync(newsEntidad);
        }
    }
}
