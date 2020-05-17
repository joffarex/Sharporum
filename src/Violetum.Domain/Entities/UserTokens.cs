namespace Violetum.Domain.Entities
{
    public class UserTokens
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdentityToken { get; set; }
    }
}