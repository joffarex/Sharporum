using System;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Services;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Infrastructure;
using Sharporum.Infrastructure.Repositories;

namespace Sharporum.API.Installers
{
    public class CustomServicesInstaller : IInstaller

    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            services.AddMediatR(AppDomain.CurrentDomain.Load("Sharporum.Core"));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IVoteRepository, VoteRepository>();
            services.AddScoped<IAsyncRepository<Category>, CategoryRepository>();
            services.AddScoped<IAsyncRepository<Comment>, CommentRepository>();
            services.AddScoped<IAsyncRepository<Community>, CommunityRepository>();
            services.AddScoped<IAsyncRepository<Post>, PostRepository>();

            services.AddTransient<IBlobService, BlobService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<ICommunityService, CommunityService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IUserService, UserService>();
        }
    }
}