using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using robotto_backend.Areas.Identity;
using robotto_backend.Data;
using RobottoBackend.Services;

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
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<IAzuriteService>(InitializeAzuriteClientInstanceAsync(
    builder.Configuration.GetSection("Azurite")).GetAwaiter().GetResult());
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

static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
    string containerName = configurationSection.GetSection("ContainerName").Value ?? "";
    string account = configurationSection.GetSection("Account").Value ?? "";

    // If key is not set, assume we're using managed identity
    string key = configurationSection.GetSection("Key").Value ?? "";

    CosmosClientOptions options = new ()
    {
        HttpClientFactory = () => new HttpClient(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }),
        ConnectionMode = ConnectionMode.Gateway,
        LimitToEndpoint = true
    };
    
    CosmosClient client;
    client = new CosmosClient(account, key, clientOptions: options);
    
    CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
    DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return cosmosDbService;
}

static async Task<AzuriteService> InitializeAzuriteClientInstanceAsync(IConfigurationSection configurationSection)
{
    string connectionString = configurationSection.GetSection("ConnectionString").Value ?? "";;
    string containerName = configurationSection.GetSection("ContainerName").Value ?? "";

    var blobServiceClient = new BlobServiceClient(connectionString);

    if (!await ContainerExists(blobServiceClient, containerName))
    {
        await blobServiceClient.CreateBlobContainerAsync(containerName);
    }

    var blobContainerClient = new BlobContainerClient(connectionString, containerName);
    return new AzuriteService(blobContainerClient);
}

static async Task<bool> ContainerExists(BlobServiceClient blobServiceClient, string containerName)
{
    var containers = blobServiceClient.GetBlobContainersAsync(prefix: containerName);
    await foreach (var page in containers)
    {
        if (page.Name == containerName)
            return true;
    }
    
    return false;
}