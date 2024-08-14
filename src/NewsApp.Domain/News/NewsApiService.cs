using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Statuses = NewsAPI.Constants.Statuses;

namespace NewsApp.News
{
    public class NewsApiService : INewsService
    {
        public async Task<ICollection<ArticleDto>> GetNewsAsync(string query)
        {
            ICollection<ArticleDto> responseList = new List<ArticleDto>();

            // init with your API key
            var newsApiClient = new NewsApiClient("7519ded2898042bb875c413c215fe843");
            var articlesResponse = await newsApiClient.GetEverythingAsync(new EverythingRequest
            {
                Q = query,
                SortBy = SortBys.Popularity,
                Language = Languages.EN,
                // consultamos de un mes para atras ya que es lo que permite la api gratis
                From = DateTime.Now.AddMonths(-1)
            }) ;

            //TODO: se deberia lanzar una excepcion si la consulta a la api da error.
            if (articlesResponse.Status == Statuses.Ok)
            {
                articlesResponse.Articles.ForEach( t=> responseList.Add(new ArticleDto {  Author = t.Author, 
                                                                                          Title = t.Title,
                                                                                          Description = t.Description,
                                                                                          Url = t.Url,
                                                                                          PublishedAt = t.PublishedAt
                }));                                
            }
            
            return responseList;
        }
    }
}
