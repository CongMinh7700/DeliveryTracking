using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingApp;

using Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Models;
using Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<DeliveryDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<SqlDependencyService>();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.AccessDeniedPath = "/Home/Index";
        options.LogoutPath = "/Authentication/Logout";
    });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.MapHub<DriverStatusHub>("/driverStatusHub");

        var sqlDependencyService = app.Services.GetRequiredService<SqlDependencyService>();
        sqlDependencyService.Start();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        //app.MapControllerRoute(
        //    name: "default",
        //    pattern: "{controller=User}/{action=UserPage}/{id?}");

        app.Run();
    }
}
