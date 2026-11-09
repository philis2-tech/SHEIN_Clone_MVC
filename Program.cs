using Microsoft.EntityFrameworkCore;
using SHEIN_Clone_MVC.Data;
using SHEIN_Clone_MVC.Helpers;
using System.IO;

namespace SHEIN_Clone_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


        // =====================================
        // DATABASE CONFIGURATION
        // =====================================

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
            )
        );

            // =====================================
            // SESSION CONFIGURATION
            // =====================================

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // =====================================
            // MVC CONFIGURATION
            // =====================================

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // =====================================
            // WEB ROOT CONFIGURATION
            // =====================================

            var webRoot = app.Environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(
                    app.Environment.ContentRootPath,
                    "wwwroot"
                );

                Directory.CreateDirectory(webRoot);
            }

            // =====================================
            // ERROR HANDLING
            // =====================================

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // =====================================
            // CORE MIDDLEWARE
            // =====================================

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            // =====================================
            // AUTO JSON GENERATOR
            // =====================================

            var imagesFolder = Path.Combine(
                webRoot,
                "Images"
            );

            if (Directory.Exists(imagesFolder))
            {
                AutoJsonGenerator.Generate(webRoot);
            }
            else
            {
                Console.WriteLine(
                    $"Images folder not found: {imagesFolder}"
                );
            }

            // =====================================
            // DATABASE SEEDING
            // =====================================

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService
                    <ApplicationDbContext>();

                DbInitializer.Seed(context);
            }

            // =====================================
            // ROUTING
            // =====================================

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Products}/{action=Index}/{id?}"
            );

            // =====================================
            // RUN APPLICATION
            // =====================================

            app.Run();
        }
    }


}
