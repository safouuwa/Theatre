using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StarterKit.Models;
using StarterKit.Services;
using StarterKit.Filters;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.AspNetCore.Cors;

namespace StarterKit
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", builder => builder
                    .WithOrigins("http://localhost:3000")  // React app running on port 3000
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());  // Optional, if you're sending credentials (cookies, etc.)
            });

            builder.Services.AddDistributedMemoryCache();

            builder.Services.Configure<TheatreHours>(builder.Configuration.GetSection("TheatreSettings"));            

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
            builder.Services.AddScoped<IVenueService, VenueService>();


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

            app.UseCors("AllowReactApp");

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Use(async (context, next) =>
            {
                var _theatreHours = context.RequestServices.GetRequiredService<IOptions<TheatreHours>>().Value;
                var currentTime = DateTime.Now.TimeOfDay;
                var startTime = DateTime.ParseExact(_theatreHours.OpeningTime, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
                var endTime = DateTime.ParseExact(_theatreHours.ClosingTime, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
                var endpoint = context.GetEndpoint();
                bool isCrossingMidnight = endTime < startTime;
                if (currentTime <= startTime && currentTime <= endTime)
                {
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsync("The cinema is unfortunately closed at this moment. Please try again at 09:00 in the morning.");
                    return;
                }
                await next.Invoke();
            });

            app.Run();

        }
    }
}
//class for options
public class TheatreHours
    {
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
    }