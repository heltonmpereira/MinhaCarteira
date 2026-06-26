using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinhaCarteira.AppCliente.Helper;
using MinhaCarteira.AppCliente.Refit.Middleware;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using WebEssentials.AspNetCore.Pwa;
using Microsoft.AspNetCore.HttpOverrides;

namespace MinhaCarteira.AppCliente;

public class Startup
{
    public static IConfiguration Configuration { get; private set; }
    public static IWebHostEnvironment Environment { get; private set; }

    private void ConfigureDataProtection(IServiceCollection services, IWebHostEnvironment environment)
    {
        var keysDirectoryName = "Keys";
        var keysDirectoryPath = Path.Combine(environment.ContentRootPath, keysDirectoryName);
        if (!Directory.Exists(keysDirectoryPath))
        {
            Directory.CreateDirectory(keysDirectoryPath);
        }
        services.AddDataProtection()
              .PersistKeysToFileSystem(new DirectoryInfo(keysDirectoryPath))
              .SetApplicationName("MinhaCarteiraAppCliente");
    }

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Environment = env;
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureDataProtection(services, Environment);

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;

            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
        
        services
            .AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .AddRazorRuntimeCompilation()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .AddRazorRuntimeCompilation();

        services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");


        services.AddWebOptimizer(pipeline =>
        {
            pipeline.AddJavaScriptBundle(
                "/js/bundle.js",
                "/lib/jqueryui/jquery-ui.min.js",
                "/lib/popper/umd/popper.min.js",
                "/lib/bootstrap/js/bootstrap.min.js",
                "/lib/font-awesome/js/fontawesome.min.js",
                "/lib/font-awesome/js/brands.min.js",
                "/lib/font-awesome/js/solid.min.js",
                "/lib/admin-lte/js/adminlte.min.js");

            pipeline.AddJavaScriptBundle(
                "/js/bundleRodape.js",
                "/js/site.js",
                "/lib/netStack.js/netstack.js");

            pipeline.AddJavaScriptBundle(
                "/js/bundleRodape2.js",
                "/lib/highlight/highlight.min.js");

            pipeline.AddJavaScriptBundle(
                "/js/bundleValidationScript1.js",
                "/lib/jquery-validation/dist/jquery.validate.min.js");

            pipeline.AddJavaScriptBundle(
                "/js/bundleValidationScript2.js",
                "/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js");

            pipeline.AddJavaScriptBundle(
                "/js/bundleValidationScript3.js",
                "/lib/jquery.mask/jquery.mask.js");

            pipeline.AddJavaScriptBundle(
                "/js/bundleValidationScript4.js",
                "/js/maskPlugin.js");

            pipeline.AddJavaScriptBundle(
                "/js/bundleValidationScript5.js",
                "/js/methods_pt.js");

            pipeline.AddJavaScriptBundle(
                "/js/bundleValidationScript6.js",
                "/lib/tinymce/tinymce.min.js");

            pipeline.AddCssBundle(
                "/css/bundle.css",
                "/lib/bootstrap/css/bootstrap-reboot.min.css",
                "/lib/bootstrap/css/bootstrap.min.css",
                "/lib/font-awesome/css/brands.min.css",
                "/lib/font-awesome/css/solid.min.css",
                "/lib/font-awesome/css/v5-font-face.min.css",
                "/lib/admin-lte/css/adminlte.min.css",
                "/css/stackTrace.css",
                "/lib/highlight.js/styles/default.min.css",
                "/lib/highlight.js/styles/tokyo-night-light.min.css",
                "/lib/jqueryui/themes/hot-sneaks/jquery-ui.css");
        },
        option =>
        {
            option.EnableCaching = true;
            option.EnableDiskCache = false;
            option.EnableMemoryCache = true;
            option.AllowEmptyBundle = true;
        });

        services.AddAntiforgery(opts => opts.Cookie.Name = Constante.NOME_COOKIE_ANTIFORGERY);
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opt =>
            {
                opt.Cookie.Name = Constante.NOME_COOKIE_ASPNET;
                opt.Cookie.SameSite = SameSiteMode.Strict;
                opt.ExpireTimeSpan = TimeSpan.FromDays(30);
                opt.SlidingExpiration = true;
                opt.LoginPath = new PathString("/login/index");
                opt.LogoutPath = new PathString("/login/logout");
                opt.AccessDeniedPath = new PathString("/login/naoautorizado");
            });

        services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        var options = new PwaOptions()
        {
            CacheId = "2.0.0",
            EnableCspNonce = true,
            Strategy = ServiceWorkerStrategy.NetworkFirst,
            //CustomServiceWorkerStrategyFileName = "js/service-worker.js",
            OfflineRoute = "/Offline.html",
            BaseRoute = Configuration.GetValue<string>("BaseRoutePwa"),
        };
        services.AddProgressiveWebApp(options);

        services.AdicionarConexoesRefit(Configuration["BaseUrlApi"]);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var cultureInfo = new CultureInfo("pt-BR")
        {
            NumberFormat =
            {
                CurrencySymbol = "R$",
                CurrencyPositivePattern = 2
            }
        };

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            HttpOnly = HttpOnlyPolicy.Always,
            Secure = CookieSecurePolicy.Always
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        //Registered before static files to always set header
        app.UseHsts(options => options.MaxAge(days: 365).IncludeSubdomains().Preload());
        app.UseXContentTypeOptions();
        app.UseReferrerPolicy(opts => opts.NoReferrer());

        app.UseForwardedHeaders();
        app.UseHttpsRedirection();
        app.UseWebOptimizer();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        //Registered after static files, to set headers for dynamic content.
        app.UseNoCacheHttpHeaders();
        app.UseXfo(xfo => xfo.SameOrigin());
        app.UseXXssProtection(options => options.EnabledWithBlockMode());
        app.UseRedirectValidation(opts =>
        {
            opts.AllowSameHostRedirectsToHttps();
            opts.AllowSameHostRedirectsToHttps(10443); //Allow redirects to custom HTTPS port
        });

        var secaoWebSec = Configuration.GetSection("NWebSec");
        if (secaoWebSec != null)
        {
            app.UseCsp(options => options
                .DefaultSources(s => s.Self())
                .ObjectSources(s => s.None())
                .BaseUris(s => s.None())
                .ImageSources(s => s
                    .Self()
                    .CustomSources([.. secaoWebSec.GetValue<string>("ImageSources").Split(",").Select(s => s.Trim())])
                    )
                .FontSources(s => s
                    .Self()
                    .CustomSources([.. secaoWebSec.GetValue<string>("FontSources").Split(",").Select(s => s.Trim())])
                    )
                .ConnectSources(s => s
                    .Self()
                    .CustomSources([.. secaoWebSec.GetValue<string>("ConnectSources").Split(",").Select(s => s.Trim())])
                    )
                .StyleSources(s => s
                    .CustomSources([.. secaoWebSec.GetValue<string>("StyleSources").Split(",").Select(s => s.Trim())])
                    )
                .ScriptSources(s => s
                    .CustomSources([.. secaoWebSec.GetValue<string>("ScriptSources").Split(",").Select(s => s.Trim())])
                    )
             );
        }

        var securitytxt = $"Contact: contato@dhanidias.com.br\r\nExpires: Mon, 01 Aug 2050 00:00 +0300";
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
              name: "admin",
              areaName: "Admin",
              pattern: "admin/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapGet("/.well-known/security.txt", () => securitytxt);
            endpoints.MapGet("/security.txt", () => securitytxt);
        });
    }
}