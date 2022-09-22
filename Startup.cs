﻿using blog_web.Data.Extension;
using blog_web.Extension;
using blog_web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace blog_web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<blogdbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaulConnectionString")));

            services.AddSession();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddControllersWithViews();

            services.AddScoped<Saveimage>();

            services.AddMemoryCache();

            services.AddAuthentication("CookieAuthentication_zz")
                    .AddCookie("CookieAuthentication_zz", config =>
                    {
                        config.Cookie.Name = "UserLoginCookie";
                        config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                        config.LoginPath = "/dang-nhap.html";
                        config.LogoutPath = "/dang-xuat.html";
                        config.AccessDeniedPath = "/not-found.html";
                    });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings,only this changes expiration
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromMinutes(30);
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });

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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
           
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication(); // xác thực

            app.UseAuthorization(); // quyền

          
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                  );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
