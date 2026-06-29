using System.Data;
using gestionUsuarios.Respository;
using gestionUsuarios.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

//Read the conection
var connectionString = builder.Configuration.GetConnectionString("PostgresConection");

builder.Services.AddTransient<IDbConnection>((sp)=> new NpgsqlConnection(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>() ;


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();