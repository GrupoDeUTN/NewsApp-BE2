using NewsApp.News;
using NewsApp.Themes;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace NewsApp;

public class NewsAppTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Theme, int> _themeRepository;
    private readonly IRepository<NewsEntidad, int> _newRepository;
    private readonly IdentityUserManager _identityUserManager;


    public NewsAppTestDataSeedContributor(IRepository<Theme, int> themeRepository, IdentityUserManager identityUserManager, IRepository<NewsEntidad, int> newRepository)
    {
        _themeRepository = themeRepository;
        _identityUserManager = identityUserManager;
        _newRepository = newRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // USUARIO       
        IdentityUser identityUser1 = new IdentityUser(Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d"), "admin", "admin@abp.io");
        await _identityUserManager.CreateAsync(identityUser1, "1q2w3E*");
       // await _identityUserManager.AddToRoleAsync(identityUser1, "Admin");


        //TEMAS 
        var theme1 = await _themeRepository.InsertAsync(new Theme { Name = "Primer tema", User = identityUser1 });
        
        var theme2 = await _themeRepository.InsertAsync(new Theme { Name = "Segundo tema", User = identityUser1 });

        var theme3 = await _themeRepository.InsertAsync(new Theme { Name = "Tercer tema", User = identityUser1 });


        //NOTICIAS 
        var new1 = await _newRepository.InsertAsync(new NewsEntidad { Title = "Primer noticia", ThemeId = 2 });
        //var new2 = await _newRepository.InsertAsync(new NewsEntidad { Title = "Segunda noticia", ThemeId = 2 });

        //Añadir noticias a algún tema
        theme2.listNews.Add(new1);

        
         
    }
}
