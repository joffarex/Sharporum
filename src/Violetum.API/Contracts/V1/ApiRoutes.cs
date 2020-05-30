namespace Violetum.API.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class Posts
        {
            public const string GetMany = Base + "/posts";
            public const string Create = Base + "/posts";
            public const string NewsFeed = Base + "/posts/news-feed";
            public const string Get = Base + "/posts/{postId}";
            public const string Update = Base + "/posts/{postId}";
            public const string Delete = Base + "/posts/{postId}";
            public const string Vote = Base + "/posts/{postId}/vote";
        }

        public static class Categories
        {
            public const string GetMany = Base + "/categories";
            public const string Create = Base + "/categories";
            public const string Get = Base + "/categories/{categoryId}";
            public const string Update = Base + "/categories/{categoryId}";
            public const string Delete = Base + "/categories/{categoryId}";
        }

        public static class Comments
        {
            public const string GetMany = Base + "/comments";
            public const string Create = Base + "/comments";
            public const string Get = Base + "/comments/{commentId}";
            public const string Update = Base + "/comments/{commentId}";
            public const string Delete = Base + "/comments/{commentId}";
            public const string Vote = Base + "/comments/{commentId}/vote";
        }

        public static class Profiles
        {
            public const string Get = Base + "/profiles/{profileId}";
            public const string Update = Base + "/profiles/{profileId}";
            public const string GetFollowers = Base + "/profiles/{profileId}/followers";
            public const string GetFollowing = Base + "/profiles/{profileId}/following";
            public const string Follow = Base + "/profiles/{profileId}/follow";
            public const string Unfollow = Base + "/profiles/{profileId}/unfollow";
        }
    }
}