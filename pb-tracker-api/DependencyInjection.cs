using Microsoft.Extensions.DependencyInjection;
using pb_tracker_api.Auth;
using pb_tracker_api.Repositories;

namespace pb_tracker_api;

public static class DependencyInjection
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPwdService, PwdService>();
        services.AddScoped<ITokenService, TokenService>();
    }

    public static void AddRepos(this IServiceCollection services)
    {
        services.AddScoped<IUserRepo, UserRepo>();
    }
}

