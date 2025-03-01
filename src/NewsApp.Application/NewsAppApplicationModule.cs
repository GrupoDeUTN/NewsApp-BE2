﻿using Microsoft.Extensions.DependencyInjection;
using NewsApp.News;
using NewsApp.NewsBackground;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace NewsApp;

[DependsOn(
    typeof(NewsAppDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(NewsAppApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class NewsAppApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<NewsAppApplicationModule>();
        });

        //se registra el servicio de noticias. Deberia registrarse solo, pero como me dio error lo incorporo aca:
        context.Services.AddTransient<INewsApiService, NewsApiService>();
        context.Services.AddHostedService<NewsBackgroundAppService>();

    }
}
