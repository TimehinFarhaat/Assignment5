using CloudinaryDotNet;
using CSharpMvcBasics.Configuration;
using CSharpMvcBasics.Implementation.Repository;
using CSharpMvcBasics.Implementation.Service;
using CSharpMvcBasics.Interface.Repository;
using CSharpMvcBasics.Interface.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
});

var env = builder.Environment;

// 🔹 Load Configuration
builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

// 🔹 Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});

// 🔹 Clarifai
builder.Services.Configure<ClarifaiSettings>(builder.Configuration.GetSection("ClarifaiSettings"));
builder.Services.AddHttpClient();

// 🔹 Image Storage (Dev: Local, Prod: Cloudinary)
if (env.IsDevelopment())
    builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();
else
    builder.Services.AddScoped<IImageStorageService, CloudinaryStorageService>();

// 🔹 Database
var connectionString = builder.Configuration.GetConnectionString(
    env.IsDevelopment() ? "DefaultConnection" : "PostgresConnection");

Console.WriteLine($"📦 Using connection string: {connectionString}");
Console.WriteLine($"🌍 Environment: {env.EnvironmentName}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (env.IsDevelopment())
        options.UseSqlServer(connectionString);
    else
        options.UseNpgsql(connectionString);
});

// 🔹 MVC + Session
builder.Services.AddSession();
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

// 🔹 DI - Repositories and Services
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IClarifaiService, ClarifaiService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ITaxService, TaxImplementation>();

// 🔹 Build App
try
{
    var app = builder.Build();

    // 🔹 Middleware
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseSession();
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Fatal error during app startup: {ex}");
    throw;
}
