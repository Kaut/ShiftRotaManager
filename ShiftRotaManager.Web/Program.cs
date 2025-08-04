using Microsoft.EntityFrameworkCore;
using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Core.Services;
using ShiftRotaManager.Data.Data;
using ShiftRotaManager.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<IRotaRepository, RotaRepository>(); // Register new Rota Repository

// Register Services
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ITeamMemberService, TeamMemberService>();
builder.Services.AddScoped<IRotaService, RotaService>(); // Register new Rota Service

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

app.UseAuthorization(); // For authentication/authorization (not fully implemented in POC)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Apply migrations on startup (for development/testing purposes, consider alternatives for production)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Apply any pending migrations
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();