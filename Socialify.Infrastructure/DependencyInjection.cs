
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Socialify.Application.RepoInterfaces;
using Socialify.Application.Interfaces;
using Socialify.Application.Services;
using Socialify.Infrastructure.Data.Repository;
using Socialify.Infrastructure.Identity;

namespace Socialify.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}
