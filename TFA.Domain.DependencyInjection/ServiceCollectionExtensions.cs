using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.UseCase.CreateTopic;
using TFA.Domain.UseCase.GetForums;
using TFA.Domain.UseCase.GetTopics;

namespace TFA.Domain.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumDomain(this IServiceCollection services)
    {
        services
            .AddScoped<IGetForumsUseCase, GetForumsUseCase>()
            .AddScoped<ICreateTopicUseCase, CreateTopicUseCase>()
            .AddScoped<IGetTopicsUseCase, GetTopicsUseCase>()
            .AddScoped<IIntentionResolver, TopicIntentionResolver>();

        services
            .AddScoped<IIntentionManager, IntentionManager>()
            .AddScoped<IIdentityProvider, IdentityProvider>();

        services
            .AddValidatorsFromAssemblyContaining<Forum>(includeInternalTypes: true);

        services.AddMemoryCache();

        return services;
    }
}