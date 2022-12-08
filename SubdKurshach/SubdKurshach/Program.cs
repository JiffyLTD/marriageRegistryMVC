using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SubdKurshach.DbContext;
using SubdKurshach.Models.Users;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options
            => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<User, IdentityRole>(opts =>
        {
            opts.Password.RequiredLength = 5;   // минимальная длина
            opts.Password.RequireNonAlphanumeric = false;   // требуются ли не алфавитно-цифровые символы
            opts.Password.RequireLowercase = false; // требуются ли символы в нижнем регистре
            opts.Password.RequireUppercase = false; // требуются ли символы в верхнем регистре
            opts.Password.RequireDigit = false; // требуются ли цифры
            opts.User.RequireUniqueEmail = true;//уникальность email
            opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ@.1234567890()";//разрешенные символы имени пользователя
        }).AddEntityFrameworkStores<AppDbContext>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

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
        app.UseAuthentication();

        app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}");

        app.Run();
    }
}