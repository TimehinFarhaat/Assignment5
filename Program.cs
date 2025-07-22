
using CSharpMvcBasics.Configuration;
using CSharpMvcBasics.Implementation.Repository;
using CSharpMvcBasics.Implementation.Service;
using CSharpMvcBasics.Interface.Repository;
using CSharpMvcBasics.Interface.Services;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args); 

// Database

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




// Google Vision API key
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "Keys/identifyimageapp.json");
builder.Services.AddHttpClient(); 

//Clarifai settings
builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<ClarifaiSettings>(
    builder.Configuration.GetSection("ClarifaiSettings")
);



builder.Services.Configure<ClarifaiSettings>(builder.Configuration.GetSection("ClarifaiSettings"));




// TempData support using session
builder.Services.AddSession();
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

// DI


builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IClarifaiService, ClarifaiService>();
builder.Services.AddScoped<IGoogleVisionService, GoogleVisionService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ITaxService, TaxImplementation>();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession(); // ✅ Required for TempData to work

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
