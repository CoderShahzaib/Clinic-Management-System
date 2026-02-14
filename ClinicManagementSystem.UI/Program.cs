using ClinicManagementSystem.Core.Domain.IdentityEntities;
using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.Infrastructure.DatabaseContext;
using ClinicManagementSystem.Infrastructure.Services;
using ClinicManagementSystem.Infrastructure.Data.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";       
    options.LogoutPath = "/Logout";      
    options.AccessDeniedPath = "/AccessDenied";
    options.Cookie.Name = "ClinicAuthCookie"; 
    options.Cookie.HttpOnly = true;              
    options.ExpireTimeSpan = TimeSpan.FromDays(1);  
    options.SlidingExpiration = true;             
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

    await IdentitySeeder.SeedAdminAsync(userManager, roleManager);
    await IdentitySeeder.SeedDoctorRoleAsync(roleManager);
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.Redirect("/Login");
        }
        else if (context.User.IsInRole("Admin"))
        {
            context.Response.Redirect("/Admin-Dashboard");
        }
        else if (context.User.IsInRole("Doctor"))
        {
            context.Response.Redirect("/Doctor-Dashboard");
        }
        else if (context.User.IsInRole("Patient"))
        {
            context.Response.Redirect("/Patient-Dashboard");
        }
        else
        {
            context.Response.Redirect("/Login");
        }
        return; 
    }
    await next();
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
