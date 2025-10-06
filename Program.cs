using Magazynek.Data;
using Magazynek.Entities;
using Magazynek.Services;
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

string connectionString = builder.Configuration.GetConnectionString("MainDB")!;
builder.Services.AddDbContextFactory<DatabaseContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.LogTo(Console.WriteLine, LogLevel.Error);
});

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

app.Run();
