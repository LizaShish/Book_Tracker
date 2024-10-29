using Book_Tracker.Interface;
using Book_Tracker.Models;
using Book_Tracker.Repository;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Book_Tracker
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<AppDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRepository<Book>, BookRepository>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;  
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); 
                app.UseHsts();  
            }

            app.UseHttpsRedirection(); // Перенаправление HTTP-запросов на HTTPS
            app.UseStaticFiles(); // Поддержка статических файлов (CSS, JS, изображения и т.д.)
            app.UseCookiePolicy();
            app.UseRouting(); // Включение маршрутизации
            app.UseAuthorization(); // Подключение системы авторизации

            // Настройка маршрутов по умолчанию
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Book}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                 name: "authors",
                 pattern: "{controller=Author}/{action=Index}/{id?}");
            }); 

        }
    }
}
