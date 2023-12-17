using Auction.Models.MSSQLModels;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Auction
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<LocalDBContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => 
                { 
                    options.LoginPath = "/auth/login";
                    options.AccessDeniedPath = "/"; 
                });
            builder.Services.AddAuthorization();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute("search", "", new {controller = "Home", action = "Index"});
            app.MapControllerRoute("getlot", "lot", new { controller = "Lot", action = "Index" });
            app.MapControllerRoute("managelot", "lot/{action}", new { controller = "Lot" });
            app.MapGroup("auth/").MapControllerRoute("auth", "{action}", new { controller = "Auth" });

            app.Run();
        }
    }
}
