﻿using System.Collections.Generic;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.ViewModels;

namespace Violetum.Web.Models
{
    public class PostPageViewModel
    {
        public PostViewModel Post { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }
        public CommentDto CommentDto { get; set; }
    }
}