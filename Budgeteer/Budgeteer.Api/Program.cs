using Budgeteer.Api;
using Budgeteer.Application.Domain;
using Budgeteer.Application.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

RegisterServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

void RegisterServices(IServiceCollection services)
{
    builder.Services.AddControllers();

    services.AddScoped<TokenService>();
    
    services.AddDbContext<BudgeteerContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsAssembly("Budgeteer.Application"))
        );

    services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<BudgeteerContext>();
    
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}