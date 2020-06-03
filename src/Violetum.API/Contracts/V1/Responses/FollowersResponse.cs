namespace Violetum.API.Contracts.V1.Responses
{
    public class FollowersResponse<TViewModel>
    {
        public TViewModel Followers { get; set; }
    }
}