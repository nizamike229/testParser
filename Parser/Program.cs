using Npgsql;
using WebApplication1.BackgroundServices;
using WebApplication1.Interfaces;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddTransient<NpgsqlConnection>(_ =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
services.AddTransient<INewsService, NewsService>();
services.AddTransient<IUserService, UserService>();
services.AddHostedService<MyHostedService>();   

services.AddDistributedMemoryCache();

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();