using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using StarterKit.Services;
using StarterKit.Filters;

namespace StarterKit
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            

            builder.Services.AddDistributedMemoryCache();

            

            builder.Services.AddSession(options => 
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true; 
                options.Cookie.IsEssential = true; 
            });

            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<ITheatreShowService, TheatreShowService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IPointService, PointService>();
            builder.Services.AddScoped<IRewardService, RewardService>();


            builder.Services.AddDbContext<DatabaseContext>(
                options => options.UseSqlite(builder.Configuration.GetConnectionString("SqlLiteDb")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Use(async (context, next) =>
            {
                var currentTime = DateTime.Now.TimeOfDay;
                var startTime = new TimeSpan(0, 0, 0);  // 00:00
                var endTime = new TimeSpan(9, 0, 0);    // 09:00
                var endpoint = context.GetEndpoint();
                if (currentTime >= startTime && currentTime <= endTime && endpoint.Metadata.OfType<UserOnly>().Any())
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("The cinema is unfortunately closed at this moment. Please try again at 09:00 in the morning.");
                    return;
                }
                await next.Invoke();
            });

            app.Run();

        }
    }
}