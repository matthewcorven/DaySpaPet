using DaySpaPet.Web;
using DaySpaPet.Web.Interfaces;
using Microsoft.FluentUI.AspNetCore.Components;
using DaySpaPet.Web.Server.Services;
using DaySpaPet.Web.Server.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents(options => 
    options.DetailedErrors = builder.Environment.IsDevelopment())
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device specific services used by Razor Class Library (DaySpaPet.Web)
builder.Services.AddScoped<IFormFactor, FormFactor>();

builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents(options =>
        {
            options.UseTooltipServiceProvider = true;
        });
builder.Services.AddDaySpaAppComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<DaySpaPet.Web.Server.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(DaySpaPet.Web._Imports).Assembly);

app.Run();