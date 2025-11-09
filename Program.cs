

using System.Threading.Tasks;
using Magazynek.Data;
using Magazynek.Entities;
using Magazynek.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();


builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5035);
});

string envFileName = "main.env";

string connectionString = EnvironmentConfig.GetPostgresConnectionString(envFileName);

builder.Services.AddDbContextFactory<DatabaseContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.LogTo(Console.WriteLine, LogLevel.Error);
});



builder.Services.AddIdentity<magazynek.Entities.AppUser, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<ISystemSettingsService,     SystemSettingsService>();
builder.Services.AddScoped<ITmeService,                TmeService>();
builder.Services.AddScoped<IProductService,            ProductService>();
builder.Services.AddScoped<IShippingEntryService,      ShippingEntryService>();
builder.Services.AddScoped<IProjectService,            ProjectService>();
builder.Services.AddScoped<IProjectReservationService, ProjectReservationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


await DoStartupThingsOnApp(app);
await SeedRolesAndAdminAsync(app.Services);


app.Run();

static async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<magazynek.Entities.AppUser>>();

    // Dodaj role, jeśli nie istnieją
    List<string> roleNames = [.. Enum.GetNames(typeof(magazynek.Entities.AppUser.UserType))];
    foreach (var roleName in roleNames)
        if (!await roleManager.RoleExistsAsync(roleName)) await roleManager.CreateAsync(new IdentityRole(roleName));


    // Utwórz admina, jeśli nie istnieje
    var adminEmail = "admin_magazynek@antslab.eu";
    var adminPassword = "Admin123!";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new magazynek.Entities.AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            userType = magazynek.Entities.AppUser.UserType.Admin
        };
        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, magazynek.Entities.AppUser.UserType.Admin.ToString());
        Console.WriteLine($"Admin user created with email: {adminEmail}");
        Console.WriteLine($"Default password: {adminPassword}");
    }
}
static async Task DoStartupThingsOnApp(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var settings = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();
        await settings.InitSettings();
    }
}