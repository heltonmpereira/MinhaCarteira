using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Modelo.Data;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace MinhaCarteira.AppServer;

public class Startup(IConfiguration configuration)
{
    private const string nomePoliticaCors = "_policySpecifOrigin";
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services
            .AddControllers(opt =>
            {
                opt.AllowEmptyInputInBodyModelBinding = true;
                opt.Filters.Add<AuditoriaActionFilter>();
                opt.Filters.Add<LoggingActionFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "MinhaCarteira - API",
                    Version = "v1"
                });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
                });
        });

        var origins = Configuration.GetSection("CorsOrigins").Value.Split(',').Select(s => s.Trim()).ToArray();
        if (origins != null && origins.Length > 0)
        {
            services.AddCors(x => x.AddPolicy(
            name: nomePoliticaCors,
            policy =>
            {
                policy
                    .WithOrigins(origins)
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    //.AllowAnyMethod()
                    .AllowAnyHeader()
                    ;
            }));
        }

        services.AdicionarAutenticacao();
        services.AdicionarDados(
            Configuration.GetConnectionString("Default"));

        services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        //{
        //    scope.ServiceProvider
        //        .GetService<MinhaCarteiraContext>()?
        //        .Database
        //        .Migrate();
        //}

        // Definindo a cultura padrão: pt-BR
        var supportedCultures = new[] { new CultureInfo("pt-BR") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        Debug.WriteLine(Configuration["SwaggerEndpoint"]);
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(
                $"{Configuration["UrlBase"]}{Configuration["SwaggerEndpoint"]}",
                "MinhaCarteira - API v1");
            c.InjectStylesheet($"{Configuration["UrlBase"]}/css/SwaggerDark.css");
            c.DocExpansion(DocExpansion.None);
        });

        app.UseFileServer(new FileServerOptions()
        {
            EnableDefaultFiles = true
        });

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors(nomePoliticaCors);
        app.UseAuthentication();
        app.UseAuthorization();

        //Registered before static files to always set header
        app.UseHsts(options => options.MaxAge(days: 365).IncludeSubdomains().Preload());
        app.UseXContentTypeOptions();
        app.UseReferrerPolicy(opts => opts.NoReferrer());
        app.UseNoCacheHttpHeaders();
        app.UseXfo(xfo => xfo.SameOrigin());
        app.UseXXssProtection(options => options.EnabledWithBlockMode());
        app.UseRedirectValidation(opts =>
        {
            opts.AllowSameHostRedirectsToHttps();
            opts.AllowSameHostRedirectsToHttps(10443); //Allow redirects to custom HTTPS port
            //opts.AllowedDestinations("http://www.nwebsec.com/", "https://www.google.com/accounts/");
        });

        var securitytxt = $"Contact: contato@dhanidias.com.br\r\nExpires: Mon, 01 Aug 2050 00:00 +0300";
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireCors(nomePoliticaCors);

            endpoints.MapGet("/.well-known/security.txt", () => securitytxt);
            endpoints.MapGet("/security.txt", () => securitytxt);
            //endpoints.MapGet("/", () => "API rodando no Render!");
        });
    }
}