namespace Violetum.Domain.Models
{
    public class UserTokens
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdentityToken { get; set; }
    }
}