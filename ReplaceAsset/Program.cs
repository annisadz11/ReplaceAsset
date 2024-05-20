using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using ReplaceAsset.Data;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

// Layanan DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserAdmin", policy => policy.RequireRole("UserAdmin"));
    options.AddPolicy("UserEmployee", policy => policy.RequireRole("UserEmployee"));
    options.AddPolicy("UserIntern", policy => policy.RequireRole("UserIntern"));
    options.AddPolicy("UserManagerIT", policy => policy.RequireRole("UserManagerIT"));
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();

app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var identity = new ClaimsIdentity();
        var fullUsername = context.User.Identity.Name;
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var isUserAdmin = await dbContext.UserAdmins.AnyAsync(a => a.UserName == fullUsername);
        var isUserEmployee = await dbContext.UserEmployees.AnyAsync(a => a.UserName == fullUsername);
        var isUserIntern = await dbContext.UserInterns.AnyAsync(a => a.UserName == fullUsername);
        var isUserManagerIT = await dbContext.UserManagerITs.AnyAsync(a => a.UserName == fullUsername);

        if (isUserAdmin)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserAdmin"));
        }
        if (isUserEmployee)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserEmployee"));
        }
        if (isUserIntern)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserIntern"));
        }
        if (isUserManagerIT)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserManagerIT"));
        }

        context.User.AddIdentity(identity);
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();