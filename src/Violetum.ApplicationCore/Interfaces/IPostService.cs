using Violetum.ApplicationCore.ViewModels;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface IPostService
    {
        PostViewModel GetPost(string postId);
    }
}