using CloudinaryDotNet;
using CSharpMvcBasics.Configuration;
using CSharpMvcBasics.Implementation.Repository;
using CSharpMvcBasics.Implementation.Service;
using CSharpMvcBasics.Interface.Repository;
using CSharpMvcBasics.Interface.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;




try
{
    var app = builder.Build();

    // Your middleware pipeline here

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error during app startup: {ex}");
    throw;
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
});

// 🔹 Configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});



if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
}
Console.WriteLine($"ASPNETCORE_ENVIRONMENT = {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

//// 🔹 Database
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Clarifai
builder.Services.Configure<ClarifaiSettings>(
    builder.Configuration.GetSection("ClarifaiSettings"));
builder.Services.AddHttpClient();

// 🔹 Session + MVC
builder.Services.AddSession();
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

// 🔹 Dependency Injection
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IClarifaiService, ClarifaiService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ITaxService, TaxImplementation>();

// 🔹 Build App
var app = builder.Build();

// 🔹 Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
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
