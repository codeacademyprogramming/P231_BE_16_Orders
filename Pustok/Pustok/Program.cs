using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using Pustok;
using Pustok.DAL;
using Pustok.Models;
using Pustok.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddDbContext<PustokDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//builder.Services.AddSingleton<LayoutService>();
builder.Services.AddScoped<LayoutService>();
//builder.Services.AddTransient<LayoutService>();



builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 8;
    opt.Password.RequireNonAlphanumeric = false;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<PustokDbContext>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToAccessDenied = options.Events.OnRedirectToLogin = context =>
    {
        var uri = new Uri(context.RedirectUri);
        
        if (context.HttpContext.Request.Path.Value.StartsWith("/manage"))
            context.Response.Redirect("/manage/account/login"+uri.Query);
        else
            context.Response.Redirect("/account/login" + uri.Query);
        
        return Task.CompletedTask;
    };
});

builder.Services.AddSession(x =>
{
    x.IdleTimeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHttpContextAccessor();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
