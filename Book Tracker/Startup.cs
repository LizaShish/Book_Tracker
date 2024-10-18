using Book_Tracker.Interface;
using Book_Tracker.Models;
using Book_Tracker.Repository;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Book_Tracker
{
    //Этот файл отвечает за настройку конфигурации и сервисов приложения,
    //а также за маршрутизацию. Вот пример итогового файла:
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Метод для регистрации сервисов
        public void ConfigureServices(IServiceCollection services)
        {
            // Добавляем поддержку MVC и Razor Pages
            services.AddControllersWithViews();

            // Настраиваем подключение к базе данных
            services.AddDbContext<AppDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Регистрация зависимостей
            services.AddScoped<IRepository<Book>, BookRepository>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;  // Устанавливаем, чтобы cookies передавались только через HTTPS
            });
        }

        // Метод для настройки middleware (среды исполнения запросов)
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Включение страницы ошибок в режиме разработки
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); // Указание страницы для обработки ошибок в продакшн-среде
                app.UseHsts();  // Использование HTTP Strict Transport Security
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
                // Маршрут по умолчанию (контроллер Book, действие Index)
                //Это значит, что если не указан путь, приложение по умолчанию будет направлено на контроллер Book и его действие Index.

                 endpoints.MapControllerRoute(
                   name: "authors",
                   pattern: "{controller=Author}/{action=Index}/{id?}");
            }); 



            
        }
    }
}
