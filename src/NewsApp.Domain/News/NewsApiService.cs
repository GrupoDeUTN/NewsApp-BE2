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

    public class NewsApiService : INewsService
    {

        //private readonly IRepository<AccesoApiEntidad, int> _accesoApiRepository;
        //private readonly ILogger<NewsApiService> _logger;
        //private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        //private readonly ICurrentUser _currentUser; // Añadir esto

        //public NewsApiService(
        //    IRepository<AccesoApiEntidad, int> accesoApiRepository,
        //    ILogger<NewsApiService> logger,
        //    UserManager<Volo.Abp.Identity.IdentityUser> userManager,
        //    ICurrentUser currentUser) // Añadir esto
        //{
        //    _accesoApiRepository = accesoApiRepository;
        //    _logger = logger;
        //    _userManager = userManager;
        //    _currentUser = currentUser; // Añadir esto
        //}
        public async Task<ICollection<ArticleDto>> GetNewsAsync(string query)
        {
            ICollection<ArticleDto> responseList = new List<ArticleDto>();

            //var userGuid = _currentUser.Id.GetValueOrDefault();     
            //var identityUser = await _userManager.FindByIdAsync(userGuid.ToString());


            //var accesoApi = new AccesoApiEntidad
            //{
            //    TiempoInicio = DateTime.Now,
            //    ErrorMessage = null,
            //    UserId = identityUser.Id
            //};

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

                // Registrar el tiempo de fin y calcular el tiempo total
                //accesoApi.TiempoFin = DateTime.Now;
                //accesoApi.TiempoTotal = accesoApi.TiempoFin - accesoApi.TiempoInicio;
                //await _accesoApiRepository.InsertAsync(accesoApi); 
            }
            
            return responseList;
        }
    }
}


