using Inmobiliaria2024.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://localhost:5000", "http://localhost:5001", "http://*:5000", "http://*:5001");
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true, 
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["TokenAuthentication:Issuer"],
        ValidAudience = configuration["TokenAuthentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
            configuration["TokenAuthentication:SecretKey"])),
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/chatsegurohub") ||
                path.StartsWithSegments("/Propietario/reset") ||
                path.StartsWithSegments("/Propietario/token")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddDbContext<DataContext>(
	options => options.UseMySql(
		configuration["ConnectionStrings:DefaultConnection"],
		ServerVersion.AutoDetect(configuration["ConnectionStrings:DefaultConnection"])
	)
);
// builder.Services.AddDbContext<DataContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//     new MySqlServerVersion(new Version(8, 0, 2)) // Asegúrate de poner la versión correcta de tu MySQL
// ).EnableSensitiveDataLogging() // Esto permitirá mostrar los valores reales en los logs
// );
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAllOrigins",
//         builder => builder
//         .AllowAnyOrigin()
//         .AllowAnyMethod()
//         .AllowAnyHeader());
// });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseCors("AllowAllOrigins");
//app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin()
                .AllowAnyMethod()
				.AllowAnyHeader());


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
