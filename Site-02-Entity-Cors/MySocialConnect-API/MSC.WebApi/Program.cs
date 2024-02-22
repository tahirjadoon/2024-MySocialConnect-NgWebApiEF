using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSC.Core.BusinessLogic;
using MSC.Core.Constants;
using MSC.Core.DB.Data;
using MSC.Core.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/***Custom Section Start***/
IConfiguration configuration = builder.Configuration; // allows both to access and to set up the config 

//DBContext and connection string 
//Migration assembly is needed since DBContext is in MSC.Core where as the Migrations are getting created in MSC.WebApi via 
//dotnet ef migrations add InitialCreate -o DbFile/Migrations
//if every thing is in MSC.WebApi then b is not needed. 
builder.Services.AddDbContext<DataContext>(opt => {
    opt.UseSqlite(builder.Configuration.GetConnectionString(ConfigKeyConstants.DefaultConnection), 
                    b => b.MigrationsAssembly("MSC.WebApi")
                );
    
});

//add resources for DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserBusinessLogic, UserBusinessLogic>();

//CORS
builder.Services.AddCors();
/***Custom Section End***/

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/***Custom Section Start***/

/***Custom Section End***/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/***Custom Section Start***/
//ordering is important here. UseCors before UseAuthentication and UseAuthentication before UseAuthorization
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200/", "http://localhost:4200/"));

/***Custom Section End***/

app.UseAuthorization();

app.MapControllers();

app.Run();
