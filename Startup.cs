using PizzaDelivery.Models;
using System.Globalization;
using PizzaDelivery.Profiles;
using PizzaDelivery.Services;
using PizzaDelivery.Settings;
using PizzaDelivery.Interfaces;
using PizzaDelivery.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Caching;
using MudBlazor.Services;

namespace PizzaDelivery
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddRazorPages();
            services.AddServerSideBlazor(); // Adicione esta linha
            services.AddMudServices(); // Adicione esta linha

            services.AddOutputCache();
            services.AddImageSharp(options =>
                {
                    options.BrowserMaxAge = TimeSpan.FromDays(7);
                    options.CacheMaxAge = TimeSpan.FromDays(365);
                    options.CacheHashLength = 8;
                }).Configure<PhysicalFileSystemCacheOptions>(options =>
                {
                    options.CacheFolder = "img/cache";
                });

            services.AddSingleton<IProcessadorImagem, ProcessadorImagemService>();

            services.AddAutoMapper(typeof(UsuarioProfile));

            services.Configure<OutlookSettings>(Configuration.GetSection(nameof(OutlookSettings)));
            services.AddSingleton<IEmailService, OutlookService>();

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEstadoRepository, EstadoRepository>();
            services.AddScoped<ICidadeRepository, CidadeRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IPedidoItemRepository, PedidoItemRepository>();

            //string connectionString = Configuration.GetConnectionString("MySqlDevConn");
            //services.AddDbContext<PizzaDeliveryDbContext>(options =>
            //    options.UseMySQL(connectionString)
            //      .LogTo(Console.WriteLine, LogLevel.Information)
            //      .EnableSensitiveDataLogging()
            //      .EnableDetailedErrors()
            //      );

            string connectionString = Configuration.GetConnectionString("postgresql");
            services.AddDbContext<PizzaDeliveryDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Para evitar problemas com DateTime.OffSet no PostgreSQL
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddIdentity<UsuarioModel, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true; //false
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; //idem
                options.Password.RequireNonAlphanumeric = false; //true senha deverá conter @, #, $, etc.
                options.Password.RequireUppercase = false; //true;
                options.Password.RequireLowercase = false; //true;
                options.Password.RequireDigit = false; //true senha deverá conter caracteres numéricos.
                options.Password.RequiredUniqueChars = 1; //1;
                options.Password.RequiredLength = 6; //6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //5
                options.Lockout.MaxFailedAccessAttempts = 5; //5
                options.Lockout.AllowedForNewUsers = true; //true		
                options.SignIn.RequireConfirmedEmail = true; //false
                options.SignIn.RequireConfirmedPhoneNumber = false; //false
                options.SignIn.RequireConfirmedAccount = false; //false
            })
            .AddEntityFrameworkStores<PizzaDeliveryDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthorization();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = $".{Configuration.GetValue<string>("AppSettings:AppGuid")}ASPXFORMS"; //AspNetCore.Cookies
                options.Cookie.HttpOnly = true; //true
                options.ExpireTimeSpan = TimeSpan.FromDays(14); //14 dias / Tempo de vida do cookie de autenticação
                options.LoginPath = "/Usuario/Login"; // /Account/Login
                options.LogoutPath = "/Home/Index";  // /Account/Logout
                options.AccessDeniedPath = "/Usuario/AcessoRestrito"; // /Account/AccessDenied
                options.SlidingExpiration = true; //true - gera um novo cookie a cada requisição se o cookie estiver com menos de meia vida
                options.ReturnUrlParameter = "returnUrl"; //returnUrl
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            UserManager<UsuarioModel> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Para ambiente de produção, você pode configurar um tratamento de erro personalizado.
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();

            var supportedCultures = new[] { new CultureInfo("pt-BR") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("pt-BR", "pt-BR"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures

            });

            app.UseImageSharp();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
            });
            Inicializador.InicializarIdentity(userManager, roleManager).Wait();

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseOutputCache();
        }
    }
}




