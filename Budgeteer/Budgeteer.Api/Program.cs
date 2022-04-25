using Budgeteer.Api;
using Budgeteer.Application.Domain;
using Budgeteer.Application.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var serviceCollection = builder.Services; 
var builderConfiguration = builder.Configuration;

RegisterDbContext(serviceCollection, builderConfiguration);
RegisterServices(serviceCollection, builderConfiguration);
ConfigureIdentity(serviceCollection);

builder.Services.AddControllers();
serviceCollection.AddEndpointsApiExplorer();
serviceCollection.AddSwaggerGen();

var app = builder.Build();
ConfigureApp(app);
app.Run();

void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddScoped<TokenService>();
    serviceCollection.AddCors(o => o.AddPolicy("AllowAny", b =>
    {
        b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));
}

void RegisterDbContext(IServiceCollection services, IConfiguration configuration)
{
    services.AddDbContext<BudgeteerContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsAssembly("Budgeteer.Application"))
    );
}

void ConfigureIdentity(IServiceCollection services)
{
    services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<BudgeteerContext>();
    services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 0;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.AllowedForNewUsers = false;
    });
}

void ConfigureApp(WebApplication application)
{
    if (application.Environment.IsDevelopment())
    {
        application.UseSwagger();
        application.UseSwaggerUI();
    }
    
    application.UseHttpsRedirection();
    application.UseAuthorization();
    
    application.UseCors("AllowAny");
    
    application.MapControllers();
    
}