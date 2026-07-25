using Microsoft.EntityFrameworkCore;
using SHEIN_Clone_MVC.Data;
using SHEIN_Clone_MVC.Helpers;

namespace SHEIN_Clone_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -------------------------
            // DATABASE
            // -------------------------
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            // -------------------------
            // MVC
            // -------------------------
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // -------------------------
            // ERROR HANDLING
            // -------------------------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // -------------------------
            // CORE MIDDLEWARE ORDER (IMPORTANT)
            // -------------------------
            app.UseHttpsRedirection();

            // ✅ THIS FIXES YOUR IMAGE ISSUE
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // -------------------------
            // OPTIONAL: JSON GENERATOR
            // -------------------------
            var webRoot = app.Environment.WebRootPath;
            AutoJsonGenerator.Generate(webRoot);

            // -------------------------
            // DATABASE SEEDING
            // -------------------------
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                DbInitializer.Seed(context);
            }

            // -------------------------
            // ROUTING
            // -------------------------
            app.UseSession();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Products}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}