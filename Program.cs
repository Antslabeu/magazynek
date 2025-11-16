
using Magazynek.Data;
using Magazynek.Entities;
using Magazynek.Services;
using Microsoft.EntityFrameworkCore;
using Magazynek.Data.Mailer;


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


builder.Services.AddScoped<IFlashService,              FlashService>();
builder.Services.AddScoped<ISystemSettingsService,     SystemSettingsService>();
builder.Services.AddScoped<ITmeService,                TmeService>();
builder.Services.AddScoped<IProductService,            ProductService>();
builder.Services.AddScoped<IShippingEntryService,      ShippingEntryService>();
builder.Services.AddScoped<IProjectService,            ProjectService>();
builder.Services.AddScoped<IProjectReservationService, ProjectReservationService>();


builder.Services.AddSingleton<INeededSetting,  NeededSetting>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IEmailSender,    MailKitEmailSender>();

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


await SeedRolesAndAdminAsync(app.Services);

app.Run();

static async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();

    // Utwórz admina, jeśli nie istnieje
    var adminLogin = "admin_magazynek@antslab.eu";
    var adminPassword = "Admin123!";
    var adminName = "System admin";

    User? adminUser = await sessionService.GetUser(User.UserRole.Admin);

    if (adminUser == null)
    {
        Guid activatorGuid = Guid.NewGuid();
        adminUser = new User(
            id: Guid.NewGuid(),
            name: adminName,
            login: adminLogin,
            password: adminPassword,
            userRole: User.UserRole.Admin,
            activatorGuid: activatorGuid
        );
        adminUser.SetActive(activatorGuid);
        await sessionService.PrepareUserTables(adminUser);
        await sessionService.AddNewUser(adminUser);
        Console.WriteLine($"Admin user created with login: {adminUser.login}");
        Console.WriteLine($"Default password: {adminPassword}");
    }
}