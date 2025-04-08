using CurrieTechnologies.Razor.SweetAlert2;
using DynamicPasswordPolicy.Database;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using Service.Repository;
using Service.ServiceImplementation;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddDbContext<AppDatabaseEntity>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IEncryptionDecryption, ServiceEncryptionDecryption>();
builder.Services.AddScoped<IRepository, ServiceRepository>();
builder.Services.AddScoped<IUser, ServiceUser>();
builder.Services.AddScoped<IAdminPanel, ServiceAdminPanel>();
builder.Services.AddSweetAlert2();  // Register SweetAlert2


builder.Services.AddSession();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
