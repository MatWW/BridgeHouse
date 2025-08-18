using Backend.Extensions;
using Backend.ExceptionHandlers;
using Backend.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddAuthorization()
    .AddCorsPolicies(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddHttpContextAccessor()
    .AddProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddApiDocumentation();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BridgeHub>("/gameHub");

app.UseApiDocumentation();

app.Run();
