using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FomoApi.Models;
using FomoAPI.Application;
using FomoAPI.Application.EventBuses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MasterContext>(options =>
                 options.UseSqlServer(

            Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                 .AddEntityFrameworkStores<MasterContext>()
                 .AddDefaultTokenProviders();

            services.AddAuthentication()
            .AddGoogle(o =>
            {
                o.ClientId = Configuration["Authentication:Google:ClientId"];
                o.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                o.SaveTokens = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(DevelopmentCorsPolicyName,
                builder =>
                {
                    builder.WithOrigins("https://localhost:44395")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials();
                });
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHttpClient("AlphaVantage", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("AlphaVantageUrl"));          
            });

            ApplicationContainer = AutoFacSetup.Setup(services, Configuration);

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseCors(DevelopmentCorsPolicyName);
            app.UseMvc();
        }


    }
}
