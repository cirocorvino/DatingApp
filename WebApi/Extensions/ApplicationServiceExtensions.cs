using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Interfaces;
using WebApi.Services;

namespace WebApi.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();

        services.AddDbContext<DatingAppDBContext>(options =>
            options.UseSqlite(config.GetConnectionString("DefaultConnection"))
        );

        services.AddCors();
        // builder.Services.AddCors( opt => 
        //         opt.AddDefaultPolicy(policy => 
        //                 policy
        //                 .AllowAnyHeader()
        //                 .AllowAnyMethod()
        //                 .WithOrigins(["http://localhost:4200", "https://localhost:4200"])
        //                 ));

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository,UserRepository>();
        services.AddScoped<ILikesRepository, LikesRepository>();    
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

        return services;
    }
}
