using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.UseCase.CreateForum;
using TFA.Domain.UseCase.CreateTopic;
using TFA.Domain.UseCase.GetForums;
using TFA.Domain.UseCase.GetTopics;
using TFA.Storage.Storages;

namespace TFA.Storage.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumStorage(this IServiceCollection services, string dbConnectionString)
    {
        services
            .AddScoped<ICreateForumStorage, CreateForumStorage>()
            .AddScoped<IGetForumsStorage, GetForumsStorage>()
            .AddScoped<ICreateTopicStorage, CreateTopicStorage>()
            .AddScoped<IGetTopicsStorage, GetTopicsStorage>()
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddScoped<IMomentProvider, MomentProvider>()
            .AddDbContextPool<ForumDbContext>(options => options
                .UseNpgsql(dbConnectionString));

        services.AddMemoryCache();

        services.AddAutoMapper(config => config
            .AddMaps(Assembly.GetAssembly(typeof(ForumDbContext))));

        return services;
    }
}