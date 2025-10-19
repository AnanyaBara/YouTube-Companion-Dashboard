using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;
using YouTubeCompanion.Data;
using YouTubeCompanion.Repositories;
using YouTubeCompanion.Services;
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "YouTube Companion API", Version = "v1" });
});


// DB - Postgres via connection string from env
var conn = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DATABASE_URL") ?? "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=ytdb;";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(conn));


// Application services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IEventLogger, EventLogger>();
builder.Services.AddScoped<IYouTubeService, YouTubeService>();


// CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


var app = builder.Build();


// Ensure DB created (for dev). For production use migrations.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();