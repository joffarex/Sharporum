using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Violetum.ApplicationCore;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.Services;
using Violetum.ApplicationCore.Validators;
using Violetum.Domain.Infrastructure;
using Violetum.Infrastructure.Repositories;
using Violetum.Web.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceRegister
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection @this)
        {
            Type serviceType = typeof(Service);
            IEnumerable<TypeInfo> definedType = serviceType.Assembly.DefinedTypes;

            IEnumerable<TypeInfo> services = definedType
                .Where(x => x.GetTypeInfo().GetCustomAttribute<Service>() != null);

            foreach (TypeInfo service in services)
            {
                @this.AddTransient(service);
            }

            @this.AddTransient<ICategoryValidators, CategoryValidators>();
            @this.AddTransient<IPostValidators, PostValidators>();
            @this.AddTransient<ICommentValidators, CommentValidators>();
            @this.AddTransient<IUserValidators, UserValidators>();

            @this.AddTransient<IProfileService, ProfileService>();
            @this.AddTransient<ICategoryService, CategoryService>();
            @this.AddTransient<IPostService, PostService>();
            @this.AddTransient<ICommentService, CommentService>();
            @this.AddTransient<IFollowerService, FollowerService>();

            @this.AddTransient<ITokenManager, TokenManager>();
            @this.AddTransient<IVoteRepository, VoteRepository>();
            @this.AddTransient<IPostRepository, PostRepository>();
            @this.AddTransient<ICategoryRepository, CategoryRepository>();
            @this.AddTransient<ICommentRepository, CommentRepository>();
            @this.AddTransient<IFollowerRepository, FollowerRepository>();
            return @this;
        }
    }
}