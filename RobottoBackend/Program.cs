using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using robotto_backend.Areas.Identity;
using robotto_backend.Data;
using RobottoBackend.Services;
using RobottoBackend.Data.Repositories;
using RobottoBackend.Helpers;
using Radzen;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

builder.Services.AddSingleton<ITestRepository>(StartupHelpers.InitializeTestRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<IMissionRepository>(StartupHelpers.InitializeMissionRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<IResourceRepository>(StartupHelpers.InitializeResourceRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<INaturalHazardRepository>(StartupHelpers.InitializeNaturalHazardRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<IDroneTelemetryRepository>(StartupHelpers.InitializeDroneTelemetryRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<IDroneMetadataRepository>(StartupHelpers.InitializeDroneMetadataRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());

builder.Services.AddSingleton<IAzuriteService>(StartupHelpers.InitializeAzuriteClientInstanceAsync(
    builder.Configuration.GetSection("Azurite")).GetAwaiter().GetResult());

builder.Services.AddRadzenComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();