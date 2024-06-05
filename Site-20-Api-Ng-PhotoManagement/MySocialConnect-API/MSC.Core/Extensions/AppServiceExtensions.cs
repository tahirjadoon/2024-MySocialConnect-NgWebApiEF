using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MSC.Core.ActionFilters;
using MSC.Core.BusinessLogic;
using MSC.Core.Constants;
using MSC.Core.DB.Data;
using MSC.Core.DB.Entities;
using MSC.Core.DB.UnitOfWork;
using MSC.Core.Helper;
using MSC.Core.Mappers;
using MSC.Core.Repositories;
using MSC.Core.Services;
using MSC.Core.SignalR;

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
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IMessageBusinessLogic, MessageBusinessLogic>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPhotoService, PhotoService>();
        //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //when have single project/assembly
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        //adding the Cloudinary to read data from
        //check programs.cs for ref: builder.Services.Configure<EnvConfig>(configuration);
        services.Configure<CloudinaryConfig>(config.GetSection(ConfigKeyConstants.CloudinarySettingsKey));
        
        //add the action filter as a service, it wil get applied to the abse controller
        services.AddScoped<LogUserActivityFilter>();

        services.AddSignalR();
        services.AddSingleton<PresenceTrackerMemory>();
        services.AddScoped<ISignalRRepository, SignalRRepository>();
        services.AddScoped<ISignalRBusinessLogic, SignalRBusinessLogic>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<IPhotoBusinessLogic, PhotoBusinessLogic>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

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
                                    .AllowAnyMethod()
                                    //signalR
                                    .AllowCredentials()
                                    ;
                                });
            });
        }
        return myAllowSpecificOrigins;
    }

    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration config)
    {
        //IR_REFACTOR - Identity, out it before services.AddAuthentication
        //for mvc use services.AddIdentity. For the api we can't do that
        services.AddIdentityCore<AppUser>(opt => {
            //there are a lot of options that we can configure here
            //pick per the site password scheme 
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
        })
        //roles
        .AddRoles<AppRole>()
        //role manager
        .AddRoleManager<RoleManager<AppRole>>()
        //signin manager
        //.AddSignInManager<SignInManager<AppUser>>()
        //role validator
        //.AddRoleValidator<RoleValidator<AppRole>>()
        //and finally add store to create all the identity related tables 
        .AddEntityFrameworkStores<DataContext>()
        ;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            //http authentication
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, 
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetTokenKey())), 
                ValidateIssuer = false, 
                ValidateAudience = false
            };

            //signalR or websockets cannot send authentication header. Have to use query string with SignalR
            //signalR, this allows the client to send the token as query string
            options.Events = new JwtBearerEvents
            {
                //pass it as a query string
                OnMessageReceived  = context => 
                {
                    //get access_token from the query
                    var accessToken = context.Request.Query["access_token"];
                    //check the path of the request and only do for signalr. Our paths starts with hubs/ check programs.cs for details
                    var path = context.HttpContext.Request.Path;
                    if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(opt => {
            opt.AddPolicy(SiteIdentityConstants.AuthPolicy_Admin, 
                            policy => policy.RequireRole(SiteIdentityConstants.Role_Admin));

            opt.AddPolicy(SiteIdentityConstants.AuthPolicy_Moderator_Photos, 
                            policy => policy.RequireRole(SiteIdentityConstants.Role_Admin, SiteIdentityConstants.Role_Moderator));
        });

        return services;
    }
}
