using System.Collections.Generic;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.Web.Models
{
    public class CategoryPageViewModel
    {
        public CategoryViewModel Category { get; set; }
        public IEnumerable<PostViewModel> Posts { get; set; }
    }
}