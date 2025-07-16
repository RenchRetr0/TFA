using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;
using TFA.API.Middlewares;
using TFA.Domain;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.UseCase.CreateTopic;
using TFA.Domain.UseCase.GetForums;
using TFA.Storage;
using TFA.Storage.Storages;
using Forum = TFA.Domain.Models.Forum;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddLogging(b => b.AddSerilog(
   new LoggerConfiguration()
      .MinimumLevel.Debug()
      .Enrich.WithProperty("Application", "TFA.API")
      .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
      .WriteTo.Logger(lc => lc
         .Filter.ByExcluding(Matching.FromSource("Microsoft"))
         .WriteTo.OpenSearch(
            builder.Configuration.GetConnectionString("Logs"),
            "forum-logs-{0:yyyy:MM:dd}"
         )
      )
      .WriteTo.Logger(lc => lc
         .WriteTo.Console()
      )
      .CreateLogger()
));

string connectionString = builder.Configuration.GetConnectionString("Postgres")!;

builder.Services.AddScoped<IGetForumsUseCase, GetForumsUseCase>();
builder.Services.AddScoped<IGetForumsStorage, GetForumsStorage>();
builder.Services.AddScoped<ICreateTopicUseCase, CreateTopicUseCase>();
builder.Services.AddScoped<ICreateTopicStorage, CreateTopicStorage>();
builder.Services.AddScoped<IIntentionResolver, TopicIntentionResolver>();
builder.Services.AddScoped<IIntentionManager, IntentionManager>();
builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();

builder.Services.AddScoped<IGuidFactory, GuidFactory>();
builder.Services.AddScoped<IMomentProvider, MomentProvider>();

builder.Services.AddValidatorsFromAssemblyContaining<Forum>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<ForumDbContext>(options => options
   .UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

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
