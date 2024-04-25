using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MSC.Core.DB.Data;
using MSC.Core.Extensions;
using MSC.Core.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/***Custom Section Start***/
IConfiguration configuration = builder.Configuration; // allows both to access and to set up the config 
//DBContext
builder.Services.AddDbContext(configuration);
//add resources for DI
//cannot bind the full environment config inside the extension so keeping it here for referenc purposes
//using extension for getting the root pieces and for subsections creating the configure in root only
//builder.Services.Configure<EnvConfig>(configuration); 
builder.Services.AddServices(configuration);
//CORS -- using policy based
//builder.Services.AddCorsService(configuration);
var myAllowSpecificOrigins = builder.Services.AddCorsServicePolicyBased(configuration);
//Autentication
builder.Services.AddAuthenticationService(configuration);
/***Custom Section End***/

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//for the authorize button in swagger. Above AddAuthenticationService extension method is must to have
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(
    c => {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo{
            Title = "MysocialConnect",
            Version = "v1"
        });
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme(){
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header, 
            Description = "Example: \"Bearer 1safsfsdfdfd\"",
        });
        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement{
            {
                new OpenApiSecurityScheme{
                    Reference = new OpenApiReference{
                        Type = ReferenceType.SecurityScheme, 
                        Id = "Bearer"
                    }
                },
                new string[]{}
            }
        });
    }
);

var app = builder.Build();

/***Custom Section Middleware Start***/
app.UseMiddleware<ExceptionMiddleware>();
/***Custom Section Middleware End***/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/***Custom Section Start***/
//ordering is important here. UseCors before UseAuthentication and UseAuthentication before UseAuthorization
//using policy based cors and not simple
//app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200", "http://localhost:4200"));
app.UseCors(myAllowSpecificOrigins);
app.UseAuthentication();
/***Custom Section End***/

app.UseAuthorization();

app.MapControllers();

/***Custom Section Seed Data Start***/
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try{
    var context = services.GetRequiredService<DataContext>();
    //Asynchronously applies any pending migrations for the context to the database. Will create the database if it does not already exist.
    await context.Database.MigrateAsync();
    await SeedData.SeedUsers(context);
}
catch(Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during seeding data");
}
/***Custom Section Seed Data End***/

app.Run();
