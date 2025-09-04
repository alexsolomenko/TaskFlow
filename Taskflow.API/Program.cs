using Microsoft.EntityFrameworkCore;
using Taskflow.API.Services;
using Taskflow.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// test
//using var scope = app.Services.CreateScope();
//var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
//#pragma warning disable CS4014
//context.Database.OpenConnectionAsync().ContinueWith(t =>
//{
//    var canConnect = context.Database.CanConnect();
//});
//#pragma warning restore CS4014
//end test

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
