using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Core.Domain.Settings;
using ArtemisBanking.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBanking.Infrastructure.Shared;

public static class ServiceRegistration
{
    public static void AddSharedLayerIoc(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MailSettings>(config.GetSection("MailSettings"));
        services.AddScoped<IEmailService, EmailService>();
    }
}