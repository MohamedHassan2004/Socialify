using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Socialify.Application.Interfaces;
using Socialify.Application.Services;
using Socialify.Infrastructure.Identity;
using Socialify.Infrastructure.Repository;
using Socialify.Application.Services_Interfaces;
using Socialify.Application.ReposInterfaces;

namespace Socialify.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<ISearchRepository, SearchRepository>();
            services.AddScoped<IHomePageService, HomePageService>();
            services.AddScoped<IProfilePageService, ProfilePageService>();



            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}
