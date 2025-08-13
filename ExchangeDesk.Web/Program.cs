using ExchangeDesk.Web.Data;
using ExchangeDesk.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExchangeDesk.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext + connection string
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity + ���� (� ���������� ������������ �� e-mail �� DEV)
            builder.Services
                .AddDefaultIdentity<IdentityUser>(o =>
                {
                    o.SignIn.RequireConfirmedAccount = false; // ��-����� �� ����������
                })
                .AddRoles<IdentityRole>() // <-- �����: ������ ������ �����
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(); // <-- ����� �� Identity UI (Login/Register)

            var app = builder.Build();

            // Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // <-- ������ �� � ����� Authorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Seed: ���� + ����� �����
            await SeedRolesAndAdminAsync(app);
            await SeedDataAsync(app);

            app.Run();
        }
        private static async Task SeedDataAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Offices
            if (!context.Offices.Any())
            {
                context.Offices.AddRange(
                    new Office { Name = "��������� ����", Location = "�����" },
                    new Office { Name = "���� �������", Location = "�������" }
                );
            }

            // Currencies
            if (!context.Currencies.Any())
            {
                context.Currencies.AddRange(
                    new Currency { Code = "EUR", Name = "����" },
                    new Currency { Code = "USD", Name = "������ �����" },
                    new Currency { Code = "GBP", Name = "��������� �����" },
                    new Currency { Code = "TRY", Name = "������ ����" },
                    new Currency { Code = "BGN", Name = "��������� ���" } // ������ ������
                );
            }

            // ExchangeRates
            if (!context.ExchangeRates.Any())
            {
                var now = DateTime.UtcNow;
                context.ExchangeRates.AddRange(
                    new ExchangeRate { CurrencyCode = "EUR", RateToBGN = 1.95583m, AsOf = now },
                    new ExchangeRate { CurrencyCode = "USD", RateToBGN = 1.80m, AsOf = now },
                    new ExchangeRate { CurrencyCode = "GBP", RateToBGN = 2.25m, AsOf = now },
                    new ExchangeRate { CurrencyCode = "TRY", RateToBGN = 0.10m, AsOf = now }
                );
            }

            await context.SaveChangesAsync();
        }
        private static async Task SeedRolesAndAdminAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Administrator", "User" };
            foreach (var r in roles)
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));

            var adminEmail = "admin@local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var res = await userManager.CreateAsync(admin, "Admin!23"); // ����� �������� ��-�����
                if (res.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Administrator");
            }
        }
    }
}
