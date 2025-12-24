using System.Reflection;
using AutoMapper;
using TFA.API.Authentication;
using TFA.API.DependencyInjection;
using TFA.API.Middlewares;
using TFA.Domain.Authentication;
using TFA.Domain.DependencyInjection;
using TFA.Storage.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiLogging(builder.Configuration, builder.Environment);
builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Authentication").Bind);
builder.Services.AddScoped<IAuthTokenStorage, AuthTokenStorage>();

builder.Services
   .AddForumDomain()
   .AddForumStorage(builder.Configuration.GetConnectionString("Postgres")!);

builder.Services.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

var mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapControllers();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.Run();

public partial class Program { }
