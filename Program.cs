using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    policy.RequireClaim(ClaimTypes.Email, "admin@gmail.com"));

});
//builder.Services.AddAuthorizationBuilder()
//    .AddPolicy("AdminPolicy", policy =>
//    policy.RequireClaim(ClaimTypes.Email, "abcd@gmail.com"));



builder.Services.AddScoped<IProductRepository,ProductRepository>(provider =>
    new ProductRepository(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True"));
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>(provider =>
    new CategoryRepository(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True"));
builder.Services.AddScoped<IOrderRepository,OrderRepository>(provider =>
    new OrderRepository(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True"));
builder.Services.AddScoped<IRepository<OrderDetail>>(provider => new GenericRepository<OrderDetail>(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True"));
builder.Services.AddScoped<IRepository<Brand>, GenericRepository<Brand>>(provider => new GenericRepository<Brand>(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True"));
builder.Services.AddScoped<IRepository<Unit>>(provider => new GenericRepository<Unit>(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True"));



builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    //app.UseStatusCodePages();
    app.UseStatusCodePagesWithRedirects("/ErrorPages/{0}");
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
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
