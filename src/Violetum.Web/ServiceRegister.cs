using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Violetum.ApplicationCore;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.Services;
using Violetum.ApplicationCore.Validators;
using Violetum.Domain.Infrastructure;
using Violetum.Infrastructure.Repositories;
using Violetum.Web.Infrastructure;
using Violetum.Web.Validators.Category;
using Violetum.Web.Validators.Comment;
using Violetum.Web.Validators.Post;
using Violetum.Web.Validators.Profile;

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

            @this.AddTransient<IValidator<PostDto>, PostDtoValidator>();
            @this.AddTransient<IValidator<UpdatePostDto>, UpdatePostDtoValidator>();
            @this.AddTransient<IValidator<CommentDto>, CommentDtoValidator>();
            @this.AddTransient<IValidator<UpdateCommentDto>, UpdateCommentDtoValidator>();
            @this.AddTransient<IValidator<CategoryDto>, CategoryDtoValidator>();
            @this.AddTransient<IValidator<UpdateCategoryDto>, UpdateCategoryDtoValidator>();
            @this.AddTransient<IValidator<UpdateProfileDto>, UpdateProfileDtoValidator>();
            return @this;
        }
    }
}