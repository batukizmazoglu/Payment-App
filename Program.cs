using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using Payment.Client;
using Payment.Shared.Interfaces;
using Payment.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<CustomerService>();

builder.Services.AddScoped<CompanyService>();

builder.Services.AddScoped<BillService>();

builder.Services.AddScoped<UserService>();

// builder.Services.AddScoped<IUserInterface,UserService >();

builder.Services.AddMudServices();

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

// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

// builder.Services.AddAuthorization();
//
// app.UseAuthentication();
//
// app.UseAuthorization();

app.Run();
