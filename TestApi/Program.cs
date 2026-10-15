using Microsoft.EntityFrameworkCore;
using Test.Commands;
using Test.EFEntities;
using Test.Interface;
using Test.Queries;
using Test.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TestDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 10 * 1024 * 1024; // 10 MB
    options.MaxSendMessageSize = 10 * 1024 * 1024; // 10 MB
});

builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
                        typeof(Program).Assembly,
                        typeof(BulkInsertUsersCommandHandlers).Assembly,
                        typeof(GetAllUsersQueryHandler).Assembly
                            );
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "My API V1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at the app's root
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
