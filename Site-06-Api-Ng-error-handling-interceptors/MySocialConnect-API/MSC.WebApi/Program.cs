using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
builder.Services.AddSwaggerGen();

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

app.Run();
