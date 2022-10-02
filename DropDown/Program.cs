using DropDown.Data;
using DropDown.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connection = builder.Configuration.GetConnectionString("Connection");
#if DEBUG
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
#endif
builder.Services.AddDbContext<DropDownContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<AccountContext>(options =>
    options.UseSqlServer(connection));



builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<DropDownContext>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

builder.Services.AddDistributedMemoryCache();
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

app.UseAuthentication();

app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
