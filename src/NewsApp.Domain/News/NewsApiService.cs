using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using NewsApp.AccesoAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Statuses = NewsAPI.Constants.Statuses;
using Volo.Abp.Users;



namespace NewsApp.News
{

    public class NewsApiService : INewsApiService
    {

        public async Task<ICollection<ArticleDto>> GetNewsAsync(string query)
        {
            ICollection<ArticleDto> responseList = new List<ArticleDto>();


            var newsApiClient = new NewsApiClient("7519ded2898042bb875c413c215fe843");
            var articlesResponse = await newsApiClient.GetEverythingAsync(new EverythingRequest
            {
                Q = query,
                SortBy = SortBys.Popularity,
                Language = Languages.EN,
                // consultamos de un mes para atras ya que es lo que permite la api gratis
                From = DateTime.Now.AddMonths(-1)
            }) ;

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


