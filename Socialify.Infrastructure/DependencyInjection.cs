using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Socialify.Application.Interfaces;
using Socialify.Application.Services;
using Socialify.Infrastructure.Identity;
using Socialify.Infrastructure.Repository;
using Socialify.Application.Services_Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Application.Repos_Interfaces;

namespace Socialify.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Core services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IHomePageService, HomePageService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IProfilePageService, ProfilePageService>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<ISavedPostService, SavedPostService>();
            services.AddScoped<ISavedPostRepository, SavedPostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();











            services.AddScoped<IFileManager, FileManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}
