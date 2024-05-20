using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using ReplaceAsset.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserAdminPolicy", policy => policy.RequireRole("UserAdmin"));
    options.AddPolicy("UserManagerITPolicy", policy => policy.RequireRole("UserManagerIT"));
    options.AddPolicy("UserInternPolicy", policy => policy.RequireRole("UserIntern"));
    options.AddPolicy("UserEmployeePolicy", policy => policy.RequireRole("UserEmployee"));
});});

builder.Services.AddRazorPages();

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
        var isUserManagerIT = await dbContext.UserManagerITs.AnyAsync(a => a.UserName == fullUsername);
        var isUserIntern = await dbContext.UserInterns.AnyAsync(a => a.UserName == fullUsername);
        var isUserEmployee = await dbContext.UserEmployees.AnyAsync(a => a.UserName == fullUsername);

        if (isUserAdmin)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserAdmin"));
        }

        if (isUserManagerIT)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserManagerIT"));
        }

        if (isUserIntern)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserIntern"));
        }

        if (isUserEmployee)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "UserEmployee"));
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
