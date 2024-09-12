using Microsoft.EntityFrameworkCore;
using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatingAppDBContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddCors( opt => 
        opt.AddDefaultPolicy(policy => 
                policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(["http://localhost:4200", "https://localhost:4200"])
                ));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.MapControllers();

app.Run();
