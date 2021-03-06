﻿using Sharporum.Core.ViewModels.User;

namespace Sharporum.Core.ViewModels.Post
{
    public class PostViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public UserBaseViewModel Author { get; set; }
        public PostCommunityViewModel Community { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public int VoteCount { get; set; }
    }
}