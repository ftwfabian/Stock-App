using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesStock.Data;
using Microsoft.AspNetCore.Identity;
using RazorPagesStock.Models;
using RazorPagesStock.Areas.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Explicitly set the URLs
//builder.WebHost.UseUrls("http://localhost:5058", "https://localhost:5001");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Add request logging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request path: {context.Request.Path}");
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

var clientAppPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp", "public");
Console.WriteLine($"ClientApp public path: {clientAppPath}");

// Serve static files from ClientApp/public
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(clientAppPath),
    RequestPath = "",
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/plain"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// Fallback route for SPA
app.MapFallbackToFile("index.html");

Console.WriteLine($"Application is now listening on: {string.Join(", ", app.Urls)}");

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        Console.WriteLine("Root path requested. Serving index.html");
        await context.Response.SendFileAsync(Path.Combine(clientAppPath, "index.html"));
    }
    else
    {
        await next();
    }
});

app.Run();

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = serviceProvider.GetRequiredService<ApplicationDbContext>())
        {
            if (!context.StockPrice.Any())
            {
                Random random = new Random();

                double aaplPrice = 1000;
                double msftPrice = 1200;
                double nvdaPrice = 1100;

                var endDate = DateTime.UtcNow.Date.AddDays(365);
                var startDate = DateTime.UtcNow.Date.AddDays(-365);

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    context.StockPrice.Add(new StockPrice
                    {
                        Ticker = "NVDA",
                        Price = nvdaPrice,
                        Date = date,
                        SharesOutstanding = 100000
                    });
                    context.StockPrice.Add(new StockPrice
                    {
                        Ticker = "AAPL",
                        Price = aaplPrice,
                        Date = date,
                        SharesOutstanding = 10000
                    });
                    context.StockPrice.Add(new StockPrice
                    {
                        Ticker = "MSFT",
                        Price = msftPrice,
                        Date = date,
                        SharesOutstanding = 1000
                    });
                    nvdaPrice += random.Next(2) == 0 ? -20 : 20;
                    msftPrice += random.Next(2) == 0 ? -10 : 10;
                    aaplPrice += random.Next(2) == 0 ? -10 : 10;
                }
                context.SaveChanges();
            }
        }
    }
}