using Microsoft.EntityFrameworkCore;
using UserManagementApp.Data;
using UserManagementApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<PasswordService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.Run();
