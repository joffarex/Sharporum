using System.Collections.Generic;
using Violetum.ApplicationCore.ViewModels;

namespace Violetum.Web.Models
{
    public class CategoryPageViewModel
    {
        public CategoryViewModel Category { get; set; }
        public IEnumerable<PostViewModel> Posts { get; set; }
    }
}