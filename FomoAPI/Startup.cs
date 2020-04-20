using System;
using System.Threading.Tasks;
using Autofac;
using FomoApi.Infrastructure;
using FomoAPI.Setup;
using FomoAPI.AutoFacModules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FomoAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        private string DevelopmentCorsPolicyName = "DevelopmentCORS";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCustomDBContexts(Configuration)
                    .AddCustomAuthentications(Configuration)
                    .AddCustomCORS(DevelopmentCorsPolicyName)
                    .AddCustomAntiForgery()
                    .AddCustomHttpClients(Configuration)
                    .AddCustomOptions(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddApplicationPart(typeof(Startup).Assembly);
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

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApiModule());
            builder.RegisterModule(new EventBusModule());
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
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomCORS(this IServiceCollection services, string policyName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                builder =>
                {
                    builder.WithOrigins("https://localhost:44395")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomAntiForgery(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                options.Cookie.Domain = "localhost";
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
            services.AddHttpClient("AlphaVantage", c =>
            {
                c.BaseAddress = new Uri(config.GetValue<string>("AlphaVantageUrl"));
            });

            return services;
        }

        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration config)
        {
            OptionsSetup.RegisterOptions(services, config);

            return services;
        }
    }
}
