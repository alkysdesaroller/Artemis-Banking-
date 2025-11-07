using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBanking.Infrastructure.Shared;

public static class ServiceRegistration
{
    public static void AddSharedLayerIoc(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
    }
}