using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using robotto_backend.Areas.Identity;
using robotto_backend.Data;
using RobottoBackend.Services;
using RobottoBackend.Data.Repositories;

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

builder.Services.AddSingleton<ITestRepository>(InitializeTestRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<IMissionRepository>(InitializeMissionRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<IResourceRepository>(InitializeResourceRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<INaturalHazardRepository>(InitializeNaturalHazardRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<IDroneTelemetryRepository>(InitializeDroneTelemetryRepositoryAsync(
    builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<IDroneMetadataRepository>(InitializeDroneMetadataRepositoryAsync(
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

static async Task<TestRepository> InitializeTestRepositoryAsync(IConfigurationSection configurationSection)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
    string containerName = configurationSection.GetSection("TestContainerName").Value ?? "";
    var client = GetCosmosDbClient(configurationSection);
    
    TestRepository repository = new TestRepository(client, databaseName, containerName);
    DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return repository;
}

static async Task<MissionRepository> InitializeMissionRepositoryAsync(IConfigurationSection configurationSection)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
    string containerName = configurationSection.GetSection("MissionContainerName").Value ?? "";
    var client = GetCosmosDbClient(configurationSection);
    
    MissionRepository repository = new MissionRepository(client, databaseName, containerName);
    DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return repository;
}

static async Task<ResourceRepository> InitializeResourceRepositoryAsync(IConfigurationSection configurationSection)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
    string containerName = configurationSection.GetSection("ResourceContainerName").Value ?? "";
    var client = GetCosmosDbClient(configurationSection);
    
    ResourceRepository repository = new ResourceRepository(client, databaseName, containerName);
    DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return repository;
}

static async Task<NaturalHazardRepository> InitializeNaturalHazardRepositoryAsync(IConfigurationSection configurationSection)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
    string containerName = configurationSection.GetSection("NaturalHazardContainerName").Value ?? "";
    var client = GetCosmosDbClient(configurationSection);
    
    NaturalHazardRepository repository = new NaturalHazardRepository(client, databaseName, containerName);
    DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return repository;
}

static async Task<DroneMetadataRepository> InitializeDroneMetadataRepositoryAsync(IConfigurationSection configurationSection)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
    string containerName = configurationSection.GetSection("DroneMetadataContainerName").Value ?? "";
    var client = GetCosmosDbClient(configurationSection);
    
    DroneMetadataRepository repository = new DroneMetadataRepository(client, databaseName, containerName);
    DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return repository;
}

static async Task<DroneTelemetryRepository> InitializeDroneTelemetryRepositoryAsync(IConfigurationSection configurationSection)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
    string containerName = configurationSection.GetSection("DroneTelemetryContainerName").Value ?? "";
    var client = GetCosmosDbClient(configurationSection);
    
    DroneTelemetryRepository repository = new DroneTelemetryRepository(client, databaseName, containerName);
    DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return repository;
}

static CosmosClient GetCosmosDbClient(IConfigurationSection configurationSection)
{    
    string account = configurationSection.GetSection("Account").Value ?? "";
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
    
    return new CosmosClient(account, key, clientOptions: options);
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