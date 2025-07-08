using HellowWebAppSR.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HellowWebAppSR.Controllers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// In Program.cs, inside the builder.Services configuration section, add:
builder.Services.AddHttpClient<HellowWebAppSR.Services.IBingService, HellowWebAppSR.Services.BingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Default route - MUST BE LAST
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// New route for Index with random numbers
app.MapControllerRoute(
    name: "indexWithNumbers",
    pattern: "Home/Index/{num1}/{num2}/index.html",
    defaults: new { controller = "Home", action = "IndexWithNumbers" }
);

// New route for Privacy with random numbers
app.MapControllerRoute(
    name: "privacyWithNumbers",
    pattern: "Home/Privacy/{num1}/{num2}/privacy.html",
    defaults: new { controller = "Home", action = "PrivacyWithNumbers" }
);

//HomeController.StartKafkaConsumer();


app.Run();