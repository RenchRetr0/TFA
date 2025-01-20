using Microsoft.EntityFrameworkCore;
using TFA.Domain;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.UseCase.CreateTopic;
using TFA.Domain.UseCase.GetForums;
using TFA.Storage;
using TFA.Storage.Storages;

var builder = WebApplication.CreateBuilder(args);

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

app.Run();
