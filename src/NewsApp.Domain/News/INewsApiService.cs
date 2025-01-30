using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace NewsApp.News
{
    public interface INewsApiService : IDomainService
    {
        Task<ICollection<ArticleDto>> GetNewsAsync(string query);
    }
}
