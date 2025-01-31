using Microsoft.EntityFrameworkCore;
using NewsApp.AccesoAPI;
using NewsApp.Alert;
using NewsApp.News;
using NewsApp.Notification;
using NewsApp.Themes;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace NewsApp.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class NewsAppDbContext :
    AbpDbContext<NewsAppDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    #region Entidades de dominio

    public DbSet<Theme> Themes { get; set; }
    public DbSet<NewsEntidad> NewsEntidad{ get; set; }

    public DbSet<AlertEntidad> AlertEntidad { get; set; } 

    public DbSet<NotificationEntidad> NotificationEntidad { get; set; }

    public DbSet<AccesoApiEntidad> AccesoApiEntidad { get; set; }

#endregion

public NewsAppDbContext(DbContextOptions<NewsAppDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();





        // Entidad News
        builder.Entity<NewsEntidad>(b => {
            b.ToTable(NewsAppConsts.DbTablePrefix + "News", NewsAppConsts.DbSchema);
            b.ConfigureByConvention();
        });


        // Entidad Themes

        builder.Entity<Theme>(b =>
        {
            b.ToTable(NewsAppConsts.DbTablePrefix + "Themes", NewsAppConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);

            // Relación de Theme con NewsEntidad
            b.HasMany(t => t.listNews)
             .WithOne()
             .HasForeignKey(n => n.ThemeId)
             .OnDelete(DeleteBehavior.Cascade);

            // Relación padre-hijo en Theme
            b.HasMany(t => t.Themes)
             .WithOne()
             .HasForeignKey(t => t.ThemeId)
             .OnDelete(DeleteBehavior.Cascade);
        });


        //Entidad alerta 
        builder.Entity<AlertEntidad>(b =>
        {
            b.ToTable(NewsAppConsts.DbTablePrefix + "Alerts", NewsAppConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasMany(a => a.Notificaciones) // Relación uno a muchos
                .WithOne(n => n.Alert)
                .HasForeignKey(n => n.AlertId)
                .OnDelete(DeleteBehavior.Cascade); // Elimina las notificaciones si se elimina la alerta
        });

        //Entidad notificacion 
        builder.Entity<NotificationEntidad>(b =>
        {
            b.ToTable(NewsAppConsts.DbTablePrefix + "Notifications", NewsAppConsts.DbSchema);
            b.ConfigureByConvention();
        });

        //Entidad AccesoApi 
        builder.Entity<AccesoApiEntidad>(b =>
        {
            b.ToTable(NewsAppConsts.DbTablePrefix + "AccesoApi", NewsAppConsts.DbSchema);
            b.ConfigureByConvention();
        });


    }
}
