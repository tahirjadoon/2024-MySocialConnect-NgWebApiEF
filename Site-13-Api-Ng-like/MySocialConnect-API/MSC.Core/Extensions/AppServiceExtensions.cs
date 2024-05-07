using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MSC.Core.ActionFilters;
using MSC.Core.BusinessLogic;
using MSC.Core.Constants;
using MSC.Core.DB.Data;
using MSC.Core.Helper;
using MSC.Core.Mappers;
using MSC.Core.Repositories;
using MSC.Core.Services;

namespace MSC.Core.Extensions;

public static class AppServiceExtensions
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        //***** check Program_site03_before_extensions.txt for details - in the root of the project setup*******

        //DBContext and connection string 
        //Migration assembly is needed since DBContext is in MSC.Core where as the Migrations are getting created in MSC.WebApi via 
        //dotnet ef migrations add InitialCreate -o DbFile/Migrations
        //if every thing is in MSC.WebApi then b is not needed. 
        services.AddDbContext<DataContext>(opt => {
            opt.UseSqlite(config.GetDefaultConnectionString(), 
                            b => b.MigrationsAssembly("MSC.WebApi")
                        );
            
        });
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserBusinessLogic, UserBusinessLogic>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        services.AddScoped<ILikesBusinessLogic, LikesBusinessLogic>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPhotoService, PhotoService>();
        //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //when have single project/assembly
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        //adding the Cloudinary to read data from
        //check programs.cs for ref: builder.Services.Configure<EnvConfig>(configuration);
        services.Configure<CloudinaryConfig>(config.GetSection(ConfigKeyConstants.CloudinarySettingsKey));
        
        //add the action filter as a service, it wil get applied to the abse controller
        services.AddScoped<LogUserActivityFilter>();

        return services;
    }

    public static IServiceCollection AddCorsService(this IServiceCollection services, IConfiguration config)
    {
        services.AddCors();
        return services;
    }

    public static string AddCorsServicePolicyBased(this IServiceCollection services, IConfiguration config)
    {
        var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        //https://stackoverflow.com/questions/42858335/how-to-hardcode-and-read-a-string-array-in-appsettings-json
        var allowedSpecificOrigins = config.GetAllowSpecificOrigins();
        if (allowedSpecificOrigins != null && allowedSpecificOrigins.Any())
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                                policy =>
                                {
                                    policy.WithOrigins(allowedSpecificOrigins.ToArray())
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                                });
            });
        }
        return myAllowSpecificOrigins;
    }

    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, 
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetTokenKey())), 
                ValidateIssuer = false, 
                ValidateAudience = false
            };
        });
        return services;
    }
}
