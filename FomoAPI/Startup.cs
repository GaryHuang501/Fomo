using System;
using System.Threading.Tasks;
using FomoApi.Infrastructure;
using FomoAPI.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FomoAPI.Controllers.Authorization;
using Microsoft.OpenApi.Models;
using System.IO;
using FomoAPI.Application.Swagger.OperationFilters;

namespace FomoAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private readonly string DevelopmentCorsPolicyName = "DevelopmentCORS";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCustomDBContexts(Configuration)
                    .AddCustomOptions(Configuration)
                    .AddCustomAuthentications(Configuration)
                    .AddCustomCORS(Configuration, DevelopmentCorsPolicyName)
                    .AddCustomAntiForgery(Configuration)
                    .AddCustomHttpClients(Configuration)
                    .AddSwaggerGen(c =>
                   {
                       c.SwaggerDoc("v1",
                            new OpenApiInfo
                            {
                                Title = "FOMO API V1",
                                Version = "V1"
                            }
                         );

                       c.OperationFilter<AuthResponsesOperationFilter>();
                       var filePath = Path.Combine(System.AppContext.BaseDirectory, "FomoAPI.xml");
                       c.IncludeXmlComments(filePath);
                   });

            services.AddSwaggerGenNewtonsoftSupport();


            services.AddMvc()
                    .AddApplicationPart(typeof(Startup).Assembly)
                    .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(DevelopmentCorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FOMO API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapRazorPages();
                });
            });
        }
    }

    public static class CustomStartUpExtensions
    {
        public static IServiceCollection AddCustomDBContexts(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<LoginContext>(options =>
                 options.UseSqlServer(config["Database:ConnectionString"]));

            return services;
        }

        public static IServiceCollection AddCustomAuthentications(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
                    .AddEntityFrameworkStores<LoginContext>()
                    .AddDefaultTokenProviders();

            services.AddAuthorization(o =>
           {
               o.AddPolicy(PolicyTypes.PortfolioOwner, policy =>
               {
                   policy.Requirements.Add(new PortfolioOwnerRequirement());
                   policy.RequireAuthenticatedUser();
               });
           });

            services.AddAuthentication()
                    .AddGoogle(o =>
                    {
                        o.ClientId = config["Authentication:Google:ClientId"];
                        o.ClientSecret = config["Authentication:Google:ClientSecret"];
                        o.SaveTokens = true;
                    });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/api/accounts/login";
                options.LogoutPath = $"/api/Account/Logout";
                options.Cookie.Name = "IdentityCookie";
                options.Cookie.Path = "/";
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomCORS(this IServiceCollection services, IConfiguration configuration, string policyName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                builder =>
                {
                    builder.WithOrigins(configuration.GetValue<string>("Accounts:MainPage"))
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomAntiForgery(this IServiceCollection services, IConfiguration config)
        {
            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                options.Cookie.Name = "X-CSRF-TOKEN-COOKIENAME";
                options.Cookie.Path = "Path";
                options.FormFieldName = "AntiforgeryFieldname";
                options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
                options.SuppressXFrameOptionsHeader = false;
            });

            return services;
        }

        public static IServiceCollection AddCustomHttpClients(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient(config["AlphaVantage:ClientName"], c => { });
            services.AddHttpClient(config["Finnhub:ClientName"], c => { });
            services.AddHttpClient("Firebase:ClientName", c => { });

            return services;
        }

        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration config)
        {
            OptionsSetup.RegisterOptions(services, config);

            return services;
        }


    }
}
